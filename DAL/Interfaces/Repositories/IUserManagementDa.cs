using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces.Repositories
{
    public interface IUserManagementDa
    {
        Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsers();

        Task<ICollection<ApplicationUser>> GetAllIntanetPortalUsersExcludingCurrent(string id);

        Task<ApplicationUser> GetUserByUsername(string username);

        Task<List<IdentityUserClaim<string>>> GetClaimsForUserByUserIdAndClaimType(string userId, string claimType);

        Task<List<IdentityRole>> GetRolesForUser(List<string> userRoles);

        Task<ApplicationUser> GetUserByEmail(string email);

        Task<ICollection<IdentityRole>> GetRoles();

        Task<IdentityRole> GetRole(string roleId);

        Task<List<IdentityRoleClaim<string>>> GetAllRoleClaims();
        Task<List<IdentityRoleClaim<string>>> GetClaimsForRoleByRoleIdAndClaimType(string roleId, string claimType);

        Task<ApplicationUser> GetUserById(string userId);
        Task<ICollection<IdentityUserRole<string>>> GetUserRoles();
        Task<ICollection<IdentityUserRole<string>>> GetUserRolesByUserId(string userId);

        Task<string> GetPreferredLanguage(string userId);

        //Task AddLanguageClaimForUser(string userId, string claimValue);
        Task AddLanguageClaimForUser(IdentityUserClaim<string> claimDb);
        Task UpdateLanguageClaimForUser(IdentityUserClaim<string> claimDb);

        Task<IdentityRole> AddRole(IdentityRole role);

        Task AddClaimForRole(IdentityRoleClaim<string> forInsert);

        Task AddClaimForUser(IdentityUserClaim<string> forInsert);

        Task<IdentityUserRole<string>> AddRoleForUser(IdentityUserRole<string> userRole);

        Task<ApplicationUser> AddUser(ApplicationUser user);

        Task<IdentityRole> UpdateRole(IdentityRole role);
        Task<ApplicationUser> UpdateUser(ApplicationUser user);

        Task DeleteClaimsRolesForUser(List<IdentityUserClaim<string>> userClaims, ICollection<IdentityUserRole<string>> userRoles);

        Task DeleteClaimsForRole(List<IdentityRoleClaim<string>> roleClaims);
        Task<IdentityRole> DeleteRole(IdentityRole role);

        Task<ApplicationUser> DeleteUser(ApplicationUser user);


    }
}
