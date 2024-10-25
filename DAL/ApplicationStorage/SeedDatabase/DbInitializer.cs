using DAL.ApplicationStorage.SeedDatabase.ModulesConfigs.MMDetectionSetup;
using DAL.Interfaces.Helpers;
using Entities;
using Entities.MapConfigurationEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json.Linq;
using SD;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ApplicationStorage.SeedDatabase
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger, IAppSettingsAccessor appSettingsAccessor, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _appSettingsAccessor = appSettingsAccessor;
            _configuration = configuration;

        }

        public void Initialize(bool? runMigrations, bool? loadModules, List<string> modulesToLoad)
        {
            Console.WriteLine("Initialize Started");
            Console.WriteLine(_db.Database.GetConnectionString());
            try
            {
                if (runMigrations.HasValue && runMigrations.Value == true)
                {
                    if (_db.Database.GetPendingMigrations().Count() > 0)
                    {
                        _db.Database.Migrate();
                    }
                }

                //Seed application settings 
                Console.WriteLine("SeedApplicationSettings Started");
                SeedApplicationSettings();
                Console.WriteLine("SeedApplicationSettings Ended");
                //Seed roles
                Console.WriteLine("SeedRoles Started");
                SeedRoles();
                Console.WriteLine("SeedRoles Ended");
                //Seed users
                Console.WriteLine("SeedUsers Started");
                SeedUsers();
                Console.WriteLine("SeedUsers Ended");
                //TODO: load modules logic
                if (loadModules.HasValue && loadModules.Value)
                {
                    foreach (string module in modulesToLoad)
                    {
                        switch (module)
                        {
                            case "/module:MMDetectionSetup":
                                {
                                    Console.WriteLine("MMDetectionSetup Started");
                                    var mm = new MMDetectionSetupService(_configuration, _db);
                                    ResultDTO seedTrainedModelsResult = mm.GetAndSeedTrainedModelsFromSeedFile();
                                    
                                    if (seedTrainedModelsResult.IsSuccess == false && seedTrainedModelsResult.ExObj is null)
                                        Console.WriteLine(seedTrainedModelsResult.ErrMsg!);
                                    else if (seedTrainedModelsResult.IsSuccess == false && seedTrainedModelsResult.ExObj is not null)
                                        Console.WriteLine(((Exception)seedTrainedModelsResult.ExObj).InnerException is null
                                            ? ((Exception)seedTrainedModelsResult.ExObj).Message
                                            : ((Exception)seedTrainedModelsResult.ExObj).InnerException!.Message);
                                    else
                                        Console.WriteLine("SUCCESSFULL Seed of TrainedModels");

                                    Console.WriteLine("MMDetectionSetup Ended");
                                    break;
                                }
                            default:
                                throw new InvalidOperationException("Module Name not recognized");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private void SeedApplicationSettings()
        {
            var appSettingsPath = _configuration["SeedDatabaseFilePaths:SeedDatabaseApplicationSettings"];
            if (string.IsNullOrEmpty(appSettingsPath))
            {
                return;
            }

            if (!_db.ApplicationSettings.Any())
            {
                var appSettingsData = File.ReadAllText(appSettingsPath);
                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());
                var appSettings = JsonSerializer.Deserialize<List<ApplicationSettings>>(appSettingsData, options);
                if (appSettings?.Count > 0)
                {
                    _db.ApplicationSettings.AddRange(appSettings);
                }
                _db.SaveChanges();
            }
        }

        private void SeedRoles()
        {
            var rolesPath = _configuration["SeedDatabaseFilePaths:SeedDatabaseUsers"];
            if (string.IsNullOrEmpty(rolesPath))
            {
                return;
            }

            if (!_db.Roles.Any())
            {
                var rolesData = File.ReadAllText(rolesPath);
                var jsonArray = JArray.Parse(rolesData);
                var rolesDataJson = jsonArray.FirstOrDefault(obj => obj["InitialRoles"] != null);

                if (rolesDataJson != null)
                {
                    var rolesArray = rolesDataJson["InitialRoles"] as JArray;
                    var roles = rolesArray?.ToObject<List<IdentityRole>>();
                    if (roles?.Count > 0)
                    {
                        _db.Roles.AddRange(roles);
                    }
                    _db.SaveChanges();
                }
            }
        }

        private void SeedUsers()
        {
            var usersPath = _configuration["SeedDatabaseFilePaths:SeedDatabaseUsers"];
            if (string.IsNullOrEmpty(usersPath))
            {
                return;
            }

            if (!_db.Users.Any())
            {
                var usersData = File.ReadAllText(usersPath);
                var jsonArray = JArray.Parse(usersData);
                var superadminDataJson = jsonArray.FirstOrDefault(obj => obj["Superadmin"] != null);
                var initialUsersDataJson = jsonArray.FirstOrDefault(obj => obj["InitialUsers"] != null);
                var defaultPasswordAdminUsers = _configuration["SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForAdminUsers"];
                var defaultPasswordSuperAdminUser = _configuration["SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForSuperAdmin"];

                //seed super admin
                if (superadminDataJson != null)
                {
                    if (string.IsNullOrEmpty(defaultPasswordSuperAdminUser))
                    {
                        return;
                    }

                    var superadmin = superadminDataJson["Superadmin"]?.ToObject<ApplicationUser>();
                    if (superadmin != null)
                    {
                        var resultCreatedSuperAdmin = _userManager.CreateAsync(superadmin, defaultPasswordSuperAdminUser).GetAwaiter().GetResult();
                        if (resultCreatedSuperAdmin.Succeeded)
                        {
                            // Add claims to super admin
                            var defaultSuperAdminUserName = _configuration["SeedDatabaseDefaultValues:SeedDatabaseDefaultSuperAdminUserName"];
                            if (string.IsNullOrEmpty(defaultSuperAdminUserName))
                            {
                                return;
                            }
                            var superAdminUser = _db.Users.Where(x => x.UserName == defaultSuperAdminUserName).FirstOrDefault();
                            var superAdminRole = _db.Roles.Where(x => x.Name == "SuperAdmin").FirstOrDefault();
                            if (superAdminUser != null)
                            {
                                List<Claim> claims = new List<Claim>
                                {
                                    new Claim("SpecialAuthClaim", "superadmin"),
                                    new Claim("PreferedLanguageClaim", "en")
                                };
                                _userManager.AddClaimsAsync(superAdminUser, claims).GetAwaiter().GetResult();

                                //Assign super admin to role superadmin
                                if (superAdminRole != null && !string.IsNullOrEmpty(superAdminRole.Name))
                                {
                                    _db.UserRoles.Add(new IdentityUserRole<string>
                                    {
                                        UserId = superAdminUser.Id,
                                        RoleId = superAdminRole.Id
                                    });
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                //seed other users
                if (initialUsersDataJson != null)
                {
                    var initialUsersArray = initialUsersDataJson["InitialUsers"] as JArray;
                    var users = initialUsersArray?.ToObject<List<dynamic>>();
                    if (string.IsNullOrEmpty(defaultPasswordAdminUsers))
                    {
                        return;
                    }

                    if (users?.Count > 0)
                    {
                        foreach (var user in users)
                        {
                            string userUserName = user.UserName;
                            var applicationUser = new ApplicationUser
                            {
                                UserName = userUserName
                            };
                            var resultUserCreated = _userManager.CreateAsync(applicationUser, defaultPasswordAdminUsers).GetAwaiter().GetResult();

                            //Assign user to roles
                            if (resultUserCreated.Succeeded)
                            {
                                var rolesArray = user.Roles as JArray;
                                var roleValues = rolesArray?.ToObject<List<string>>();
                                var createdAdminUser = _db.Users.Where(x => x.UserName == applicationUser.UserName).FirstOrDefault();
                                if (createdAdminUser != null && roleValues != null)
                                {
                                    foreach (var role in roleValues)
                                    {
                                        var roleDb = _db.Roles.Where(x => x.Name == role).FirstOrDefault();
                                        if (roleDb != null)
                                        {
                                            _db.UserRoles.Add(new IdentityUserRole<string>
                                            {
                                                UserId = createdAdminUser.Id,
                                                RoleId = roleDb.Id
                                            });
                                        }
                                    }
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SeedMMDetection()
        {

        }
    }
}
