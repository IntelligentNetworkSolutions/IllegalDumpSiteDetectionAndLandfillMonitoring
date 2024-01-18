using AutoMapper;
using DAL.Helpers;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using Microsoft.AspNetCore.Identity;
using SD;
using Services.Interfaces.Services;

namespace Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserManagementDa _userManagementDa;
        private readonly IMapper _mapper;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        public UserManagementService(IUserManagementDa userManagementDa, IAppSettingsAccessor appSettingsAccessor, IMapper mapper)
        {
            _userManagementDa = userManagementDa;
            _mapper = mapper;
            _appSettingsAccessor = appSettingsAccessor;
        }

        #region Create 
        public async Task AddUser(UserManagementDTO dto)
        {
            dto.UserName = dto.UserName?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.IsActive ??= false;
            dto.Password = dto.Password?.Trim();
            dto.ConfirmPassword = dto.ConfirmPassword?.Trim();

            ApplicationUser user = _mapper.Map<ApplicationUser>(dto);

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.NormalizedUserName = dto.UserName.ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.NormalizedEmail = dto.Email.ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                var password = new PasswordHasher<UserManagementDTO>();
                var hashed = password.HashPassword(dto, dto.ConfirmPassword);
                user.PasswordHash = hashed;
            }
            var insertedUser = await _userManagementDa.AddUser(user) ?? throw new Exception("User not found");

            var userRoles = _mapper.Map<IEnumerable<IdentityUserRole<string>>>(dto.RolesInsert);
            foreach (var userRole in userRoles)
            {
                userRole.UserId = insertedUser.Id;
                await _userManagementDa.AddRoleForUser(userRole);
            }

            var userClaims = _mapper.Map<List<IdentityUserClaim<string>>>(dto.ClaimsInsert);
            foreach (var claim in userClaims)
            {
                claim.UserId = insertedUser.Id;
                claim.ClaimType = "AuthorizationClaim";
                await _userManagementDa.AddClaimForUser(claim);
            }
        }

        public async Task AddRole(RoleManagementDTO dto)
        {
            if (dto.Name is not null)
            {
                dto.Name = dto.Name.Trim();
                dto.NormalizedName = dto.Name.ToUpper();
            }

            var role = _mapper.Map<IdentityRole>(dto) ?? throw new Exception("Role not found");
            var addedRole = await _userManagementDa.AddRole(role) ?? throw new Exception("Role not found");

            foreach (var claim in dto.ClaimsInsert)
            {
                var forInsert = _mapper.Map<IdentityRoleClaim<string>>(claim);
                forInsert.RoleId = addedRole.Id;
                await _userManagementDa.AddClaimForRole(forInsert);
            }
        }

        public async Task AddLanguageClaimForUser(string userId, string culture)
        {
            var listClaimsDb = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim");
            var claimDb = listClaimsDb.FirstOrDefault();
            if (claimDb is not null)
            {
                claimDb.ClaimValue = culture;
                await _userManagementDa.UpdateLanguageClaimForUser(claimDb);
            }
            else
            {
                var forInsert = _mapper.Map<IdentityUserClaim<string>>(culture);
                forInsert.ClaimType = "PreferedLanguageClaim";
                forInsert.UserId = userId;
                await _userManagementDa.AddLanguageClaimForUser(forInsert);
            }
        }
        #endregion

        #region Update
        public async Task UpdateUser(UserManagementDTO dto)
        {
            // TODO: Return ResultDTO
            dto.UserName = dto.UserName?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.Password = dto.Password?.Trim();
            dto.ConfirmPassword = dto.ConfirmPassword?.Trim();

            var findUser = await _userManagementDa.GetUserById(dto.Id) ?? throw new Exception("User not found");
            _mapper.Map(dto, findUser);

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                findUser.NormalizedUserName = dto.UserName.ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.Email))
                findUser.NormalizedEmail = dto.Email.ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                var password = new PasswordHasher<UserManagementDTO>();
                var hashed = password.HashPassword(dto, dto.ConfirmPassword);
                findUser.PasswordHash = hashed;
            }

            bool resUpdateUser = await _userManagementDa.UpdateUser(findUser);
            ApplicationUser updatedUser = resUpdateUser ? findUser : throw new Exception("User not found");
            var userClaims =
                await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(updatedUser.Id, "AuthorizationClaim") ?? throw new Exception("User claims found");
            var userRoles =
                await _userManagementDa.GetUserRolesByUserId(updatedUser.Id) ?? throw new Exception("User roles found");
            await _userManagementDa.DeleteClaimsRolesForUser(userClaims, userRoles);

            foreach (var role in dto.RolesInsert)
            {
                var userRole = _mapper.Map<IdentityUserRole<string>>(role);
                userRole.UserId = updatedUser.Id;
                await _userManagementDa.AddRoleForUser(userRole);
            }

            foreach (var claim in dto.ClaimsInsert)
            {
                var forInsert = _mapper.Map<IdentityUserClaim<string>>(claim);
                forInsert.ClaimType = "AuthorizationClaim";
                forInsert.UserId = updatedUser.Id;
                await _userManagementDa.AddClaimForUser(forInsert);
            }
        }

        public async Task<ResultDTO> UpdateUserPassword(string userId, string currentPassword, string password)
        {
            UserDTO? userDto = await GetUserById(userId);
            if (userDto is null)
                return ResultDTO.Fail("User Not Found");

            // TODO: Check Pass Requirements Server-Side
            //(int passMinLength, bool passMustLetters, bool passMustNumbers) =
            //    await GetBasicPasswordRequirements(10, true, true);

            var passwordHasher = new PasswordHasher<UserDTO>();
            string? hashedPasswordViewModel = passwordHasher.HashPassword(userDto, password);
            PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(userDto, userDto.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
                return ResultDTO.Fail("Password Verification Failed");
            if (string.IsNullOrWhiteSpace(userDto.PasswordHash))
                return ResultDTO.Fail("Password Not Set");

            userDto.PasswordHash = hashedPasswordViewModel;

            ApplicationUser appUser = _mapper.Map<ApplicationUser>(userDto);

            bool resUpdate = await _userManagementDa.UpdateUser(appUser);
            if (!resUpdate)
                return ResultDTO.Fail("Failed while Updating User");

            return ResultDTO.Ok();
        }

        public async Task UpdateRole(RoleManagementDTO dto)
        {
            // TODO: Return ResultDTO
            var findRole = await _userManagementDa.GetRole(dto.Id) ?? throw new Exception("Role not found");
            if (dto.Name is not null)
                dto.Name = dto.Name.Trim();

            findRole.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(findRole.Name))
                findRole.NormalizedName = findRole.Name.ToUpper();

            bool resUpdate = await _userManagementDa.UpdateRole(findRole);
            var updatedRole = resUpdate ? findRole : throw new Exception("Role not found");

            var roleClaims = await _userManagementDa.GetAllRoleClaims();
            roleClaims = roleClaims.Where(x => x.RoleId == updatedRole.Id && x.ClaimType == "AuthorizationClaim").ToList();
            if (roleClaims is null)
                throw new Exception("Role claims not found");

            await _userManagementDa.DeleteClaimsForRole(roleClaims);

            foreach (var claim in dto.ClaimsInsert)
            {
                var forInsert = _mapper.Map<IdentityRoleClaim<string>>(claim);
                forInsert.RoleId = updatedRole.Id;
                await _userManagementDa.AddClaimForRole(forInsert);
            }
        }
        #endregion

        #region Delete
        public async Task DeleteUser(string userId)
        {
            var user = await _userManagementDa.GetUserById(userId) ?? throw new Exception("User not found");
            await _userManagementDa.DeleteUser(user);
        }

        public async Task DeleteRole(string roleId)
        {
            var role = await _userManagementDa.GetRole(roleId) ?? throw new Exception("Role not found");
            await _userManagementDa.DeleteRole(role);
        }
        #endregion

        #region Read
        public IQueryable<IdentityRole> GetRolesAsQueriable()
        {
            return _userManagementDa.GetRolesAsQueriable();
        }

        public async Task<UserManagementDTO> FillUserManagementDto(UserManagementDTO dto)
        {
            var roles = await _userManagementDa.GetRoles() ?? throw new Exception("Roles not found");
            dto ??= new UserManagementDTO();

            if (!string.IsNullOrEmpty(dto.Id))
            {
                var user = await _userManagementDa.GetUserById(dto.Id);
                var userRolesDbTable = await _userManagementDa.GetUserRoles();
                var insertedUserRoles = userRolesDbTable.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
                var claims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(user.Id, "AuthorizationClaim");
                var allUsersExceptCurrentDb = await _userManagementDa.GetAllIntanetPortalUsersExcludingCurrent(user.Id);

                _mapper.Map(user, dto);

                dto.RolesInsert = roles.Where(z => insertedUserRoles.Contains(z.Id)).Select(z => z.Id).ToList();
                dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
                dto.AllUsersExceptCurrent = _mapper.Map<List<UserDTO>>(allUsersExceptCurrentDb);                
            }

            var allUsers = await _userManagementDa.GetAllIntanetPortalUsers() ?? throw new Exception("Users not found");
            var roleClaims = await _userManagementDa.GetAllRoleClaims() ?? throw new Exception("Role claims not found");
            
            dto.RoleClaims = _mapper.Map<List<RoleClaimDTO>>(roleClaims);           
            dto.AllUsers = _mapper.Map<List<UserDTO>>(allUsers);

            ResultDTO<int> resPassMinLenght = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("PasswordMinLength", 4);
            ResultDTO<bool> resPassHasLetters = await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", false);
            ResultDTO<bool> resPassHasNumbers = await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", false);

            // TODO: get pass settings
            dto.PasswordMinLength = resPassMinLenght.IsSuccess ? resPassMinLenght.Data : 4;
            dto.PasswordMustHaveLetters = resPassHasLetters.IsSuccess ? resPassHasLetters.Data : false;
            dto.PasswordMustHaveNumbers = resPassHasNumbers.IsSuccess ? resPassHasNumbers.Data : false;
            dto.Roles = _mapper.Map<List<RoleDTO>>(roles);

            return dto;
        }

        public async Task<RoleManagementDTO> FillRoleManagementDto(RoleManagementDTO dto)
        {
            dto ??= new RoleManagementDTO();

            if (!string.IsNullOrEmpty(dto.Id))
            {
                var role = await _userManagementDa.GetRole(dto.Id);
                dto = _mapper.Map<RoleManagementDTO>(role);             
                var claims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role.Id, "AuthorizationClaim");
                dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
            }

            return dto;            
        }

        public async Task<RoleDTO?> GetRoleById(string roleId)
        {
            // TODO: Review Error Throwing
            var roleDb =  await _userManagementDa.GetRole(roleId) ?? throw new Exception("Role not found");
            var dto = _mapper.Map<RoleDTO>(roleDb) ?? throw new Exception("Role DTO not found");
            return dto;
        }
        
        public async Task<UserDTO?> GetUserById(string userId)
        {
            // TODO: Review Error Throwing
            var userDb = await _userManagementDa.GetUserById(userId) ?? throw new Exception("User not found");            
            var dto = _mapper.Map<UserDTO>(userDb) ?? throw new Exception("User dto not found");
            return dto;
        }

        public async Task<List<RoleClaimDTO>> GetRoleClaims(string roleId)
        {
            var roleClaims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim") ?? throw new Exception("Role claims not found");
            var roleClaimDTOs = _mapper.Map<List<RoleClaimDTO>>(roleClaims);
            return roleClaimDTOs;
        }
        public async Task<List<UserClaimDTO>> GetUserClaims(string userId)
        {
            var userClaims=  await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim") ?? throw new Exception("User claims not found");
            var userClaimDTOs = _mapper.Map<List<UserClaimDTO>>(userClaims);
            return userClaimDTOs;
        }

        public async Task<List<RoleDTO>> GetRolesForUser(string userId)
        {
            var roleIdsList = _userManagementDa.GetUserRolesByUserId(userId).GetAwaiter().GetResult().Select(x => x.RoleId).ToList() ?? throw new Exception("Role ids not found");
            var userRoles = await _userManagementDa.GetRolesForUser(roleIdsList) ?? throw new Exception("User roles not found");
            var userRolesDTOs = _mapper.Map<List<RoleDTO>>(userRoles);
            return userRolesDTOs;           
        }    

        public async Task<ICollection<UserDTO>> GetAllIntanetPortalUsers()
        {
            var allUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            return _mapper.Map<List<UserDTO>>(allUsers) ?? new List<UserDTO>();          
        }

        public async Task<ICollection<RoleDTO>> GetAllRoles()
        {
            var allRoles = await _userManagementDa.GetRoles();
            return _mapper.Map<List<RoleDTO>>(allRoles) ?? new List<RoleDTO>();
        }

        public async Task<ICollection<UserRoleDTO>> GetAllUserRoles()
        {
            var allUserRoles = await _userManagementDa.GetUserRoles();
            return _mapper.Map<List<UserRoleDTO>>(allUserRoles) ?? new List<UserRoleDTO>();
        }

        public async Task<List<ClaimDTO>> GetClaimsForUser(string userId)
        {
            var roleIdsList = _userManagementDa.GetUserRolesByUserId(userId).GetAwaiter().GetResult().Select(x => x.RoleId).ToList() ?? throw new Exception("Role ids not found");
            var userRolesDb = await _userManagementDa.GetRolesForUser(roleIdsList) ?? throw new Exception("User roles found");

            var userRoles = userRolesDb.Select(x => x.Id).ToList();          
            var roleClaims = new List<ClaimDTO>();

            var allUserClaims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim");
            var userClaims = allUserClaims.Select(x => new ClaimDTO() { ClaimType = x.ClaimType, ClaimValue = x.ClaimValue }).ToList();
            foreach (var role in userRoles)
            {                
                var claimForRoleDb = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role, "AuthorizationClaim") ?? throw new Exception("Claims for role not found");
                var claimsForRole = claimForRoleDb.Select(x => new ClaimDTO() { ClaimValue = x.ClaimValue, ClaimType = x.ClaimType }).ToList();
                roleClaims.AddRange(claimsForRole);
            }

            List<ClaimDTO> claimsForAdd = new List<ClaimDTO>();
            claimsForAdd.AddRange(roleClaims);
            claimsForAdd.AddRange(userClaims);
            var distinctedClaimsForAdd = claimsForAdd.Distinct().ToList();

            return distinctedClaimsForAdd;
        }

        public async Task<string?> GetPreferredLanguageForUser(string userId)
        {
            return await _userManagementDa.GetPreferredLanguage(userId);
        }
        #endregion

    }
}
