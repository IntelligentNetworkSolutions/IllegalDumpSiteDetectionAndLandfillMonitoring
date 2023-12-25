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

        Task<List<IdentityUserClaim<string>>> GetClaimsForUser(string userId);

        Task<List<IdentityRole>> GetRolesForUser(string userId);

        Task<ApplicationUser> GetUserByEmail(string email);

        Task<ICollection<IdentityRole>> GetRoles();

        Task<IdentityRole> GetRole(string roleId);

        Task<List<IdentityRoleClaim<string>>> GetAllRoleClaims();
        Task<List<IdentityRoleClaim<string>>> GetClaimsForRole(string roleId);

        Task<ApplicationUser> GetUser(string userId);
        Task<ICollection<IdentityUserRole<string>>> GetUserRoles();


        Task<string> GetPreferredLanguage(string userId);

        Task AddLanguageClaimForUser(string userId, string claimValue);

        Task<IdentityRole> AddRole(IdentityRole role);

        void AddClaimForRole(string id, string claim);

        void AddClaimForUser(string id, string claim);

        Task<IdentityUserRole<string>> AddRoleForUser(string userId, string roleId);

        Task<ApplicationUser> AddUser(ApplicationUser user);

        Task<IdentityRole> UpdateRole(IdentityRole role);
        Task<ApplicationUser> UpdateUser(ApplicationUser user);

        void DeleteClaimsRolesForUser(ApplicationUser user);

        void DeleteClaimsForRole(IdentityRole role);
        Task<IdentityRole> DeleteRole(string id);

        Task<ApplicationUser> DeleteUser(string id);


    }
}
