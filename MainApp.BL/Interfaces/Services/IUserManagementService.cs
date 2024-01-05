using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.MVC;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Services.Interfaces.Services
{
    public interface IUserManagementService
    {
        IQueryable<IdentityRole> GetRolesAsQueriable();
        Task UpdateUser(UserManagementDTO dto);
        Task AddUser(UserManagementDTO dto);
        Task<UserManagementDTO> FillUserManagementDto(UserManagementDTO dto);
        Task<RoleManagementDTO> FillRoleManagementDto(RoleManagementDTO dto);
        Task UpdateRole(RoleManagementDTO dto);
        Task AddRole(RoleManagementDTO dto);
        Task<RoleDTO> GetRoleById(string roleId);
        Task DeleteRole(string roleId);
        Task<UserDTO> GetUserById(string userId);
        Task DeleteUser(string userId);
        Task<List<RoleClaimDTO>> GetRoleClaims(string roleId);
        Task<List<UserClaimDTO>> GetUserClaims(string userId);
        Task<List<RoleDTO>> GetRolesForUser(string userId);
        Task<ICollection<UserDTO>> GetAllIntanetPortalUsers();
        Task<ICollection<RoleDTO>> GetAllRoles();
        Task<ICollection<UserRoleDTO>> GetAllUserRoles();
        Task<List<ClaimDTO>> GetClaimsForUser(string userId);
        Task AddLanguageClaimForUser(string userId, string culture);
    }
}
