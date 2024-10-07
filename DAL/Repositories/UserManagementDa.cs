using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<ApplicationUser>> GetAllIntanetPortalUsers()
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

        public async Task<ApplicationUser> GetUserBySpecificClaim()
        {
            try
            {
                return await _db.Users.Where(u => _db.UserClaims.Any(c => c.UserId == u.Id && c.ClaimType == "SpecialAuthClaim" && c.ClaimValue == "superadmin")).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ApplicationUser>> GetAllIntanetPortalUsersExcludingCurrent(string id)
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
                return await _db.Users.FirstOrDefaultAsync(z => z.Id == userId);
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

        #endregion

        #region Roles and Claims
        public async Task<List<IdentityRole>> GetAllRoles()
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

        public async Task<List<IdentityUserRole<string>>> GetUserRoles()
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

        public async Task<List<IdentityUserRole<string>>> GetUserRolesByUserId(string userId)
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

        public async Task<List<IdentityUserRole<string>>> AddRolesForUserRange(List<IdentityUserRole<string>> userRoles, IDbContextTransaction? transaction = null)
        {
            IDbContextTransaction? localTransaction = transaction;

            try
            {
                if (localTransaction == null)
                {
                    localTransaction = await _db.Database.BeginTransactionAsync();
                }

                foreach (var userRole in userRoles)
                {
                    _db.Add(userRole);
                }

                await _db.SaveChangesAsync();

                if (transaction == null)
                {
                    await localTransaction.CommitAsync();
                }

                return userRoles;
            }
            catch (Exception ex)
            {
                if (transaction == null && localTransaction != null)
                {
                    await localTransaction.RollbackAsync();
                }

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

        public async Task AddClaimsForUserRange(List<IdentityUserClaim<string>>? userClaims, IDbContextTransaction? transaction = null)
        {
            IDbContextTransaction? localTransaction = transaction;

            try
            {
                if (localTransaction == null)
                {
                    localTransaction = await _db.Database.BeginTransactionAsync();
                }

                if (userClaims != null)
                {
                    foreach (var claim in userClaims)
                    {
                        _db.Add(claim);
                    }
                    await _db.SaveChangesAsync();

                }

                if (transaction == null)
                {
                    await localTransaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                if (transaction == null && localTransaction != null)
                {
                    await localTransaction.RollbackAsync();
                }

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
                return resUpdate >= 0;
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

        public bool? CheckUserBeforeDelete(string userId)
        {
            try
            {
                return _db.Datasets.Any(x => x.CreatedById == userId) ||
                    _db.Datasets.Any(x => x.UpdatedById == userId) ||
                    _db.DatasetClasses.Any(x => x.CreatedById == userId) ||
                    _db.DatasetImages.Any(x => x.CreatedById == userId) ||
                    _db.DatasetImages.Any(x => x.UpdatedById == userId) ||
                    _db.ImageAnnotations.Any(x => x.CreatedById == userId) ||
                    _db.ImageAnnotations.Any(x => x.UpdatedById == userId) ||
                    _db.MapConfigurations.Any(x => x.CreatedById == userId) ||
                    _db.MapConfigurations.Any(x => x.UpdatedById == userId) ||
                    _db.MapLayerGroupConfigurations.Any(x => x.CreatedById == userId) ||
                    _db.MapLayerGroupConfigurations.Any(x => x.UpdatedById == userId) ||
                    _db.MapLayerConfigurations.Any(x => x.CreatedById == userId) ||
                    _db.MapLayerConfigurations.Any(x => x.UpdatedById == userId) ||
                    _db.DetectionIgnoreZones.Any(x => x.CreatedById == userId) ||
                    _db.DetectionRuns.Any(x => x.CreatedById == userId) ||
                    _db.TrainedModels.Any(x => x.CreatedById == userId) ||
                    _db.TrainingRuns.Any(x => x.CreatedById == userId);
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
                                                   List<IdentityUserRole<string>> userRoles,
                                                   IDbContextTransaction? transaction = null)
        {
            IDbContextTransaction? localTransaction = transaction;

            try
            {
                if (localTransaction == null)
                {
                    localTransaction = await _db.Database.BeginTransactionAsync();
                }

                foreach (var claim in userClaims)
                {
                    _db.Remove(claim);
                }

                foreach (var role in userRoles)
                {
                    _db.Remove(role);
                }

                await _db.SaveChangesAsync();

                if (transaction == null)
                {
                    await localTransaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                if (transaction == null && localTransaction != null)
                {
                    await localTransaction.RollbackAsync();
                }

                _logger.LogError(ex.Message);
                throw;
            }
        }



        public async Task DeleteClaimsForRole(List<IdentityRoleClaim<string>> roleClaims, IDbContextTransaction? transaction = null)
        {
            IDbContextTransaction? localTransaction = transaction;

            try
            {
                if (localTransaction == null)
                {
                    localTransaction = await _db.Database.BeginTransactionAsync();
                }

                foreach (var claim in roleClaims)
                {
                    _db.Remove(claim);
                }

                await _db.SaveChangesAsync();

                if (transaction == null)
                {
                    await localTransaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                if (transaction == null && localTransaction != null)
                {
                    await localTransaction.RollbackAsync();
                }

                _logger.LogError(ex.Message);
                throw;
            }
        }


        #endregion
    }
}
