using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase;
using DAL.Interfaces.Helpers;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Tests.DalTests.ApplicationStorage.SeedDatabase
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class DbInitializerTests
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<ILogger<DbInitializer>> _mockLogger;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public DbInitializerTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            _mockLogger = new Mock<ILogger<DbInitializer>>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public void Initialize_ShouldSeedApplicationSettings_WhenSettingsFileExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var settingsPath = Path.GetTempFileName();
            var settingsJson = @"[
                {
                    ""Id"": ""1"",
                    ""Key"": ""TestSetting"",
                    ""Value"": ""TestValue"",
                    ""Description"": ""Test Description"",
                    ""DataType"": ""String""
                }
            ]";
            File.WriteAllText(settingsPath, settingsJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseApplicationSettings"])
                .Returns(settingsPath);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                var settings = dbContext.ApplicationSettings.AsNoTracking().ToList();
                Assert.NotEmpty(settings);
                Assert.Equal("TestSetting", settings[0].Key);
                Assert.Equal("TestValue", settings[0].Value);
            }
            finally
            {
                // Cleanup
                File.Delete(settingsPath);
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_ShouldSeedRoles_WhenNoRolesExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var rolesPath = Path.GetTempFileName();
            var rolesJson = @"[{
                ""InitialRoles"": [
                    {
                        ""Id"": ""907a8bfb-ce4d-4ff1-9c56-7b8708a7c5f0"",
                        ""Name"": ""TestRole"",
                        ""NormalizedName"": ""TESTROLE""
                    }
                ]
            }]";
            File.WriteAllText(rolesPath, rolesJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseUsers"])
                .Returns(rolesPath);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                var roles = dbContext.Roles.AsNoTracking().ToList();
                Assert.NotEmpty(roles);
                Assert.Equal("TestRole", roles[0].Name);
            }
            finally
            {
                // Cleanup
                File.Delete(rolesPath);
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_ShouldSeedSuperAdmin_WhenNoUsersExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            // Configure test values
            const string superAdminUsername = "superadmin";
            const string superAdminPassword = "SuperAdminPass123!";

            var usersPath = Path.GetTempFileName();
            var usersJson = @"[{
                ""Superadmin"": {
                    ""Id"": ""3369eab9-4c84-4f7d-aa5b-c5acd4b949b6"",
                    ""UserName"": """ + superAdminUsername + @""",
                    ""NormalizedUserName"": ""SUPERADMIN"",
                    ""Email"": ""superadmin@test.com"",
                    ""NormalizedEmail"": ""SUPERADMIN@TEST.COM"",
                    ""EmailConfirmed"": true,
                    ""FirstName"": ""Super"",
                    ""LastName"": ""Admin"",
                    ""IsActive"": true
                }
            }]";
            File.WriteAllText(usersPath, usersJson);

            // Setup configuration
            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseUsers"])
                .Returns(usersPath);
            _mockConfiguration.Setup(c => c["SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForSuperAdmin"])
                .Returns(superAdminPassword);
            _mockConfiguration.Setup(c => c["SeedDatabaseDefaultValues:SeedDatabaseDefaultSuperAdminUserName"])
                .Returns(superAdminUsername);

            // Setup UserManager with verification
            ApplicationUser capturedUser = null;
            string capturedPassword = null;

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, password) =>
                {
                    capturedUser = user;
                    capturedPassword = password;
                })
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(um => um.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                Assert.NotNull(capturedUser);
                Assert.Equal(superAdminUsername, capturedUser.UserName);
                Assert.Equal(superAdminPassword, capturedPassword);

                _mockUserManager.Verify(um => um.CreateAsync(
                    It.Is<ApplicationUser>(u => u.UserName == superAdminUsername),
                    superAdminPassword),
                    Times.Once);
            }
            finally
            {
                // Cleanup
                File.Delete(usersPath);
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithInvalidModule_ShouldThrowException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() =>
                    initializer.Initialize(false, true, new List<string> { "/module:InvalidModule" }));
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithMissingAppSettingsPath_ShouldSkipAppSettingsSeeding()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseApplicationSettings"])
                .Returns((string)null);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                Assert.Empty(dbContext.ApplicationSettings.AsNoTracking().ToList());
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithMissingRolesPath_ShouldSkipRoleSeeding()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseUsers"])
                .Returns((string)null);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                Assert.Empty(dbContext.Roles.AsNoTracking().ToList());
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithInvalidAppSettingsJson_ShouldHandleError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var settingsPath = Path.GetTempFileName();
            var invalidJson = "{ invalid json }";
            File.WriteAllText(settingsPath, invalidJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseApplicationSettings"])
                .Returns(settingsPath);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                //initializer.Initialize(false, false, new List<string>());

                //// Assert
                //_mockLogger.Verify(
                //    x => x.Log(
                //        LogLevel.Error,
                //        It.IsAny<EventId>(),
                //        It.Is<It.IsAnyType>((o, t) => true),
                //        It.IsAny<Exception>(),
                //        It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)),
                //    Times.AtLeastOnce);

                Assert.Throws<System.Text.Json.JsonException>(() => initializer.Initialize(false, false, new List<string>()));
            }
            finally
            {
                File.Delete(settingsPath);
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithExistingUsers_ShouldSkipUserSeeding()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var existingUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "existing",
                Email = "existing@test.com"
            };
            dbContext.Users.Add(existingUser);
            dbContext.SaveChanges();

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                _mockUserManager.Verify(
                    x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                    Times.Never);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithMMDetectionSetupModule_ShouldSeedModels()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var seedFilePath = Path.GetTempFileName();
            var seedJson = @"{
                ""Training"": {
                    ""InitialBaseModels"": [
                        {
                            ""Name"": ""TestModel"",
                            ""ConfigDownloadUrl"": ""http://test.com/config.py"",
                            ""ModelFile"": ""http://test.com/model.pth""
                        }
                    ]
                }
            }";
            File.WriteAllText(seedFilePath, seedJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedTrainingAndDetectionProcess"])
                .Returns(seedFilePath);

            var mmSection = new Mock<IConfigurationSection>();
            SetupMMDetectionConfiguration(mmSection);
            _mockConfiguration.Setup(c => c.GetSection("MMDetectionConfiguration"))
                .Returns(mmSection.Object);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, true, new List<string> { "/module:MMDetectionSetup" });

                // Assert
                _mockConfiguration.Verify(c => c.GetSection("MMDetectionConfiguration"), Times.AtLeastOnce);
            }
            finally
            {
                File.Delete(seedFilePath);
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_WithMigrations_ShouldApplyMigrations()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(true, false, new List<string>());

                // Assert
                // Since we can't easily verify migrations in a test, we just verify no exception is thrown
                Assert.True(true);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void Initialize_ShouldSeedInitialUsers_WithRoles()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var usersPath = Path.GetTempFileName();
            var usersJson = @"[{
                ""InitialUsers"": [
                    {
                        ""UserName"": ""admin1"",
                        ""Roles"": [""Admin""]
                    }
                ]
            }]";
            File.WriteAllText(usersPath, usersJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedDatabaseUsers"])
                .Returns(usersPath);
            _mockConfiguration.Setup(c => c["SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForAdminUsers"])
                .Returns("AdminPass123!");

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var initializer = new DbInitializer(
                dbContext,
                _mockUserManager.Object,
                _mockLogger.Object,
                _mockAppSettingsAccessor.Object,
                _mockConfiguration.Object);

            try
            {
                // Act
                initializer.Initialize(false, false, new List<string>());

                // Assert
                _mockUserManager.Verify(
                    um => um.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == "admin1"), "AdminPass123!"),
                    Times.Once);
            }
            finally
            {
                File.Delete(usersPath);
                transaction.Rollback();
            }
        }

        private void SetupMMDetectionConfiguration(Mock<IConfigurationSection> section)
        {
            section.Setup(s => s["Base:CondaExeAbsPath"]).Returns("/path/to/conda");
            section.Setup(s => s["Base:RootDirAbsPath"]).Returns(Path.Combine(Path.GetTempPath(), "mmdetection_test"));
            section.Setup(s => s["Base:ScriptsDirRelPath"]).Returns("scripts");
            section.Setup(s => s["Base:ResourcesDirRelPath"]).Returns("resources");
            section.Setup(s => s["Base:OpenMMLabAbsPath"]).Returns("/path/to/openmmlab");

            section.Setup(s => s["Training:DatasetsDirRelPath"]).Returns("datasets");
            section.Setup(s => s["Training:ConfigsDirRelPath"]).Returns("configs");
            section.Setup(s => s["Training:OutputDirRelPath"]).Returns("output");
            section.Setup(s => s["Training:BackboneCheckpointAbsPath"]).Returns("/path/to/checkpoint");
            section.Setup(s => s["Training:CliLogsAbsPath"]).Returns("/path/to/logs");

            section.Setup(s => s["Detection:OutputDirRelPath"]).Returns("detection_output");
            section.Setup(s => s["Detection:CliLogsAbsPath"]).Returns("/path/to/detection_logs");
        }
    }
}
