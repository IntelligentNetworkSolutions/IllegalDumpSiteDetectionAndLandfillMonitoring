using AutoMapper;
using DAL.Helpers;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DocumentFormat.OpenXml.Bibliography;
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
        public async Task<ResultDTO> AddUser(UserManagementDTO dto)
        {
            if (dto is null)
                return ResultDTO.Fail("UserManagementDTO Must Not be Null");

            try
            {
                dto.TrimProperies();

                ApplicationUser user = _mapper.Map<ApplicationUser>(dto);
                user.NormalizeUserNameAndEmail();

                ResultDTO<string> resGetPassHash = await GetPasswordHashForAppUser(user, dto.ConfirmPassword);
                if (resGetPassHash.IsSuccess == false)
                    return ResultDTO.Fail(resGetPassHash.ErrMsg);
                user.PasswordHash = resGetPassHash.Data;

               
                ApplicationUser insertedUser = await _userManagementDa.AddUser(user);

                // Add User Roles
                ResultDTO resAddUserRoles = await AddUserRoles(insertedUser.Id, dto.RolesInsert);
                if (resAddUserRoles.IsSuccess == false && ResultDTO.HandleError(resAddUserRoles))
                    return ResultDTO.Fail(resAddUserRoles.ErrMsg);

                // Add User Claims
                ResultDTO resAddUserClaims = await AddUserClaims(insertedUser.Id, dto.ClaimsInsert, "AuthorizationClaim");
                if (resAddUserClaims.IsSuccess == false && ResultDTO.HandleError(resAddUserClaims))
                    return ResultDTO.Fail(resAddUserClaims.ErrMsg);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> AddRole(RoleManagementDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    return ResultDTO.Fail("Role must have a name");

                dto.Name = dto.Name.Trim();
                dto.NormalizedName = dto.Name.ToUpper();

                var role = _mapper.Map<IdentityRole>(dto) ?? throw new Exception("Role not mapped");
                var addedRole = await _userManagementDa.AddRole(role) ?? throw new Exception("Role not added");

                foreach (var claim in dto.ClaimsInsert)
                {
                    var forInsert = _mapper.Map<IdentityRoleClaim<string>>(claim);
                    forInsert.RoleId = addedRole.Id;
                    await _userManagementDa.AddClaimForRole(forInsert);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> AddLanguageClaimForUser(string userId, string culture)
        {
            try
            {
                List<IdentityUserClaim<string>> listClaimsDb =
                    await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim");

                // Add Claim
                if (listClaimsDb is null || listClaimsDb.Count() == 0)
                {
                    var forInsert = _mapper.Map<IdentityUserClaim<string>>(culture);
                    forInsert.ClaimType = "PreferedLanguageClaim";
                    forInsert.UserId = userId;
                    await _userManagementDa.AddLanguageClaimForUser(forInsert);

                    return ResultDTO.Ok();
                }

                // Update Claim
                var claimDb = listClaimsDb.First();
                claimDb.ClaimValue = culture;
                await _userManagementDa.UpdateLanguageClaimForUser(claimDb);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        public async Task<(int passMinLenght, bool passHasLetters, bool passHasNumbers)> GetPasswordRequirements()
        {
            const int passMinLenDefault = 4;
            const bool passMustLettersDefault = false;
            const bool passMustNumbersDefault = false;

            ResultDTO<int> resPassMinLenght = 
                await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("PasswordMinLength", passMinLenDefault);
            ResultDTO<bool> resPassHasLetters = 
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", passMustLettersDefault);
            ResultDTO<bool> resPassHasNumbers = 
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", passMustNumbersDefault);

            return (resPassMinLenght.Data, resPassHasLetters.Data, resPassHasNumbers.Data);
        }

        public async Task<(bool hasMinLength, bool hasLetters, bool hasNumbers)> CheckPasswordRequirements(string password)
        {
            bool checkLength = false;
            bool checkLetters = false;
            bool checkNumbers = false;

            if (string.IsNullOrEmpty(password))
                return (checkLength, checkLetters, checkNumbers);

            (int passMinLenght, bool passHasLetters, bool passHasNumbers) = await GetPasswordRequirements();

            if (password.Length >= passMinLenght)
                checkLength = true;

            if(password.Any(c => Char.IsLetter(c)))
                checkLetters = true;

            if(password.Any(c => Char.IsNumber(c)))
                checkNumbers = true;

            return (checkLength, checkLetters, checkNumbers);
        }

        public async Task<ResultDTO<string>> GetPasswordHashForAppUser(ApplicationUser appUser, string passwordNew)
        {
            (bool hasMinLength, bool hasLetters, bool hasNumbers) = await CheckPasswordRequirements(passwordNew);
            if (hasMinLength == false || hasLetters == false || hasNumbers == false)
                return ResultDTO<string>.Fail("Password does not meet requirements");

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            string? hashedPasswordViewModel = passwordHasher.HashPassword(appUser, passwordNew);
            if (string.IsNullOrWhiteSpace(hashedPasswordViewModel))
                return ResultDTO<string>.Fail("Password Not Set");

            return ResultDTO<string>.Ok(hashedPasswordViewModel);
        }

        public async Task<ResultDTO> AddUserClaims(string appUserId, List<string> claimsInsert, string claimsType)
        {
            try
            {
                if (claimsInsert is null)
                    return ResultDTO.Fail("RolesInsert must not be null");

                if (claimsInsert.Count == 0)
                    return ResultDTO.Ok();

                foreach (var claim in claimsInsert)
                {
                    var userClaim = _mapper.Map<IdentityUserClaim<string>>(claim);
                    userClaim.UserId = appUserId;
                    userClaim.ClaimType = claimsType;
                    await _userManagementDa.AddClaimForUser(userClaim);
                }


                return await Task.FromResult(ResultDTO.Ok());
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> AddUserRoles(string appUserId, List<string> rolesInsert)
        {
            try
            {
                if (rolesInsert is null)
                    return ResultDTO.Fail("RolesInsert must not be null");

                if (rolesInsert.Count == 0  )
                    return ResultDTO.Ok();

                // Set user roles Ids
                foreach (var role in rolesInsert)
                {
                    var userRole = _mapper.Map<IdentityUserRole<string>>(role);
                    userRole.UserId = appUserId;
                    await _userManagementDa.AddRoleForUser(userRole);
                }

                return await Task.FromResult(ResultDTO.Ok());
            }
            catch(Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        #region Update
        public async Task<ResultDTO> UpdateUser(UserManagementDTO dto)
        {
            if (dto is null)
                return ResultDTO.Fail("UserManagementDTO Must Not be Null");

            try
            {
                dto.TrimProperies();

                ApplicationUser? userEntity = await _userManagementDa.GetUserById(dto.Id);
                if (userEntity is null)
                    return ResultDTO.Fail("User not found");

                userEntity = _mapper.Map(dto, userEntity);      // Map properties from dto to entity
                userEntity.NormalizeUserNameAndEmail();         // Normalize mapped props

                if (string.IsNullOrEmpty(dto.ConfirmPassword) == false)
                {
                    ResultDTO<string> resGetPass = await GetPasswordHashForAppUser(userEntity, dto.ConfirmPassword);
                    if (resGetPass.IsSuccess == false) 
                        return ResultDTO.Fail(resGetPass.ErrMsg!);

                    userEntity.PasswordHash = resGetPass.Data;      // Set Hashed Password
                }

                // Update User Properties
                bool resUpdateUser = await _userManagementDa.UpdateUser(userEntity);
                if (resUpdateUser == false)
                    return ResultDTO.Fail("User not updated");

                List<IdentityUserClaim<string>> userClaims = 
                    await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userEntity.Id, "AuthorizationClaim");

                List<IdentityUserRole<string>> userRoles =
                    await _userManagementDa.GetUserRolesByUserId(userEntity.Id);

                // Delete Existing User Roles and Claims
                await _userManagementDa.DeleteClaimsRolesForUser(userClaims, userRoles);

                // Add User Roles
                ResultDTO resAddUserRoles = await AddUserRoles(userEntity.Id, dto.RolesInsert);
                if (resAddUserRoles.IsSuccess == false && ResultDTO.HandleError(resAddUserRoles))
                    return ResultDTO.Fail(resAddUserRoles.ErrMsg);

                // Add User Claims
                ResultDTO resAddUserClaims = await AddUserClaims(userEntity.Id, dto.ClaimsInsert, "AuthorizationClaim");
                if (resAddUserClaims.IsSuccess == false && ResultDTO.HandleError(resAddUserClaims))
                    return ResultDTO.Fail(resAddUserClaims.ErrMsg);

                return ResultDTO.Ok();
            }
            catch(Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> UpdateUserPassword(string userId, string currentPassword, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(password))
                return ResultDTO.Fail("All fields are required");

            UserDTO? userDto = await GetUserById(userId);
            if (userDto is null)
                return ResultDTO.Fail("User Not Found");

            ApplicationUser appUser = _mapper.Map<ApplicationUser>(userDto);

            ResultDTO<string> resGetPassHash = await GetPasswordHashForAppUser(appUser, userDto.PasswordHash);
            if (resGetPassHash.IsSuccess == false)
                return ResultDTO.Fail(resGetPassHash.ErrMsg);

            appUser.PasswordHash = resGetPassHash.Data;

            bool resUpdate = await _userManagementDa.UpdateUser(appUser);
            if (!resUpdate)
                return ResultDTO.Fail("Failed while Updating User");

            return ResultDTO.Ok();
        }

        public async Task<ResultDTO> UpdateRole(RoleManagementDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    return ResultDTO.Fail("Role Must have a Name");

                IdentityRole? roleEntity = await _userManagementDa.GetRole(dto.Id);
                if (roleEntity is null)
                    return ResultDTO.Fail("Role not found");

                roleEntity.Name = dto.Name.Trim();
                roleEntity.NormalizedName = roleEntity.Name.ToUpper();

                bool resUpdate = await _userManagementDa.UpdateRole(roleEntity);
                if (resUpdate == false)
                    return ResultDTO.Fail("Role Not Updated");

                // Get Existing Claims
                List<IdentityRoleClaim<string>> roleClaims =
                    await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(roleEntity.Id, "AuthorizationClaim");
                // Delete Existing Claims
                await _userManagementDa.DeleteClaimsForRole(roleClaims);
                
                foreach (var claim in dto.ClaimsInsert)
                {
                    var roleClaim = _mapper.Map<IdentityRoleClaim<string>>(claim);
                    roleClaim.RoleId = roleEntity.Id;
                    roleClaim.ClaimType = "AuthorizationClaim";
                    await _userManagementDa.AddClaimForRole(roleClaim);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO<bool>> CheckUserBeforeDelete(string userId)
        {
            try
            {
                var hasUserEntry = _userManagementDa.CheckUserBeforeDelete(userId);
                if (hasUserEntry.HasValue)
                    return ResultDTO<bool>.Ok(hasUserEntry.Value);

                return ResultDTO<bool>.Fail("Error occured while retriving data");
            }
            catch (Exception ex)
            {
                return ResultDTO<bool>.ExceptionFail(ex.Message, ex);
            }
        }
        public async Task<ResultDTO> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManagementDa.GetUserById(userId);
                if (user is null)
                    return ResultDTO.Fail("User not found");

                await _userManagementDa.DeleteUser(user);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteRole(string roleId)
        {
            try
            {
                IdentityRole? role = await _userManagementDa.GetRole(roleId);
                if (role is null)
                    return ResultDTO.Fail("Role not found");

                await _userManagementDa.DeleteRole(role);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Read
        public async Task<ResultDTO<UserManagementDTO>> FillUserManagementDto(UserManagementDTO? dto = null)
        {
            try
            {
                if (dto is null)
                    dto = new UserManagementDTO();

                if (!string.IsNullOrEmpty(dto.Id))
                {
                    ApplicationUser? user = await _userManagementDa.GetUserById(dto.Id);
                    if (user is null)
                        return ResultDTO<UserManagementDTO>.Fail("User not found");

                    dto = _mapper.Map(user, dto);

                    List<IdentityUserRole<string>> userRoles = 
                        await _userManagementDa.GetUserRolesByUserId(user.Id);
                    dto.RolesInsert = userRoles.Select(x => x.RoleId).ToList();

                    List<IdentityUserClaim<string>> userAuthClaims = 
                        await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(user.Id, "AuthorizationClaim");
                    dto.ClaimsInsert = userAuthClaims.Select(z => z.ClaimValue).ToList();

                    List<ApplicationUser> allUsersExceptCurrentDb = 
                        await _userManagementDa.GetAllIntanetPortalUsersExcludingCurrent(user.Id);
                    dto.AllUsersExceptCurrent = _mapper.Map<List<UserDTO>>(allUsersExceptCurrentDb);
                }

                // TODO: Review -> Why all users, this is not okay
                List<ApplicationUser> allUsers = await _userManagementDa.GetAllIntanetPortalUsers();
                dto.AllUsers = _mapper.Map<List<UserDTO>>(allUsers);

                var roles = await _userManagementDa.GetAllRoles();
                dto.Roles = _mapper.Map<List<RoleDTO>>(roles);

                List<IdentityRoleClaim<string>> roleClaims = await _userManagementDa.GetAllRoleClaims();
                dto.RoleClaims = _mapper.Map<List<RoleClaimDTO>>(roleClaims);
                
                (int passMinLenght, bool passHasLetters, bool passHasNumbers) = 
                    await GetPasswordRequirements();

                dto.PasswordMinLength = passMinLenght;
                dto.PasswordMustHaveLetters = passHasLetters;
                dto.PasswordMustHaveNumbers = passHasNumbers;

                return ResultDTO<UserManagementDTO>.Ok(dto);
            }
            catch(Exception ex)
            {
                return ResultDTO<UserManagementDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<RoleManagementDTO>> FillRoleManagementDto(RoleManagementDTO? dto = null)
        {
            try
            {
                if (dto is null)
                    dto = new RoleManagementDTO();

                if (string.IsNullOrEmpty(dto.Id) == false)
                {
                    var role = await _userManagementDa.GetRole(dto.Id);
                    if (role is null)
                        return ResultDTO<RoleManagementDTO>.Fail("Role not found");

                    dto = _mapper.Map<RoleManagementDTO>(role);
                    var claims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role.Id, "AuthorizationClaim");
                    dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
                }

                return ResultDTO<RoleManagementDTO>.Ok(dto);
            }
            catch(Exception ex)
            {
                return ResultDTO<RoleManagementDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<RoleDTO?> GetRoleById(string roleId)
        {
            // TODO: Review Error Throwing
            var roleDb = await _userManagementDa.GetRole(roleId) ?? throw new Exception("Role not found");
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
            var userClaims =
                await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim")
                ?? throw new Exception("User claims not found");
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

        public async Task<UserDTO> GetSuperAdminUserBySpecificClaim()
        {
            var superAdmin = await _userManagementDa.GetUserBySpecificClaim();
            return _mapper.Map<UserDTO>(superAdmin) ?? new UserDTO();
        }
        public async Task<ICollection<UserDTO>> GetAllIntanetPortalUsers()
        {
            var allUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            return _mapper.Map<List<UserDTO>>(allUsers) ?? new List<UserDTO>();
        }

        public async Task<ICollection<RoleDTO>> GetAllRoles()
        {
            var allRoles = await _userManagementDa.GetAllRoles();
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
