using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dal;
using Dal.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DTOs.MainApp.MVC;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Interfaces.Services;

namespace Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManagementDa _userManagementDa;
        private readonly ApplicationSettingsHelper _applicationSettingsHelper;

        public UserManagementService(UserManagementDa userManagementDa, ApplicationSettingsHelper applicationSettingsHelper)
        {
            _userManagementDa = userManagementDa;
            _applicationSettingsHelper = applicationSettingsHelper;
        }

        public IQueryable<IdentityRole> GetRolesAsQueriable()
        {
            return _userManagementDa.GetRolesAsQueriable();
        }

        public async Task<UserManagementDTO> FillUserManagementDto(UserManagementDTO dto)
        {
            var roles = await _userManagementDa.GetRoles();
            if(roles is null)
            {
                throw new Exception("Roles not found");
            }
            
            dto ??= new UserManagementDTO();

            if (!string.IsNullOrEmpty(dto.Id))
            {
                var user = await _userManagementDa.GetUserById(dto.Id);
                var userRolesDbTable = await _userManagementDa.GetUserRoles();
                var insertedUserRoles = userRolesDbTable.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
                var claims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(user.Id, "AuthorizationClaim");
                var allUsersExceptCurrentDb = await _userManagementDa.GetAllIntanetPortalUsersExcludingCurrent(user.Id);
                
                dto.Id = user.Id;
                dto.FirstName = user.FirstName;
                dto.LastName = user.LastName;
                dto.UserName = user.UserName;
                dto.PhoneNumber = user.PhoneNumber;
                dto.Email = user.Email;
                dto.IsActive = user.IsActive;
                dto.RolesInsert = roles.Where(z => insertedUserRoles.Contains(z.Id)).Select(z => z.Id).ToList();
                dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
                dto.AllUsersExceptCurrent = allUsersExceptCurrentDb.Select(z => new UserDTO
                {
                    FirstName = z.FirstName,
                    LastName = z.LastName,
                    Id = z.Id,
                    UserName = z.UserName,
                    PhoneNumber = z.PhoneNumber,
                    Email = z.Email,
                    IsActive = z.IsActive,
                    NormalizedUserName= z.NormalizedUserName,
                }).ToList();
            }

            var allUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            if (allUsers is null)
            {
                throw new Exception("Users not found");
            }
            var roleClaims = await _userManagementDa.GetAllRoleClaims();
            if (roleClaims is null)
            {
                throw new Exception("Role claims not found");
            }

            dto.RoleClaims = roleClaims.Select(z => new RoleClaimDTO
            {
              Id= z.Id,
              ClaimType= z.ClaimType,
              ClaimValue= z.ClaimValue,
              RoleId = z.RoleId
            }).ToList();

            dto.AllUsers = allUsers.Select(z => new UserDTO
            {
                FirstName = z.FirstName,
                LastName = z.LastName,
                Id = z.Id,
                UserName = z.UserName,
                PhoneNumber = z.PhoneNumber,
                Email = z.Email,
                IsActive = z.IsActive,
                NormalizedUserName = z.NormalizedUserName,
            }).ToList();
            dto.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength", "4");
            dto.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters", "false");
            dto.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers", "false");
            dto.Roles = roles.Select(z => new RoleDTO
            {
                Id = z.Id,
                Name= z.Name,
                NormalizedName = z.NormalizedName
            }).ToList();

            return dto;
          
        }

        public async Task UpdateUser(UserManagementDTO dto)
        {
            var findUser = await _userManagementDa.GetUserById(dto.Id);
            if (findUser is null)
            {
                throw new Exception("User not found");
            }            
                
            dto.UserName = dto.UserName?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.Password = dto.Password?.Trim();
            dto.ConfirmPassword = dto.ConfirmPassword?.Trim();
            findUser.FirstName = dto.FirstName;
            findUser.LastName = dto.LastName;
            findUser.UserName = dto.UserName;
            findUser.PhoneNumber = dto.PhoneNumber;
            findUser.Id = dto.Id;
            findUser.Email = dto.Email;
            findUser.IsActive = dto.IsActive ?? false;
            if (dto.UserName != " " && dto.UserName is not null)
            {
                findUser.NormalizedUserName = dto.UserName.ToUpper();
            }
            if (dto.Email != " " && dto.Email is not null)
            {
                findUser.NormalizedEmail = dto.Email.ToUpper();
            }
            if (dto.ConfirmPassword != " " && dto.ConfirmPassword is not null)
            {
                var password = new PasswordHasher<UserManagementDTO>();
                var hashed = password.HashPassword(dto, dto.ConfirmPassword);
                findUser.PasswordHash = hashed;
            }
            var updatedUser = await _userManagementDa.UpdateUser(findUser);
            if(updatedUser is null)
            {
                throw new Exception("User not found");
            }
            
            var userClaims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(updatedUser.Id, "AuthorizationClaim");
            if (userClaims is null)
            {
                throw new Exception("User claims found");
            }
            var userRoles = await _userManagementDa.GetUserRolesByUserId(updatedUser.Id);
            if (userRoles is null)
            {
                throw new Exception("User roles found");
            }
            await _userManagementDa.DeleteClaimsRolesForUser(userClaims,userRoles);

            foreach (var role in dto.RolesInsert)
            {
                IdentityUserRole<string> userRole = new IdentityUserRole<string>();
                userRole.UserId = updatedUser.Id;
                userRole.RoleId = role;
                await _userManagementDa.AddRoleForUser(userRole);
            }

            foreach (var claim in dto.ClaimsInsert)
            {
                IdentityUserClaim<string> forInsert = new IdentityUserClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.UserId = updatedUser.Id;
                await _userManagementDa.AddClaimForUser(forInsert);
            }
        }

        public async Task AddUser(UserManagementDTO dto)
        {
            dto.UserName = dto.UserName?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.IsActive ??= false;
            dto.Password = dto.Password?.Trim();
            dto.ConfirmPassword = dto.ConfirmPassword?.Trim();

            var password = new PasswordHasher<UserManagementDTO>();
            var hashed = password.HashPassword(dto, dto.Password);
            dto.PasswordHash = hashed;

            ApplicationUser user = new()
            {
                UserName = dto.UserName,
                Email = dto.Email,
                IsActive = dto.IsActive,
                PasswordHash = dto.PasswordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                NormalizedUserName = dto.UserName.ToUpper(),
                NormalizedEmail = dto.Email.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = dto.PhoneNumber
            };

            var insertedUser = await _userManagementDa.AddUser(user);
            if(insertedUser is null)
            {
                throw new Exception("User not found");
            }

            foreach (var role in dto.RolesInsert)
            {
                IdentityUserRole<string> userRole = new IdentityUserRole<string>();
                userRole.UserId = insertedUser.Id;
                userRole.RoleId = role;
                await _userManagementDa.AddRoleForUser(userRole);
            }
            foreach (var claim in dto.ClaimsInsert)
            {
                IdentityUserClaim<string> forInsert = new IdentityUserClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.UserId = insertedUser.Id;
                await _userManagementDa.AddClaimForUser(forInsert);
            }
        }

        public async Task<RoleManagementDTO> FillRoleManagementDto(RoleManagementDTO dto)
        {
            if (dto is null)
            {
                dto = new RoleManagementDTO();
            }

            if (!string.IsNullOrEmpty(dto.Id))
            {
                var role = await _userManagementDa.GetRole(dto.Id);
                dto.Id = role.Id;
                dto.NormalizedName = role.NormalizedName;
                dto.Name = role.Name;                
                var claims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role.Id, "AuthorizationClaim");
                dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
            }

            return dto;            
        }

        public async Task UpdateRole(RoleManagementDTO dto)
        {
            var findRole = await _userManagementDa.GetRole(dto.Id);
            if(findRole is null)
            {
                throw new Exception("Role not found");
            }
            if (dto.Name is not null)
            {
                dto.Name = dto.Name.Trim();
            }
            
            findRole.Name = dto.Name;
            if (findRole.Name != " " && findRole.Name is not null)
            {
                findRole.NormalizedName = findRole.Name.ToUpper();
            }
            var updatedRole = await _userManagementDa.UpdateRole(findRole);
            if (updatedRole is null)
            {
                throw new Exception("Role not found");
            }
            
            var roleClaims = await _userManagementDa.GetAllRoleClaims();
            roleClaims = roleClaims.Where(x => x.RoleId == updatedRole.Id && x.ClaimType == "AuthorizationClaim").ToList();
            if (roleClaims is null)
            {
                throw new Exception("Role claims not found");
            }
            await _userManagementDa.DeleteClaimsForRole(roleClaims);

            foreach (var claim in dto.ClaimsInsert)
            {
                IdentityRoleClaim<string> forInsert = new IdentityRoleClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.RoleId = updatedRole.Id;
                await _userManagementDa.AddClaimForRole(forInsert);
            }
        }

        public async Task AddRole(RoleManagementDTO dto)
        {
            if (dto.Name is not null)
            {
                dto.Name = dto.Name.Trim();
                dto.NormalizedName = dto.Name.ToUpper();
            }

            IdentityRole role = new()
            {
                Name = dto.Name,
                NormalizedName = dto.NormalizedName,
            };

            var addedRole = await _userManagementDa.AddRole(role);
            if (addedRole is null)
            {
                throw new Exception("Role not found");
            }
            foreach (var claim in dto.ClaimsInsert)
            {
                IdentityRoleClaim<string> forInsert = new IdentityRoleClaim<string>();
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.ClaimValue = claim;
                forInsert.RoleId = addedRole.Id;
                await _userManagementDa.AddClaimForRole(forInsert);
            }
        }

        public async Task<RoleDTO> GetRoleById(string roleId)
        {
            var roleDb =  await _userManagementDa.GetRole(roleId);
            if (roleDb is null)
            {
                throw new Exception("Role not found");
            }
            RoleDTO dto = new()
            {
                Id = roleId,
                Name = roleDb.Name,
                NormalizedName = roleDb.NormalizedName,
            };
            return dto;
        }
        public async Task DeleteRole(string roleId)
        {
            var role = await _userManagementDa.GetRole(roleId);
            if(role is null)
            {
                throw new Exception("Role not found");
            }
            await _userManagementDa.DeleteRole(role);
        }
        
        public async Task<UserDTO> GetUserById(string userId)
        {
            var userDb = await _userManagementDa.GetUserById(userId);
            if(userDb is null)
            {
                throw new Exception("User not found");
            }
            UserDTO dto = new()
            {
                Id = userDb.Id,
                Email = userDb.Email,
                UserName = userDb.UserName,
                PhoneNumber = userDb.PhoneNumber,
                FirstName= userDb.FirstName,
                LastName= userDb.LastName,
                IsActive= userDb.IsActive
            };
            return dto;
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userManagementDa.GetUserById(userId);
            if(user is null)
            {
                throw new Exception("User not found");
            }
            await _userManagementDa.DeleteUser(user);
        }

        public async Task<List<RoleClaimDTO>> GetRoleClaims(string roleId)
        {
            var roleClaims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim");
            if(roleClaims is null)
            {
                throw new Exception("Role claims not found");
            }
            return roleClaims.Select(z => new RoleClaimDTO
            {
                Id = z.Id,
                ClaimType = z.ClaimType,
                ClaimValue = z.ClaimValue,
                RoleId = z.RoleId

            }).ToList();
        }
        public async Task<List<UserClaimDTO>> GetUserClaims(string userId)
        {
            var userClaims=  await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim");
            if (userClaims is null)
            {
                throw new Exception("User claims not found");
            }
            return userClaims.Select(z => new UserClaimDTO
            {
                Id = z.Id,
                ClaimType = z.ClaimType,
                ClaimValue = z.ClaimValue,
                UserId = z.UserId

            }).ToList();
        }

        public async Task<List<RoleDTO>> GetRolesForUser(string userId)
        {
            var roleIdsList = _userManagementDa.GetUserRolesByUserId(userId).GetAwaiter().GetResult().Select(x => x.RoleId).ToList();
            if (roleIdsList is null)
            {
                throw new Exception("Role ids not found");
            }

            var userRoles = await _userManagementDa.GetRolesForUser(roleIdsList);              
            if (userRoles is null)
            {
                throw new Exception("User roles not found");
            }

            return userRoles.Select(z => new RoleDTO
            {
                Id = z.Id,
                Name = z.Name,
                NormalizedName = z.NormalizedName

            }).ToList();
        }    

        public async Task<ICollection<UserDTO>> GetAllIntanetPortalUsers()
        {
            var allUsers = await _userManagementDa.GetAllIntanetPortalUsers();

            if(allUsers is null)
            {
                return new List<UserDTO>();
            }

            return allUsers.Select(z => new UserDTO
            {
                FirstName = z.FirstName,
                LastName = z.LastName,
                Id = z.Id,
                UserName = z.UserName,
                PhoneNumber = z.PhoneNumber,
                Email = z.Email,
                IsActive = z.IsActive
            }).ToList();
        }

        public async Task<ICollection<RoleDTO>> GetAllRoles()
        {
            var allRoles = await _userManagementDa.GetRoles();

            if (allRoles is null)
            {
                return new List<RoleDTO>();
            }

            return allRoles.Select(z => new RoleDTO
            {
                Id= z.Id,
                Name= z.Name,
                NormalizedName = z.NormalizedName
                
            }).ToList();
        }

        public async Task<ICollection<UserRoleDTO>> GetAllUserRoles()
        {
            var allUserRoles = await _userManagementDa.GetUserRoles();
            
            if (allUserRoles is null)
            {
                return new List<UserRoleDTO>();
            }

            return allUserRoles.Select(z => new UserRoleDTO
            {
                RoleId = z.RoleId,
                UserId = z.UserId

            }).ToList();
        }

        public async Task<List<ClaimDTO>> GetClaimsForUser(string userId)
        {
            var roleIdsList = _userManagementDa.GetUserRolesByUserId(userId).GetAwaiter().GetResult().Select(x => x.RoleId).ToList();
            if (roleIdsList is null)
            {
                throw new Exception("Role ids not found");
            }
            var userRolesDb = await _userManagementDa.GetRolesForUser(roleIdsList);
            if(userRolesDb is null)
            {
                throw new Exception("User roles found");
            }
            var userRoles = userRolesDb.Select(x => x.Id).ToList();
          
            var roleClaims = new List<ClaimDTO>();

            var allUserClaims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim");
            var userClaims = allUserClaims.Select(x => new ClaimDTO() { ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToList();
            foreach (var role in userRoles)
            {                
                var claimForRoleDb = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role, "AuthorizationClaim");
                if (claimForRoleDb is null)
                {
                    throw new Exception("Claims for role not found");
                }
                var claimsForRole = claimForRoleDb.Select(x => new ClaimDTO() { ClaimValue = x.ClaimValue, ClaimType = x.ClaimType }).ToList();
                roleClaims.AddRange(claimsForRole);
            }

            List<ClaimDTO> claimsForAdd = new List<ClaimDTO>();
            claimsForAdd.AddRange(roleClaims);
            claimsForAdd.AddRange(userClaims);
            var distinctedClaimsForAdd = claimsForAdd.Distinct().ToList();

            return distinctedClaimsForAdd;
        }

        public async Task AddLanguageClaimForUser(string userId, string culture)
        {
            var listClaimsDb = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim");
            var claimDb = listClaimsDb.FirstOrDefault();
            if(claimDb is not null)
            {
                claimDb.ClaimValue = culture;
                await _userManagementDa.UpdateLanguageClaimForUser(claimDb);
            }
            else
            {
                IdentityUserClaim<string> forInsert = new IdentityUserClaim<string>();
                forInsert.ClaimType = "PreferedLanguageClaim";
                forInsert.ClaimValue = culture;
                forInsert.UserId = userId;
                await _userManagementDa.AddLanguageClaimForUser(forInsert);
            }
        }
    }
}
