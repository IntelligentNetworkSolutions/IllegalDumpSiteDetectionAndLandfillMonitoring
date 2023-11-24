using Dal.ApplicationStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    public class UserManagementDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<UserManagementDa> _logger;
        public UserManagementDa(ApplicationDbContext db, ILogger<UserManagementDa> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsers()
        {
            try
            {
                var users =  await _db.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsersExcludingCurrent(string id)
        {
            try
            {
                var users = await _db.Users.Where(x => x.Id != id).ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            try
            {
                return await _db.Users.Where(z => z.UserName == username).
                FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<IdentityUserClaim<string>>> GetClaimsForUser(string userId)
        {
            try
            {
                return await _db.UserClaims.Where(z => z.UserId == userId && z.ClaimType == "AuthorizationClaim").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<IdentityRole>> GetRolesForUser(string userId)
        {
            try
            {
                var userRoles = await _db.UserRoles.Where(z => z.UserId == userId).Select(z => z.RoleId).ToListAsync();

                return await _db.Roles.Where(z => userRoles.Contains(z.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> DeleteUser(string id)
        {
            try
            {
                var user = await _db.Users.FindAsync(id);
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            try
            {
                return await _db.Users.Where(z => z.Email == email).
                FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<IdentityRole> AddRole(IdentityRole role)
        {
            try
            {
                role.NormalizedName = role.Name.ToUpper();
                _db.Add(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public void AddClaimForRole(string id, string claim)
        {
            try
            {
                IdentityRoleClaim<string> forInsert = new IdentityRoleClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.RoleId = id;
                _db.Add(forInsert);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        public void AddClaimForUser(string id, string claim)
        {
            try
            {
                IdentityUserClaim<string> forInsert = new IdentityUserClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.UserId = id;
                _db.Add(forInsert);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> UpdateUser(UserManagementEditUserViewModel user)
        {
            try
            {
                var findUser = _db.Users.Find(user.Id);
                findUser.FirstName = user.FirstName;
                findUser.LastName = user.LastName;
                findUser.UserName = user.UserName;
                findUser.PhoneNumber = user.PhoneNumber;
                findUser.Id = user.Id;
                findUser.Email = user.Email;
                findUser.IsActive = user.IsActive ?? false;
                if (user.UserName != " " && user.UserName != null)
                {
                    findUser.NormalizedUserName = user.UserName.ToUpper();
                }
                if (user.Email != " " && user.Email != null)
                {
                    findUser.NormalizedEmail = user.Email.ToUpper();
                }
                if (user.ConfirmPassword != " " && user.ConfirmPassword != null)
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, user.ConfirmPassword);
                    findUser.PasswordHash = hashed;
                }
                _db.Update(findUser);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user)
        {
            try
            {
               
                _db.Update(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
        public void DeleteClaimsRolesForUser(ApplicationUser user)
        {
            try
            {
                var userClaims = _db.UserClaims.Where(z => z.UserId == user.Id && z.ClaimType == "AuthorizationClaim").ToList();
                var userRoles = _db.UserRoles.Where(z => z.UserId == user.Id).ToList();
                foreach (var claim in userClaims)
                {
                    _db.Remove(claim);
                }
                foreach (var role in userRoles)
                {
                    _db.Remove(role);
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<IdentityUserRole<string>> AddRoleForUser(string userId, string roleId)
        {
            try
            {
                IdentityUserRole<string> userRole = new IdentityUserRole<string>();
                userRole.UserId = userId;
                userRole.RoleId = roleId;
                _db.Add(userRole);
                await _db.SaveChangesAsync();
                return userRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> AddUser(ApplicationUser user)
        {
            try
            {
                user.NormalizedUserName = user.UserName.ToUpper();
                user.NormalizedEmail = user.Email.ToUpper();
                _db.Add(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ICollection<IdentityRole>> GetRoles()
        {
            try
            {
                return await _db.Roles.
                ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<IdentityRoleClaim<string>>> GetAllRoleClaims()
        {
            try
            {
                return await _db.RoleClaims.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<IdentityRole> DeleteRole(string id)
        {
            try
            {
                var role = await _db.Roles.FindAsync(id);
                _db.Roles.Remove(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<IdentityRole> GetRole(string roleId)
        {
            try
            {
                return await _db.Roles.Where(z => z.Id == roleId).
                FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<IdentityRole> UpdateRole(IdentityRole role)
        {
            try
            {
                var findRole = _db.Roles.Find(role.Id);
                findRole.Name = role.Name;
                if (role.Name != " " && role.Name != null)
                {
                    findRole.NormalizedName = role.Name.ToUpper();
                }
                _db.Update(findRole);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public void DeleteClaimsForRole(IdentityRole role)
        {
            try
            {
                var roleClaims = _db.RoleClaims.Where(z => z.RoleId == role.Id && z.ClaimType == "AuthorizationClaim").ToList();
                foreach (var claim in roleClaims)
                {
                    _db.Remove(claim);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<IdentityRoleClaim<string>>> GetClaimsForRole(string roleId)
        {
            try
            {
                return await _db.RoleClaims.Where(z => z.RoleId == roleId && z.ClaimType == "AuthorizationClaim").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<string> GetPreferredLanguage(string userId)
        {
            try
            {
                return _db.UserClaims.Where(z => z.UserId == userId && z.ClaimType == "PreferedLanguageClaim").FirstOrDefault() != null ?
                _db.UserClaims.Where(z => z.UserId == userId && z.ClaimType == "PreferedLanguageClaim").FirstOrDefault().ClaimValue : "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ApplicationUser> GetUser(string userId)
        {
            try
            {
                return await _db.Users.Where(z => z.Id == userId).
                FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task AddLanguageClaimForUser(string userId, string claimValue)
        {
            try
            {
                var claimDb = _db.UserClaims.Where(z => z.UserId == userId && z.ClaimType == "PreferedLanguageClaim").FirstOrDefault();
                if (claimDb != null)
                {
                    claimDb.ClaimValue = claimValue;
                    _db.Update(claimDb);
                }
                else
                {
                    IdentityUserClaim<string> forInsert = new IdentityUserClaim<string>();
                    forInsert.ClaimType = "PreferedLanguageClaim";
                    forInsert.ClaimValue = claimValue;
                    forInsert.UserId = userId;
                    await _db.AddAsync(forInsert);
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<ICollection<IdentityUserRole<string>>> GetUserRoles()
        {
            try
            {
                return await _db.UserRoles.
                ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<ClaimDTO>> GetIntranetUserClaimsDb(string userId)
        {
            try
            {
                var userRoles = _db.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
                var roleClaims = new List<ClaimDTO>();

                var userClaims = _db.UserClaims.Where(x => x.UserId == userId).Select(x => new ClaimDTO() { ClaimValue = x.ClaimValue, ClaimType = x.ClaimType }).ToList();
                foreach (var role in userRoles)
                {
                    var claimForRole = _db.RoleClaims.Where(x => x.RoleId == role).Select(x => new ClaimDTO() { ClaimValue = x.ClaimValue, ClaimType = x.ClaimType }).ToList();
                    roleClaims.AddRange(claimForRole);
                }

                List<ClaimDTO> claimsForAdd = new List<ClaimDTO>();
                claimsForAdd.AddRange(roleClaims);
                claimsForAdd.AddRange(userClaims);
                var distinctedClaimsForAdd = claimsForAdd.Distinct().ToList();

                return distinctedClaimsForAdd;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
