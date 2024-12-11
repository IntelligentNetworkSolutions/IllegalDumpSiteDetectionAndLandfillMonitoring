using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using Microsoft.AspNetCore.Identity;
using SD;
using Services.Interfaces.Services;
using System.Collections.Generic;

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

                ApplicationUser? user = _mapper.Map<ApplicationUser>(dto);
                if (user == null)
                    return ResultDTO.Fail("Mapping failed");

                user.NormalizeUserNameAndEmail();

                ResultDTO<string> resGetPassHash = await GetPasswordHashForAppUser(user, dto.ConfirmPassword);
                if (resGetPassHash.IsSuccess == false && resGetPassHash.HandleError())
                    return ResultDTO.Fail(resGetPassHash.ErrMsg!);
                if (resGetPassHash.Data == null)
                    return ResultDTO.Fail("Password hash not found");

                user.PasswordHash = resGetPassHash.Data;

                ApplicationUser? insertedUser = await _userManagementDa.AddUser(user);
                if (insertedUser == null)
                    return ResultDTO.Fail("Inserted user not found");

                // Add User Roles
                ResultDTO resAddUserRoles = await AddUserRoles(insertedUser.Id, dto.RolesInsert);
                if (resAddUserRoles.IsSuccess == false && resAddUserRoles.HandleError())
                    return ResultDTO.Fail(resAddUserRoles.ErrMsg!);

                // Add User Claims
                ResultDTO resAddUserClaims = await AddUserClaims(insertedUser.Id, dto.ClaimsInsert, "AuthorizationClaim");
                if (resAddUserClaims.IsSuccess == false && resAddUserClaims.HandleError())
                    return ResultDTO.Fail(resAddUserClaims.ErrMsg!);

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

                IdentityRole? role = _mapper.Map<IdentityRole>(dto);
                if(role == null)
                    return ResultDTO.Fail("Role mapping failed");

                ResultDTO<IdentityRole>? resultAddedRole = await _userManagementDa.AddRole(role!);
                if (resultAddedRole.IsSuccess == false && resultAddedRole.HandleError())
                    return ResultDTO.Fail(resultAddedRole.ErrMsg!);
                if (resultAddedRole.Data == null)
                    return ResultDTO.Fail("Created role not found");

                foreach (var claim in dto.ClaimsInsert)
                {
                    IdentityRoleClaim<string>? forInsert = _mapper.Map<IdentityRoleClaim<string>>(claim);
                    if (forInsert == null)
                        return ResultDTO.Fail("Role mapping failed");

                    forInsert.RoleId = resultAddedRole.Data.Id;
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
                    IdentityUserClaim<string>? forInsert = _mapper.Map<IdentityUserClaim<string>>(culture);
                    if (forInsert == null)
                        return ResultDTO.Fail("User claim mapping failed");

                    forInsert.ClaimType = "PreferedLanguageClaim";
                    forInsert.UserId = userId;
                    await _userManagementDa.AddLanguageClaimForUser(forInsert);

                    return ResultDTO.Ok();
                }

                // Update Claim
                IdentityUserClaim<string>? claimDb = listClaimsDb.First();
                if (claimDb == null)
                    return ResultDTO.Fail("User claim mapping failed");

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

            if (password.Any(c => Char.IsLetter(c)))
                checkLetters = true;

            if (password.Any(c => Char.IsNumber(c)))
                checkNumbers = true;

            return (checkLength, checkLetters, checkNumbers);
        }

        public async Task<ResultDTO<string>> GetPasswordHashForAppUser(ApplicationUser appUser, string passwordNew)
        {
            (bool hasMinLength, bool hasLetters, bool hasNumbers) = await CheckPasswordRequirements(passwordNew);
            if (hasMinLength == false || hasLetters == false || hasNumbers == false)
                return ResultDTO<string>.Fail("Password does not meet requirements");

            PasswordHasher<ApplicationUser>? passwordHasher = new PasswordHasher<ApplicationUser>();

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
                    return ResultDTO.Fail("ClaimsInsert must not be null");

                if (claimsInsert.Count == 0)
                    return ResultDTO.Ok();

                List<IdentityUserClaim<string>> userClaims = new List<IdentityUserClaim<string>>();

                foreach (var claim in claimsInsert)
                {
                    IdentityUserClaim<string> userClaim = new IdentityUserClaim<string>
                    {
                        UserId = appUserId,
                        ClaimType = claimsType,
                        ClaimValue = claim
                    };
                    userClaims.Add(userClaim);
                }

                // Call the new method to add claims in bulk
                await _userManagementDa.AddClaimsForUserRange(userClaims);

                return ResultDTO.Ok();
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

                if (rolesInsert.Count == 0)
                    return ResultDTO.Ok();

                List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

                foreach (var role in rolesInsert)
                {
                    IdentityUserRole<string> userRole = new IdentityUserRole<string>
                    {
                        RoleId = role,
                        UserId = appUserId
                    };
                    userRoles.Add(userRole);
                }

                await _userManagementDa.AddRolesForUserRange(userRoles);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
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

                userEntity = _mapper.Map(dto, userEntity); 
                userEntity.NormalizeUserNameAndEmail(); 

                if (string.IsNullOrEmpty(dto.ConfirmPassword) == false)
                {
                    ResultDTO<string> resGetPass = await GetPasswordHashForAppUser(userEntity, dto.ConfirmPassword);
                    if (resGetPass.IsSuccess == false && resGetPass.HandleError())
                        return ResultDTO.Fail(resGetPass.ErrMsg!);
                    if (resGetPass.Data == null)
                        return ResultDTO.Fail("Password hash not found");

                    userEntity.PasswordHash = resGetPass.Data; 
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
                if (resAddUserRoles.IsSuccess == false && resAddUserRoles.HandleError())
                    return ResultDTO.Fail(resAddUserRoles.ErrMsg!);

                // Add User Claims
                ResultDTO resAddUserClaims = await AddUserClaims(userEntity.Id, dto.ClaimsInsert, "AuthorizationClaim");
                if (resAddUserClaims.IsSuccess == false && resAddUserClaims.HandleError())
                    return ResultDTO.Fail(resAddUserClaims.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> UpdateUserPassword(string userId, string currentPassword, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(password))
                return ResultDTO.Fail("All fields are required");

            ResultDTO<UserDTO>? resultGetUser = await GetUserById(userId);
            if(resultGetUser.IsSuccess == false && resultGetUser.HandleError())
                return ResultDTO.Fail(resultGetUser.ErrMsg!);
            if (resultGetUser.Data == null)
                return ResultDTO.Fail("User Not Found");

            ApplicationUser? appUser = _mapper.Map<ApplicationUser>(resultGetUser.Data);
            if (appUser == null)
                return ResultDTO.Fail("Mapping for user falied");

            ResultDTO<string> resGetPassHash = await GetPasswordHashForAppUser(appUser, resultGetUser.Data.PasswordHash!);
            if (resGetPassHash.IsSuccess == false && resGetPassHash.HandleError())
                return ResultDTO.Fail(resGetPassHash.ErrMsg!);
            if (resGetPassHash.Data == null)
                return ResultDTO.Fail("Password hash not found");

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
                List<IdentityRoleClaim<string>>? roleClaims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(roleEntity.Id, "AuthorizationClaim");
                if (roleClaims is null)
                    return ResultDTO.Fail("Role claims not found");
                // Delete Existing Claims
                await _userManagementDa.DeleteClaimsForRole(roleClaims);

                foreach (var claim in dto.ClaimsInsert)
                {
                    IdentityRoleClaim<string>? roleClaim = _mapper.Map<IdentityRoleClaim<string>>(claim);
                    if (roleClaim == null)
                        return ResultDTO.Fail("Mapping failed for role claim");
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
                bool? hasUserEntry = _userManagementDa.CheckUserBeforeDelete(userId);
                if (!hasUserEntry.HasValue)
                    return ResultDTO<bool>.Fail("Error occured while retriving data");

                return ResultDTO<bool>.Ok(hasUserEntry.Value);
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
                ApplicationUser? user = await _userManagementDa.GetUserById(userId);
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

                List<IdentityRole>? roles = await _userManagementDa.GetAllRoles();
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
            catch (Exception ex)
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
                    IdentityRole? role = await _userManagementDa.GetRole(dto.Id);
                    if (role is null)
                        return ResultDTO<RoleManagementDTO>.Fail("Role not found");

                    dto = _mapper.Map<RoleManagementDTO>(role);
                    List<IdentityRoleClaim<string>>? claims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(role.Id, "AuthorizationClaim");
                    if (claims is null)
                        return ResultDTO<RoleManagementDTO>.Fail("Role claims not found");
                    dto.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
                }

                return ResultDTO<RoleManagementDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ResultDTO<RoleManagementDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<RoleDTO>> GetRoleById(string roleId)
        {
            IdentityRole? roleDb = await _userManagementDa.GetRole(roleId);
            if(roleDb is null)
                return ResultDTO<RoleDTO>.Fail("Role not found");

            RoleDTO? dto = _mapper.Map<RoleDTO>(roleDb);
            if(dto == null)            
                return ResultDTO<RoleDTO>.Fail("Mapping failed");
            
            return ResultDTO<RoleDTO>.Ok(dto);
        }

        public async Task<ResultDTO<UserDTO>> GetUserById(string userId)
        {
            ApplicationUser? userDb = await _userManagementDa.GetUserById(userId);
            if (userDb is null)
                return ResultDTO<UserDTO>.Fail("User not found");

            UserDTO? dto = _mapper.Map<UserDTO>(userDb);
            if (dto == null)
                return ResultDTO<UserDTO>.Fail("Mapping failed");

            return ResultDTO<UserDTO>.Ok(dto);
        }

        public async Task<ResultDTO<List<RoleClaimDTO>>> GetRoleClaims(string roleId)
        {
            List<IdentityRoleClaim<string>>? roleClaims = await _userManagementDa.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim");
            if (roleClaims == null)
                return ResultDTO<List<RoleClaimDTO>>.Fail("Role claims not found");

            List<RoleClaimDTO>? roleClaimDTOs = _mapper.Map<List<RoleClaimDTO>>(roleClaims);
            if (roleClaimDTOs == null)
                return ResultDTO<List<RoleClaimDTO>>.Fail("Role claims mapping failed");

            return ResultDTO<List<RoleClaimDTO>>.Ok(roleClaimDTOs);
        }
        public async Task<ResultDTO<List<UserClaimDTO>>> GetUserClaims(string userId)
        {
            List<IdentityUserClaim<string>>? userClaims = await _userManagementDa.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim");
            if (userClaims == null)
                return ResultDTO<List<UserClaimDTO>>.Fail("User claims not found");

            List<UserClaimDTO>? userClaimDTOs = _mapper.Map<List<UserClaimDTO>>(userClaims);
            if (userClaimDTOs == null)
                return ResultDTO<List<UserClaimDTO>>.Fail("User claims mapping failed");

            return ResultDTO<List<UserClaimDTO>>.Ok(userClaimDTOs);
        }

        public async Task<ResultDTO<List<RoleDTO>>> GetRolesForUser(string userId)
        {
            List<string>? roleIdsList = _userManagementDa.GetUserRolesByUserId(userId).GetAwaiter().GetResult().Select(x => x.RoleId).ToList();
            if (roleIdsList == null)
                return ResultDTO<List<RoleDTO>>.Fail("Role ids list not found");

            List<IdentityRole>? userRoles = await _userManagementDa.GetRolesForUser(roleIdsList);
            if(userRoles == null)
                return ResultDTO<List<RoleDTO>>.Fail("User roles not found");

            List<RoleDTO>? userRolesDTOs = _mapper.Map<List<RoleDTO>>(userRoles);
            if (userRolesDTOs == null)
                return ResultDTO<List<RoleDTO>>.Fail("Mapping failed");

            return ResultDTO<List<RoleDTO>>.Ok(userRolesDTOs);
        }

        public async Task<ResultDTO<UserDTO>> GetSuperAdminUserBySpecificClaim()
        {
            ApplicationUser? superAdmin = await _userManagementDa.GetUserBySpecificClaim();
            if (superAdmin == null)
                return ResultDTO<UserDTO>.Fail("Super admin not found");

            UserDTO? mappedUser = _mapper.Map<UserDTO>(superAdmin);
            if (mappedUser == null)
                return ResultDTO<UserDTO>.Fail("Mapping failed for super admin");

            return ResultDTO<UserDTO>.Ok(mappedUser);
        }
        public async Task<ResultDTO<List<UserDTO>>> GetAllIntanetPortalUsers()
        {
            List<ApplicationUser>? allUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            if (allUsers == null)
                return ResultDTO<List<UserDTO>>.Fail("Users not found");

            List<UserDTO>? mappedUsers = _mapper.Map<List<UserDTO>>(allUsers);
            if (mappedUsers == null)
                return ResultDTO<List<UserDTO>>.Fail("Mapping failed for users");

            return ResultDTO<List<UserDTO>>.Ok(mappedUsers);
        }

        public async Task<ResultDTO<List<RoleDTO>>> GetAllRoles()
        {
            List<IdentityRole>? allRoles = await _userManagementDa.GetAllRoles();
            if (allRoles == null)
                return ResultDTO<List<RoleDTO>>.Fail("Roles not found");
            List<RoleDTO>? mappedRoles = _mapper.Map<List<RoleDTO>>(allRoles);
            if (mappedRoles == null)
                return ResultDTO<List<RoleDTO>>.Fail("Mapping failed for roles");

            return ResultDTO<List<RoleDTO>>.Ok(mappedRoles);
        }

        public async Task<ResultDTO<List<UserRoleDTO>>> GetAllUserRoles()
        {
            List<IdentityUserRole<string>>? allUserRoles = await _userManagementDa.GetUserRoles();
            if (allUserRoles == null)
                return ResultDTO<List<UserRoleDTO>>.Fail("User roles not found");

            List<UserRoleDTO>? mappedUserRoles = _mapper.Map<List<UserRoleDTO>>(allUserRoles);
            if (mappedUserRoles == null)
                return ResultDTO<List<UserRoleDTO>>.Fail("Mapping failed for user roles");

            return ResultDTO<List<UserRoleDTO>>.Ok(mappedUserRoles);
        }
                
        public async Task<ResultDTO<string>> GetPreferredLanguageForUser(string userId)
        {
            string? preffLang = await _userManagementDa.GetPreferredLanguage(userId);
            if (string.IsNullOrEmpty(preffLang))
                return ResultDTO<string>.Fail("Prefereed language not found");

            return ResultDTO<string>.Ok(preffLang);
        }
        #endregion

    }
}
