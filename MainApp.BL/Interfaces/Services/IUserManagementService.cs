using DTOs.MainApp.BL;
using Microsoft.AspNetCore.Identity;
using SD;

namespace Services.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task AddUser(UserManagementDTO dto);
        Task AddRole(RoleManagementDTO dto);
        Task AddLanguageClaimForUser(string userId, string culture);

        Task UpdateUser(UserManagementDTO dto);
        Task<ResultDTO> UpdateUserPassword(string userId, string currentPassword, string password);
        Task UpdateRole(RoleManagementDTO dto);

        Task DeleteUser(string userId);
        Task DeleteRole(string roleId);
        
        Task<UserManagementDTO> FillUserManagementDto(UserManagementDTO dto);
        Task<RoleManagementDTO> FillRoleManagementDto(RoleManagementDTO dto);

        IQueryable<IdentityRole> GetRolesAsQueriable();

        Task<ICollection<UserDTO>> GetAllIntanetPortalUsers();
        Task<ICollection<RoleDTO>> GetAllRoles();
        Task<UserDTO?> GetUserById(string userId);
        Task<RoleDTO?> GetRoleById(string roleId);
        Task<List<RoleDTO>> GetRolesForUser(string userId);
        Task<List<RoleClaimDTO>> GetRoleClaims(string roleId);
        Task<List<UserClaimDTO>> GetUserClaims(string userId);
        Task<ICollection<UserRoleDTO>> GetAllUserRoles();
        Task<List<ClaimDTO>> GetClaimsForUser(string userId);
        Task<string?> GetPreferredLanguageForUser(string userId);
    }
}
