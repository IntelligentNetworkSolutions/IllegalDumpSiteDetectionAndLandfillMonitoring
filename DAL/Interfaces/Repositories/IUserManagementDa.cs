using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces.Repositories
{
    public interface IUserManagementDa
    {
        #region Read

        #region Get User/s
        Task<ApplicationUser?> GetUserById(string userId);
        Task<ApplicationUser> GetUserBySpecificClaim();

        Task<ApplicationUser?> GetUserByUsername(string username);

        Task<ApplicationUser?> GetUserByEmail(string email);

        Task<List<ApplicationUser>> GetAllIntanetPortalUsers();

        Task<List<ApplicationUser>> GetAllIntanetPortalUsersExcludingCurrent(string id);
        #endregion

        #region Roles and Claims
        Task<List<IdentityRole>> GetAllRoles();

        Task<IdentityRole?> GetRole(string roleId);

        Task<List<IdentityRole>> GetRolesForUser(List<string> userRoles);

        Task<List<IdentityRoleClaim<string>>> GetAllRoleClaims();

        Task<List<IdentityUserRole<string>>> GetUserRoles();

        Task<List<IdentityUserRole<string>>> GetUserRolesByUserId(string userId);

        Task<List<IdentityUserClaim<string>>> GetClaimsForUserByUserIdAndClaimType(string userId, string claimType);
        
        Task<List<IdentityRoleClaim<string>>> GetClaimsForRoleByRoleIdAndClaimType(string roleId, string claimType);

        Task<string> GetPreferredLanguage(string userId);
        #endregion

        #region Queryables
        IQueryable<ApplicationUser> GetAppUsersAsQueriable();

        IQueryable<IdentityRole> GetRolesAsQueriable();
        #endregion
        
        #endregion

        #region Create
        Task<ApplicationUser> AddUser(ApplicationUser user);

        Task<IdentityRole> AddRole(IdentityRole role);

        Task AddClaimForRole(IdentityRoleClaim<string> forInsert);

        Task AddClaimForUser(IdentityUserClaim<string> forInsert);

        Task<IdentityUserRole<string>> AddRoleForUser(IdentityUserRole<string> userRole);

        Task AddLanguageClaimForUser(IdentityUserClaim<string> claimDb);
        #endregion

        #region Update
        Task<bool> UpdateUser(ApplicationUser user);

        Task<bool> UpdateRole(IdentityRole role);

        Task UpdateLanguageClaimForUser(IdentityUserClaim<string> claimDb);
        #endregion

        #region Delete
        Task<ApplicationUser> DeleteUser(ApplicationUser user);
        bool? CheckUserBeforeDelete(string userId);

        Task<IdentityRole> DeleteRole(IdentityRole role);

        Task DeleteClaimsForRole(List<IdentityRoleClaim<string>> roleClaims);

        Task DeleteClaimsRolesForUser(List<IdentityUserClaim<string>> userClaims, List<IdentityUserRole<string>> userRoles);
        #endregion
    }
}
