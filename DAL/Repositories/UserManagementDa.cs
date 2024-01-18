using DAL.ApplicationStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces.Repositories;

namespace DAL.Repositories
{
    public class UserManagementDa : IUserManagementDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<UserManagementDa> _logger;
        public UserManagementDa(ApplicationDbContext db, ILogger<UserManagementDa> logger)
        {
            _db = db;
            _logger = logger;
        }

        #region Read
        
        #region Get User/s
        public async Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsers()
        {
            try
            {
                return await _db.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsersExcludingCurrent(string id)
        {
            try
            {
                return await _db.Users.Where(x => x.Id != id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserById(string userId)
        {
            try
            {
                return await _db.Users.SingleOrDefaultAsync(z => z.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByUsername(string username)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(z => z.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByEmail(string email)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(z => z.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        #endregion

        #region Roles and Claims
        public async Task<ICollection<IdentityRole>> GetRoles()
        {
            try
            {
                return await _db.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IdentityRole?> GetRole(string roleId)
        {
            try
            {
                return await _db.Roles.FirstOrDefaultAsync(z => z.Id == roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
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
                throw;
            }
        }

        public async Task<List<IdentityUserClaim<string>>> GetClaimsForUserByUserIdAndClaimType(string userId, string claimType)
        {
            try
            {
                return await _db.UserClaims.Where(z => z.UserId == userId && z.ClaimType == claimType).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
      
        public async Task<List<IdentityRole>> GetRolesForUser(List<string> userRoles)
        {
            try
            {
                return await _db.Roles.Where(z => userRoles.Contains(z.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ICollection<IdentityUserRole<string>>> GetUserRoles()
        {
            try
            {
                return await _db.UserRoles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ICollection<IdentityUserRole<string>>> GetUserRolesByUserId(string userId)
        {
            try
            {
                return await _db.UserRoles.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<IdentityRoleClaim<string>>> GetClaimsForRoleByRoleIdAndClaimType(string roleId, string claimType)
        {
            try
            {
                return await _db.RoleClaims.Where(z => z.RoleId == roleId && z.ClaimType == claimType).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
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
                throw;
            }
        }
        #endregion

        #region Queryables
        public IQueryable<ApplicationUser> GetAppUsersAsQueriable()
            => _db.Users.AsSingleQuery();

        public IQueryable<IdentityRole> GetRolesAsQueriable()
            => _db.Roles.AsSingleQuery();
        #endregion
        #endregion

        #region Create
        public async Task<ApplicationUser> AddUser(ApplicationUser user)
        {
            try
            {
                _db.Add(user);

                await _db.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IdentityRole> AddRole(IdentityRole role)
        {
            try
            {                
                _db.Add(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IdentityUserRole<string>> AddRoleForUser(IdentityUserRole<string> userRole)
        {
            try
            {
                _db.Add(userRole);

                await _db.SaveChangesAsync();

                return userRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task AddClaimForRole(IdentityRoleClaim<string> forInsert)
        {
            try
            {               
                _db.Add(forInsert);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task AddClaimForUser(IdentityUserClaim<string> forInsert)
        {
            try
            {                
                _db.Add(forInsert);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task AddLanguageClaimForUser(IdentityUserClaim<string> claimDb)
        {
            try
            {
                await _db.AddAsync(claimDb);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Update
        public async Task<bool> UpdateUser(ApplicationUser user)
        {
            try
            {
                _db.Update(user);
                int resUpdate = await _db.SaveChangesAsync();
                return resUpdate > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateRole(IdentityRole role)
        {
            try
            {
                _db.Update(role);
                int resUpdate = await _db.SaveChangesAsync();
                return resUpdate > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task UpdateLanguageClaimForUser(IdentityUserClaim<string> claimDb)
        {
            try
            {
                _db.Update(claimDb);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Delete
        public async Task<ApplicationUser> DeleteUser(ApplicationUser user)
        {
            try
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IdentityRole> DeleteRole(IdentityRole role)
        {
            try
            {
                _db.Roles.Remove(role);

                await _db.SaveChangesAsync();

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteClaimsRolesForUser(List<IdentityUserClaim<string>> userClaims, 
                                                    ICollection<IdentityUserRole<string>> userRoles)
        {
            try
            {
                foreach (var claim in userClaims)
                    _db.Remove(claim);
                
                foreach (var role in userRoles)
                    _db.Remove(role);
                
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteClaimsForRole(List<IdentityRoleClaim<string>> roleClaims)
        {
            try
            {
                foreach (var claim in roleClaims)
                    _db.Remove(claim);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
