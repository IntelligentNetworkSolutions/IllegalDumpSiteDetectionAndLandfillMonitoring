using DTOs.MainApp.BL;
using Entities;
using Microsoft.AspNetCore.Identity;
using SD;

namespace Services.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<(int passMinLenght, bool passHasLetters, bool passHasNumbers)> GetPasswordRequirements();
        Task<(bool hasMinLength, bool hasLetters, bool hasNumbers)> CheckPasswordRequirements(string password);
        Task<ResultDTO<string>> GetPasswordHashForAppUser(ApplicationUser appUser, string passwordNew);


        Task<ResultDTO> AddUser(UserManagementDTO dto);
        Task<ResultDTO> AddUserRoles(string appUserId, List<string> rolesInsert);
        Task<ResultDTO> AddUserClaims(string appUserId, List<string> claimsInsert, string claimsType);
        Task<ResultDTO> AddRole(RoleManagementDTO dto);
        Task<ResultDTO> AddLanguageClaimForUser(string userId, string culture);

        Task<ResultDTO> UpdateUser(UserManagementDTO dto);
        Task<ResultDTO> UpdateUserPassword(string userId, string currentPassword, string password);
        Task<ResultDTO> UpdateRole(RoleManagementDTO dto);

        Task<ResultDTO> DeleteUser(string userId);
        Task<ResultDTO<bool>> CheckUserBeforeDelete(string userId);
        Task<ResultDTO> DeleteRole(string roleId);
        
        Task<ResultDTO<UserManagementDTO>> FillUserManagementDto(UserManagementDTO? dto = null);
        Task<ResultDTO<RoleManagementDTO>> FillRoleManagementDto(RoleManagementDTO? dto = null);

        Task<ResultDTO<List<UserDTO>>> GetAllIntanetPortalUsers();
        Task<ResultDTO<List<RoleDTO>>> GetAllRoles();
        Task<ResultDTO<UserDTO>> GetUserById(string userId);
        Task<ResultDTO<UserDTO>> GetSuperAdminUserBySpecificClaim();
        Task<ResultDTO<RoleDTO>> GetRoleById(string roleId);
        Task<ResultDTO<List<RoleDTO>>> GetRolesForUser(string userId);
        Task<ResultDTO<List<RoleClaimDTO>>> GetRoleClaims(string roleId);
        Task<ResultDTO<List<UserClaimDTO>>> GetUserClaims(string userId);
        Task<ResultDTO<List<UserRoleDTO>>> GetAllUserRoles();
        Task<ResultDTO<string>> GetPreferredLanguageForUser(string userId);
    }
}