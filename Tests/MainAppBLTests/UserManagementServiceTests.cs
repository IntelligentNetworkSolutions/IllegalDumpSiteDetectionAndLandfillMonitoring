using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using SD;
using Services;

namespace Tests.MainAppBLTests
{
    public class UserManagementServiceTests
    {
        private readonly Mock<IUserManagementDa> _mockUserManagementDa;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly UserManagementService _userManagementService;

        public UserManagementServiceTests()
        {
            _mockUserManagementDa = new Mock<IUserManagementDa>();
            _mockMapper = new Mock<IMapper>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _userManagementService = new UserManagementService(_mockUserManagementDa.Object, _mockAppSettingsAccessor.Object, _mockMapper.Object);
        }

        #region AddUser
        [Fact]
        public async Task AddUser_ReturnsFail_WhenDtoIsNull()
        {
            // Arrange
            // Act
            var result = await _userManagementService.AddUser(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("UserManagementDTO Must Not be Null", result.ErrMsg);
        }

        [Fact]
        public async Task AddUser_ReturnsFail_WhenPasswordHashFails()
        {
            // Arrange
            var dto = new UserManagementDTO
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                ConfirmPassword = "Password123!"
            };

            var mappedUser = new ApplicationUser();
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                        .ReturnsAsync(ResultDTO<int>.Ok(13));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(false));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(false));
            _mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(mappedUser);

            // Act
            var result = await _userManagementService.AddUser(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Password does not meet requirements", result.ErrMsg);
        }

        //[Fact]
        //public async Task AddUser_ReturnsFail_WhenUserRolesFail()
        //{
        //    // Arrange
        //    var dto = new UserManagementDTO
        //    {
        //        UserName = "testuser",
        //        Email = "testuser@example.com",
        //        ConfirmPassword = "Password123!",
        //        IsActive = true,
        //        FirstName = "test first name",
        //        LastName = "test Last Name"
        //        Roles = new List<RoleDTO>
        //        {
        //            new RoleDTO { Name = "Admin" },
        //            new RoleDTO { Name = "User" }
        //        }

        //    };

        //    var mappedUser = new ApplicationUser();
        //    _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
        //                .ReturnsAsync(ResultDTO<int>.Ok(10));
        //    _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
        //                            .ReturnsAsync(ResultDTO<bool>.Ok(false));
        //    _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
        //                            .ReturnsAsync(ResultDTO<bool>.Ok(false));
        //    _mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(mappedUser);

        //    // Act
        //    var result = await _userManagementService.AddUser(dto);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal("Password does not meet requirements", result.ErrMsg);
        //}
        #endregion

        #region AddLanguageClaimForUser
        [Fact]
        public async Task AddLanguageClaimForUser_AddsNewClaim_WhenNoExistingClaim()
        {
            // Arrange
            var userId = "testUser";
            var culture = "en-US";
            _mockUserManagementDa.Setup(x => x.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim"))
                                .ReturnsAsync(new List<IdentityUserClaim<string>>());
            _mockUserManagementDa.Setup(x => x.AddLanguageClaimForUser(It.IsAny<IdentityUserClaim<string>>()))
                                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<IdentityUserClaim<string>>(culture))
                      .Returns(new IdentityUserClaim<string> { ClaimValue = culture });

            // Act
            var result = await _userManagementService.AddLanguageClaimForUser(userId, culture);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManagementDa.Verify(x => x.AddLanguageClaimForUser(It.IsAny<IdentityUserClaim<string>>()), Times.Once);
        }


        [Fact]
        public async Task AddLanguageClaimForUser_UpdatesExistingClaim_WhenClaimExists()
        {
            // Arrange
            var userId = "testUser";
            var culture = "fr-FR";
            var existingClaims = new List<IdentityUserClaim<string>> {
        new IdentityUserClaim<string> { ClaimType = "PreferedLanguageClaim", UserId = userId }
    };
            _mockUserManagementDa.Setup(x => x.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim"))
                                .ReturnsAsync(existingClaims);
            _mockUserManagementDa.Setup(x => x.UpdateLanguageClaimForUser(It.IsAny<IdentityUserClaim<string>>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _userManagementService.AddLanguageClaimForUser(userId, culture);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManagementDa.Verify(x => x.UpdateLanguageClaimForUser(It.IsAny<IdentityUserClaim<string>>()), Times.Once);
        }

        [Fact]
        public async Task AddLanguageClaimForUser_ReturnsExceptionResult_OnException()
        {
            // Arrange
            var userId = "testUser";
            var culture = "de-DE";
            var exception = new Exception("Database error");
            _mockUserManagementDa.Setup(x => x.GetClaimsForUserByUserIdAndClaimType(userId, "PreferedLanguageClaim"))
                                .ThrowsAsync(exception);

            // Act
            var result = await _userManagementService.AddLanguageClaimForUser(userId, culture);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }
        #endregion

        #region GetPasswordRequirements
        [Fact]
        public async Task GetPasswordRequirements_ReturnsDefaultValues_WhenSettingsAreAbsent()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(4));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(false));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(false));

            // Act
            var result = await _userManagementService.GetPasswordRequirements();

            // Assert
            Assert.Equal(4, result.passMinLenght);
            Assert.False(result.passHasLetters);
            Assert.False(result.passHasNumbers);
        }

        [Fact]
        public async Task GetPasswordRequirements_ReturnsCustomValues_WhenSettingsAreProvided()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(8));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Act
            var result = await _userManagementService.GetPasswordRequirements();

            // Assert
            Assert.Equal(8, result.passMinLenght);
            Assert.True(result.passHasLetters);
            Assert.True(result.passHasNumbers);
        }

        [Fact]
        public async Task GetPasswordRequirements_HandlesExceptions_WhenServiceFails()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ThrowsAsync(new Exception("Service unavailable"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userManagementService.GetPasswordRequirements());
        }
        #endregion

        #region CheckPasswordRequirements
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CheckPasswordRequirements_ReturnsFalseForAll_WhenPasswordIsNullOrEmpty(string password)
        {
            // Act
            var result = await _userManagementService.CheckPasswordRequirements(password);

            // Assert
            Assert.False(result.hasMinLength);
            Assert.False(result.hasLetters);
            Assert.False(result.hasNumbers);
        }

        [Fact]
        public async Task CheckPasswordRequirements_ReturnsTrueForAll_WhenPasswordMeetsAllRequirements()
        {
            // Arrange
            string password = "Password123";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(8));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Act
            var result = await _userManagementService.CheckPasswordRequirements(password);

            // Assert
            Assert.True(result.hasMinLength);
            Assert.True(result.hasLetters);
            Assert.True(result.hasNumbers);
        }

        [Fact]
        public async Task CheckPasswordRequirements_ReturnsFalseForAll_WhenPasswordFailsAllRequirements()
        {
            // Arrange
            string password = "!";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(10));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Act
            var result = await _userManagementService.CheckPasswordRequirements(password);

            // Assert
            Assert.False(result.hasMinLength);
            Assert.False(result.hasLetters);
            Assert.False(result.hasNumbers);
        }

        [Fact]
        public async Task CheckPasswordRequirements_ReturnsMixedResults_WhenPasswordMeetsSomeRequirements()
        {
            // Arrange
            string password = "Password";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(8));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(false));

            // Act
            var result = await _userManagementService.CheckPasswordRequirements(password);

            // Assert
            Assert.True(result.hasMinLength);
            Assert.True(result.hasLetters);
            Assert.False(result.hasNumbers);
        }
        #endregion

        #region GetPasswordHash
        [Fact]
        public async Task GetPasswordHashForAppUser_ReturnsFail_WhenPasswordDoesNotMeetRequirements()
        {
            // Arrange
            var appUser = new ApplicationUser();
            string passwordNew = "weak";

            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(8));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            Mock<PasswordHasher<ApplicationUser>> mockPasswordHasher = new Mock<PasswordHasher<ApplicationUser>>();
            mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                                .Returns("hashedPassword");  // This line will not be reached in this test

            // Act
            var result = await _userManagementService.GetPasswordHashForAppUser(appUser, passwordNew);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Password does not meet requirements", result.ErrMsg);
        }

        [Fact]
        public async Task GetPasswordHashForAppUser_ReturnsSuccess_WhenAllConditionsMet()
        {
            // Arrange
            var appUser = new ApplicationUser() { Id = "TestId", UserName = "TestUser", Email = "testemail@testmailserver.test" };
            string passwordNew = "StrongPassword1";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                    .ReturnsAsync(ResultDTO<int>.Ok(8));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                    .ReturnsAsync(ResultDTO<bool>.Ok(true));

            Mock<PasswordHasher<ApplicationUser>> mockPasswordHasher = new Mock<PasswordHasher<ApplicationUser>>(null);
            mockPasswordHasher.Setup(x => x.HashPassword(appUser, passwordNew));

            // Act
            var result = await _userManagementService.GetPasswordHashForAppUser(appUser, passwordNew);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Data!);
        }
        #endregion

        #region AddUserClaims
        [Fact]
        public async Task AddUserClaims_ReturnsFail_WhenClaimsInsertIsNull()
        {
            // Arrange
            string appUserId = "user123";
            List<string> claimsInsert = null;

            // Act
            var result = await _userManagementService.AddUserClaims(appUserId, claimsInsert, "type");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ClaimsInsert must not be null", result.ErrMsg);
        }

        [Fact]
        public async Task AddUserClaims_ReturnsOk_WhenClaimsInsertIsEmpty()
        {
            // Arrange
            string appUserId = "user123";
            List<string> claimsInsert = new List<string>();

            // Act
            var result = await _userManagementService.AddUserClaims(appUserId, claimsInsert, "type");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddUserClaims_HandlesException()
        {
            // Arrange
            string appUserId = "user123";
            List<string> claimsInsert = new List<string> { "claim1" };

            // Setup the mock to throw an exception when trying to add claims
            _mockUserManagementDa.Setup(da => da.AddClaimsForUserRange(It.IsAny<List<IdentityUserClaim<string>>>(), null))
                                 .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _userManagementService.AddUserClaims(appUserId, claimsInsert, "type");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ExObj);
            Assert.Equal("Database error", result.ErrMsg);
        }

        #endregion

        #region AddUserRoles
        [Fact]
        public async Task AddUserRoles_ReturnsFail_WhenRolesInsertIsNull()
        {
            // Arrange
            string appUserId = "user123";
            List<string> rolesInsert = null;

            // Act
            var result = await _userManagementService.AddUserRoles(appUserId, rolesInsert);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("RolesInsert must not be null", result.ErrMsg);
        }

        [Fact]
        public async Task AddUserRoles_ReturnsOk_WhenRolesInsertIsEmpty()
        {
            // Arrange
            string appUserId = "user123";
            List<string> rolesInsert = new List<string>();

            // Act
            var result = await _userManagementService.AddUserRoles(appUserId, rolesInsert);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddUserRoles_SuccessfullyAddsRoles()
        {
            // Arrange
            string appUserId = "user123";
            List<string> rolesInsert = new List<string> { "Admin", "User" };
            var userRoles = rolesInsert.Select(role => new IdentityUserRole<string> { UserId = appUserId, RoleId = role }).ToList();
            _mockUserManagementDa.Setup(d => d.AddRolesForUserRange(It.IsAny<List<IdentityUserRole<string>>>(), null))
                                 .ReturnsAsync(userRoles);

            // Act
            var result = await _userManagementService.AddUserRoles(appUserId, rolesInsert);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManagementDa.Verify(da => da.AddRolesForUserRange(It.IsAny<List<IdentityUserRole<string>>>(), null), Times.Once);
        }

        [Fact]
        public async Task AddUserRoles_HandlesException()
        {
            // Arrange
            string appUserId = "user123";
            List<string> rolesInsert = new List<string> { "Admin", "User" };
            _mockMapper.Setup(m => m.Map<IdentityUserRole<string>>(It.IsAny<string>()))
                       .Returns((string roleId) => new IdentityUserRole<string> { UserId = appUserId, RoleId = roleId });

            _mockUserManagementDa.Setup(d => d.AddRolesForUserRange(It.IsAny<List<IdentityUserRole<string>>>(), null))
                                 .Throws(new Exception("Test exception"));

            // Act
            var result = await _userManagementService.AddUserRoles(appUserId, rolesInsert);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ExObj);
            Assert.Equal("Test exception", result.ErrMsg);
        }
        #endregion

        #region GetPreferredLanguage
        [Fact]
        public async Task GetPreferredLanguageForUser_Returns_CorrectLanguage_For_ValidUserId()
        {
            // Arrange
            var userId = "validUserId";
            var expectedLanguage = "English";
            _mockUserManagementDa.Setup(um => um.GetPreferredLanguage(userId)).ReturnsAsync(expectedLanguage);

            // Act
            var result = await _userManagementService.GetPreferredLanguageForUser(userId);

            // Assert
            Assert.Equal(expectedLanguage, result.Data);
        }

        [Fact]
        public async Task GetPreferredLanguageForUser_Handles_Exceptions_From_DALayer()
        {
            // Arrange
            var userId = "validUserId";
            _mockUserManagementDa.Setup(um => um.GetPreferredLanguage(userId)).ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userManagementService.GetPreferredLanguageForUser(userId));
        }
        #endregion

        #region AddRole
        [Fact]
        public async Task AddRole_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO
            {
                Name = "TestRole",
                ClaimsInsert = new List<string?>()
            };

            mockMapper.Setup(m => m.Map<IdentityRole>(It.IsAny<RoleManagementDTO>())).Throws<Exception>();

            // Act
            var result = await service.AddRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task AddRole_NullName_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO
            {
                Name = null,
                ClaimsInsert = new List<string?>()
            };

            // Act
            var result = await service.AddRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role must have a name", result.ErrMsg);
        }

        [Fact]
        public async Task AddRole_ValidData_ReturnsSuccessResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO
            {
                Name = "TestRole",
                ClaimsInsert = new List<string?>()
            };

            mockMapper.Setup(m => m.Map<IdentityRole>(It.IsAny<RoleManagementDTO>())).Returns(new IdentityRole());
            mockUserManagementDa.Setup(m => m.AddRole(It.IsAny<IdentityRole>())).ReturnsAsync(ResultDTO<IdentityRole>.Ok(new IdentityRole()));

            // Act
            var result = await service.AddRole(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }
        #endregion

        #region UpdateUser
        [Fact]
        public async Task UpdateUser_NullDto_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            UserManagementDTO dto = null;

            // Act
            var result = await service.UpdateUser(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("UserManagementDTO Must Not be Null", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new UserManagementDTO { Id = "nonexistentId" };

            mockUserManagementDa.Setup(m => m.GetUserById(dto.Id)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await service.UpdateUser(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateUser_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new UserManagementDTO
            {
                Id = "existingId",
                ConfirmPassword = "NewPassword"
            };

            var existingUser = new ApplicationUser { Id = dto.Id };

            mockUserManagementDa.Setup(m => m.GetUserById(dto.Id)).ReturnsAsync(existingUser);
            mockMapper.Setup(m => m.Map(dto, existingUser)).Throws<Exception>();

            // Act
            var result = await service.UpdateUser(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task UpdateUserPassword_NullOrEmptyFields_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            // Act
            var result = await service.UpdateUserPassword(null, "currentPassword", "newPassword");
            var result2 = await service.UpdateUserPassword("userId", null, "newPassword");
            var result3 = await service.UpdateUserPassword("userId", "currentPassword", null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("All fields are required", result.ErrMsg);

            Assert.False(result2.IsSuccess);
            Assert.Equal("All fields are required", result2.ErrMsg);

            Assert.False(result3.IsSuccess);
            Assert.Equal("All fields are required", result3.ErrMsg);
        }

        #endregion

        #region UpdateRole
        [Fact]
        public async Task UpdateRole_RoleNameIsEmpty_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO { Id = "roleId", Name = "" };

            // Act
            var result = await service.UpdateRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role Must have a Name", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateRole_RoleNotFound_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO { Id = "roleId", Name = "RoleName" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync((IdentityRole)null);

            // Act
            var result = await service.UpdateRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role not found", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateRole_UpdateRoleFails_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var roleEntity = new IdentityRole { Id = "roleId", Name = "RoleName" };
            var dto = new RoleManagementDTO { Id = "roleId", Name = "NewRoleName" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync(roleEntity);
            mockUserManagementDa.Setup(m => m.UpdateRole(It.IsAny<IdentityRole>())).ReturnsAsync(false);

            // Act
            var result = await service.UpdateRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role Not Updated", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateRole_ValidData_ReturnsSuccessResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var roleEntity = new IdentityRole { Id = "roleId", Name = "RoleName" };
            var dto = new RoleManagementDTO
            {
                Id = "roleId",
                Name = "NewRoleName",
                ClaimsInsert = new List<string?>()
            };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync(roleEntity);
            mockUserManagementDa.Setup(m => m.UpdateRole(It.IsAny<IdentityRole>())).ReturnsAsync(true);
            mockUserManagementDa.Setup(m => m.GetClaimsForRoleByRoleIdAndClaimType(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<IdentityRoleClaim<string>>());
            mockMapper.Setup(m => m.Map<IdentityRoleClaim<string>>(It.IsAny<ClaimDTO>()))
                .Returns((ClaimDTO dto) => new IdentityRoleClaim<string> { ClaimType = dto.ClaimType, ClaimValue = dto.ClaimValue });

            // Act
            var result = await service.UpdateRole(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task UpdateRole_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();

            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            var dto = new RoleManagementDTO { Id = "roleId", Name = "RoleName" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.UpdateRole(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test exception", result.ErrMsg);
        }
        #endregion

        #region DeteletUser
        [Fact]
        public async Task CheckUserBeforeDelete_UserExists_ReturnsSuccessResultWithTrue()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.CheckUserBeforeDelete(It.IsAny<string>())).Returns(true);

            // Act
            var result = await service.CheckUserBeforeDelete("userId");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task CheckUserBeforeDelete_UserDoesNotExist_ReturnsSuccessResultWithFalse()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.CheckUserBeforeDelete(It.IsAny<string>())).Returns(false);

            // Act
            var result = await service.CheckUserBeforeDelete("userId");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Data);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task CheckUserBeforeDelete_UserCheckReturnsNull_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.CheckUserBeforeDelete(It.IsAny<string>())).Returns((bool?)null);

            // Act
            var result = await service.CheckUserBeforeDelete("userId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error occured while retriving data", result.ErrMsg);
        }

        [Fact]
        public async Task CheckUserBeforeDelete_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.CheckUserBeforeDelete(It.IsAny<string>())).Throws(new Exception("Test exception"));

            // Act
            var result = await service.CheckUserBeforeDelete("userId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test exception", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteUser_UserExists_ReturnsSuccessResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var user = new ApplicationUser { Id = "userId" };

            mockUserManagementDa.Setup(m => m.GetUserById(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManagementDa.Setup(m => m.DeleteUser(It.IsAny<ApplicationUser>())).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await service.DeleteUser("userId");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetUserById(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await service.DeleteUser("userId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteUser_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetUserById(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.DeleteUser("userId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test exception", result.ErrMsg);
        }
        #endregion

        #region DeleteRole
        [Fact]
        public async Task DeleteRole_RoleNotFound_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync((IdentityRole)null);

            // Act
            var result = await service.DeleteRole("roleId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteRole_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.DeleteRole("roleId");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test exception", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteRole_RoleExists_ReturnsSuccessResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var role = new IdentityRole { Id = "roleId" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync(role);
            mockUserManagementDa.Setup(m => m.DeleteRole(It.IsAny<IdentityRole>())).ReturnsAsync(new IdentityRole());

            // Act
            var result = await service.DeleteRole("roleId");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }
        #endregion

        #region GetRole/s
        [Fact]
        public async Task GetRoleById_RoleExists_ReturnsRoleDTO()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var role = new IdentityRole { Id = "roleId", Name = "TestRole" };
            var roleDto = new RoleDTO { Id = "roleId", Name = "TestRole" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync(role);
            mockMapper.Setup(m => m.Map<RoleDTO>(It.IsAny<IdentityRole>())).Returns(roleDto);

            // Act
            var result = await service.GetRoleById("roleId");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleDto.Id, result.Data.Id);
            Assert.Equal(roleDto.Name, result.Data.Name);
        }


        [Fact]
        public async Task GetRoleById_ShouldReturnFail_WhenRoleNotFound()
        {
            // Arrange
            string roleId = "non-existent-role-id";

            _mockUserManagementDa.Setup(x => x.GetRole(roleId))
                .ReturnsAsync((IdentityRole)null);

            // Act
            var result = await _userManagementService.GetRoleById(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Role not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnFail_WhenMappingFails()
        {
            // Arrange
            string roleId = "test-role-id";
            var identityRole = new IdentityRole
            {
                Id = roleId,
                Name = "Test Role"
            };

            _mockUserManagementDa.Setup(x => x.GetRole(roleId))
                .ReturnsAsync(identityRole);

            _mockMapper.Setup(x => x.Map<RoleDTO>(identityRole))
                .Returns((RoleDTO)null);

            // Act
            var result = await _userManagementService.GetRoleById(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetRoleById_ShouldHandleNullRoleId()
        {
            // Arrange
            string roleId = null;

            // Act
            var result = await _userManagementService.GetRoleById(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Role not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetRoleById_ShouldWorkWithMinimalRoleData()
        {
            // Arrange
            string roleId = "minimal-role-id";
            var identityRole = new IdentityRole { Id = roleId };
            var expectedRoleDto = new RoleDTO { Id = roleId };

            _mockUserManagementDa.Setup(x => x.GetRole(roleId))
                .ReturnsAsync(identityRole);

            _mockMapper.Setup(x => x.Map<RoleDTO>(identityRole))
                .Returns(expectedRoleDto);

            // Act
            var result = await _userManagementService.GetRoleById(roleId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(roleId, result.Data.Id);
        }

        [Fact]
        public async Task GetRoleById_ShouldHandleSpecialCharactersInRoleId()
        {
            // Arrange
            string roleId = "role-with-special-chars-!@#$%^&*()";
            var identityRole = new IdentityRole
            {
                Id = roleId,
                Name = "Special Characters Role"
            };
            var expectedRoleDto = new RoleDTO
            {
                Id = roleId,
                Name = "Special Characters Role"
            };

            _mockUserManagementDa.Setup(x => x.GetRole(roleId))
                .ReturnsAsync(identityRole);

            _mockMapper.Setup(x => x.Map<RoleDTO>(identityRole))
                .Returns(expectedRoleDto);

            // Act
            var result = await _userManagementService.GetRoleById(roleId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(roleId, result.Data.Id);
        }
        #endregion

        #region GetUser/s
        [Fact]
        public async Task GetUserById_UserExists_ReturnsUserDTO()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var user = new ApplicationUser { Id = "userId", UserName = "TestUser" };
            var userDto = new UserDTO { Id = "userId", UserName = "TestUser" };

            mockUserManagementDa.Setup(m => m.GetUserById(It.IsAny<string>())).ReturnsAsync(user);
            mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<ApplicationUser>())).Returns(userDto);

            // Act
            var result = await service.GetUserById("userId");

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(userDto.Id, result.Data.Id);
            Assert.Equal(userDto.UserName, result.Data.UserName);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnFail_WhenUserNotFound()
        {
            // Arrange
            string userId = "non-existent-user-id";

            _mockUserManagementDa.Setup(x => x.GetUserById(userId))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _userManagementService.GetUserById(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnFail_WhenMappingFails()
        {
            // Arrange
            string userId = "test-user-id";
            var applicationUser = new ApplicationUser
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com"
            };

            _mockUserManagementDa.Setup(x => x.GetUserById(userId))
                .ReturnsAsync(applicationUser);

            _mockMapper.Setup(x => x.Map<UserDTO>(applicationUser))
                .Returns((UserDTO)null);

            // Act
            var result = await _userManagementService.GetUserById(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetUserById_ShouldHandleNullUserId()
        {
            // Arrange
            string userId = null;

            // Act
            var result = await _userManagementService.GetUserById(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetUserById_ShouldWorkWithMinimalUserData()
        {
            // Arrange
            string userId = "minimal-user-id";
            var applicationUser = new ApplicationUser { Id = userId };
            var expectedUserDto = new UserDTO { Id = userId };

            _mockUserManagementDa.Setup(x => x.GetUserById(userId))
                .ReturnsAsync(applicationUser);

            _mockMapper.Setup(x => x.Map<UserDTO>(applicationUser))
                .Returns(expectedUserDto);

            // Act
            var result = await _userManagementService.GetUserById(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
        }
        #endregion

        #region GetRoleClaims
        [Fact]
        public async Task GetRoleClaims_ShouldReturnFail_WhenRoleClaimsNotFound()
        {
            // Arrange
            string roleId = "roleId123";
            _mockUserManagementDa.Setup(da => da.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim"))
                           .ReturnsAsync((List<IdentityRoleClaim<string>>)null);

            // Act
            var result = await _userManagementService.GetRoleClaims(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role claims not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetRoleClaims_ShouldReturnFail_WhenMappingFails()
        {
            // Arrange
            string roleId = "roleId123";
            var roleClaims = new List<IdentityRoleClaim<string>>
            {
                new IdentityRoleClaim<string> { ClaimType = "AuthorizationClaim", ClaimValue = "value1" }
            };

            _mockUserManagementDa.Setup(da => da.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim"))
                           .ReturnsAsync(roleClaims);

            _mockMapper.Setup(m => m.Map<List<RoleClaimDTO>>(roleClaims))
                       .Returns((List<RoleClaimDTO>)null);

            // Act
            var result = await _userManagementService.GetRoleClaims(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role claims mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetRoleClaims_RoleClaimsExist_ReturnsRoleClaimDTOs()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var roleClaims = new List<IdentityRoleClaim<string>>
            {
                new IdentityRoleClaim<string> { Id = 1, RoleId = "roleId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue1" },
                new IdentityRoleClaim<string> { Id = 2, RoleId = "roleId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue2" }
            };

            var roleClaimDTOs = new List<RoleClaimDTO>
            {
                new RoleClaimDTO { Id = 1, RoleId = "roleId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue1" },
                new RoleClaimDTO { Id = 2, RoleId = "roleId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue2" }
            };

            mockUserManagementDa.Setup(m => m.GetClaimsForRoleByRoleIdAndClaimType(It.IsAny<string>(), "AuthorizationClaim")).ReturnsAsync(roleClaims);
            mockMapper.Setup(m => m.Map<List<RoleClaimDTO>>(It.IsAny<List<IdentityRoleClaim<string>>>())).Returns(roleClaimDTOs);

            // Act
            var result = await service.GetRoleClaims("roleId");

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(roleClaimDTOs.Count, result.Data.Count);
            for (int i = 0; i < roleClaimDTOs.Count; i++)
            {
                Assert.Equal(roleClaimDTOs[i].Id, result.Data[i].Id);
                Assert.Equal(roleClaimDTOs[i].RoleId, result.Data[i].RoleId);
                Assert.Equal(roleClaimDTOs[i].ClaimType, result.Data[i].ClaimType);
                Assert.Equal(roleClaimDTOs[i].ClaimValue, result.Data[i].ClaimValue);
            }
        }
        #endregion

        #region GetUserClaims
        [Fact]
        public async Task GetUserClaims_UserClaimsExist_ReturnsUserClaimDTOs()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var userClaims = new List<IdentityUserClaim<string>>
            {
                new IdentityUserClaim<string> { Id = 1, UserId = "userId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue1" },
                new IdentityUserClaim<string> { Id = 2, UserId = "userId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue2" }
            };

            var userClaimDTOs = new List<UserClaimDTO>
            {
                new UserClaimDTO { Id = 1, UserId = "userId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue1" },
                new UserClaimDTO { Id = 2, UserId = "userId", ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue2" }
            };

            mockUserManagementDa.Setup(m => m.GetClaimsForUserByUserIdAndClaimType(It.IsAny<string>(), "AuthorizationClaim")).ReturnsAsync(userClaims);
            mockMapper.Setup(m => m.Map<List<UserClaimDTO>>(It.IsAny<List<IdentityUserClaim<string>>>())).Returns(userClaimDTOs);

            // Act
            var result = await service.GetUserClaims("userId");

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(userClaimDTOs.Count, result.Data.Count);
            for (int i = 0; i < userClaimDTOs.Count; i++)
            {
                Assert.Equal(userClaimDTOs[i].Id, result.Data[i].Id);
                Assert.Equal(userClaimDTOs[i].UserId, result.Data[i].UserId);
                Assert.Equal(userClaimDTOs[i].ClaimType, result.Data[i].ClaimType);
                Assert.Equal(userClaimDTOs[i].ClaimValue, result.Data[i].ClaimValue);
            }
        }

       
        #endregion

        #region GetRolesForUser
        [Fact]
        public async Task GetRolesForUser_RolesExist_ReturnsRoleDTOs()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = "userId", RoleId = "roleId1" },
                new IdentityUserRole<string> { UserId = "userId", RoleId = "roleId2" }
            };

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "roleId1", Name = "Role1" },
                new IdentityRole { Id = "roleId2", Name = "Role2" }
            };

            var roleDTOs = new List<RoleDTO>
            {
                new RoleDTO { Id = "roleId1", Name = "Role1" },
                new RoleDTO { Id = "roleId2", Name = "Role2" }
            };

            mockUserManagementDa.Setup(m => m.GetUserRolesByUserId(It.IsAny<string>())).ReturnsAsync(userRoles);
            mockUserManagementDa.Setup(m => m.GetRolesForUser(It.IsAny<List<string>>())).ReturnsAsync(roles);
            mockMapper.Setup(m => m.Map<List<RoleDTO>>(It.IsAny<List<IdentityRole>>())).Returns(roleDTOs);

            // Act
            var result = await service.GetRolesForUser("userId");

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(roleDTOs.Count, result.Data.Count);
            for (int i = 0; i < roleDTOs.Count; i++)
            {
                Assert.Equal(roleDTOs[i].Id, result.Data[i].Id);
                Assert.Equal(roleDTOs[i].Name, result.Data[i].Name);
            }
        }

        [Fact]
        public async Task GetRolesForUser_UserRolesAreNull_ReturnsFailure()
        {
            // Arrange
            var userId = "validUserId";
            var roleIds = new List<IdentityUserRole<string>> { new IdentityUserRole<string> { RoleId = "role1" } };

            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetUserRolesByUserId(userId)).ReturnsAsync(new List<IdentityUserRole<string>>());
            mockUserManagementDa.Setup(m => m.GetRolesForUser(It.IsAny<List<string>>())).ReturnsAsync((List<IdentityRole>)null);

            // Act
            var result = await service.GetRolesForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("User roles not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetRolesForUser_MappingFails_ReturnsFailure()
        {
            // Arrange
            var userId = "validUserId";
            var roleIds = new List<IdentityUserRole<string>> { new IdentityUserRole<string> { RoleId = "role1" } };
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "role1", Name = "Admin" }
            };

            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var service = new UserManagementService(mockUserManagementDa.Object, null, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetUserRolesByUserId(userId)).ReturnsAsync(roleIds);
            mockUserManagementDa.Setup(m => m.GetRolesForUser(It.IsAny<List<string>>())).ReturnsAsync(roles);
            mockMapper.Setup(m => m.Map<List<RoleDTO>>(roles)).Returns((List<RoleDTO>)null);

            // Act
            var result = await service.GetRolesForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        #endregion

        #region FillRoleManagementDto
        [Fact]
        public async Task FillRoleManagementDto_NoDtoProvided_ReturnsNewDto()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            // Act
            var result = await service.FillRoleManagementDto();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<RoleManagementDTO>(result.Data);
        }

        [Fact]
        public async Task FillRoleManagementDto_RoleNotFound_ReturnsFailure()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);
            var dto = new RoleManagementDTO { Id = "roleId" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync((IdentityRole)null);

            // Act
            var result = await service.FillRoleManagementDto(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Role not found", result.ErrMsg);
        }

        [Fact]
        public async Task FillRoleManagementDto_ValidDtoProvided_ReturnsFilledDto()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);
            var dto = new RoleManagementDTO { Id = "roleId" };

            var role = new IdentityRole { Id = "roleId", Name = "RoleName" };
            var claims = new List<IdentityRoleClaim<string>>
            {
                new IdentityRoleClaim<string> { ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue1" },
                new IdentityRoleClaim<string> { ClaimType = "AuthorizationClaim", ClaimValue = "ClaimValue2" }
            };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ReturnsAsync(role);
            mockUserManagementDa.Setup(m => m.GetClaimsForRoleByRoleIdAndClaimType(It.IsAny<string>(), "AuthorizationClaim")).ReturnsAsync(claims);
            mockMapper.Setup(m => m.Map<RoleManagementDTO>(It.IsAny<IdentityRole>())).Returns(new RoleManagementDTO { Id = "roleId", Name = "RoleName" });

            // Act
            var result = await service.FillRoleManagementDto(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("roleId", result.Data.Id);
            Assert.Equal("RoleName", result.Data.Name);
            Assert.Equal(2, result.Data.ClaimsInsert.Count);
            Assert.Contains("ClaimValue1", result.Data.ClaimsInsert);
            Assert.Contains("ClaimValue2", result.Data.ClaimsInsert);
        }

        [Fact]
        public async Task FillRoleManagementDto_ExceptionThrown_ReturnsExceptionFail()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var exceptionMessage = "Test exception";
            var dto = new RoleManagementDTO { Id = "roleId" };

            mockUserManagementDa.Setup(m => m.GetRole(It.IsAny<string>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await service.FillRoleManagementDto(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
            Assert.IsType<Exception>(result.ExObj);
        }
        #endregion

        #region GetAllUserRoles
        [Fact]
        public async Task GetAllUserRoles_Success_ReturnsMappedUserRoles()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var userRoles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string> { UserId = "userId1", RoleId = "roleId1" },
                    new IdentityUserRole<string> { UserId = "userId2", RoleId = "roleId2" }
                };

            var userRoleDTOs = new List<UserRoleDTO>
                {
                    new UserRoleDTO { UserId = "userId1", RoleId = "roleId1" },
                    new UserRoleDTO { UserId = "userId2", RoleId = "roleId2" }
                };

            // Setup mocks
            mockUserManagementDa.Setup(m => m.GetUserRoles()).ReturnsAsync(userRoles);
            mockMapper.Setup(m => m.Map<List<UserRoleDTO>>(It.IsAny<List<IdentityUserRole<string>>>()))
                      .Returns(userRoleDTOs);

            // Act
            var result = await service.GetAllUserRoles();

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, ur => ur.UserId == "userId1" && ur.RoleId == "roleId1");
            Assert.Contains(result.Data, ur => ur.UserId == "userId2" && ur.RoleId == "roleId2");
        }

        [Fact]
        public async Task GetAllUserRoles_NoRoles_ReturnsEmptyList()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetUserRoles()).ReturnsAsync(new List<IdentityUserRole<string>>());
            mockMapper.Setup(m => m.Map<List<UserRoleDTO>>(It.IsAny<List<IdentityUserRole<string>>>()))
                      .Returns(new List<UserRoleDTO>());

            // Act
            var result = await service.GetAllUserRoles();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
        }
        #endregion

        #region GetAllRoles
        [Fact]
        public async Task GetAllRoles_Success_ReturnsMappedRoles()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var roles = new List<IdentityRole>
                {
                    new IdentityRole { Id = "roleId1", Name = "Role1" },
                    new IdentityRole { Id = "roleId2", Name = "Role2" }
                };

            var roleDTOs = new List<RoleDTO>
                {
                    new RoleDTO { Id = "roleId1", Name = "Role1" },
                    new RoleDTO { Id = "roleId2", Name = "Role2" }
                };

            mockUserManagementDa.Setup(m => m.GetAllRoles()).ReturnsAsync(roles);
            mockMapper.Setup(m => m.Map<List<RoleDTO>>(It.IsAny<List<IdentityRole>>())).Returns(roleDTOs);

            // Act
            var result = await service.GetAllRoles();

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, r => r.Id == "roleId1" && r.Name == "Role1");
            Assert.Contains(result.Data, r => r.Id == "roleId2" && r.Name == "Role2");
        }

        [Fact]
        public async Task GetAllRoles_NoRoles_ReturnsEmptyList()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetAllRoles()).ReturnsAsync(new List<IdentityRole>());
            mockMapper.Setup(m => m.Map<List<RoleDTO>>(It.IsAny<List<IdentityRole>>())).Returns(new List<RoleDTO>());

            // Act
            var result = await service.GetAllRoles();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetAllIntanetPortalUsers
        [Fact]
        public async Task GetAllIntranetPortalUsers_Success_ReturnsMappedUsers()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var users = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = "userId1", UserName = "User1" },
                    new ApplicationUser { Id = "userId2", UserName = "User2" }
                };

            var userDTOs = new List<UserDTO>
                {
                    new UserDTO { Id = "userId1", UserName = "User1" },
                    new UserDTO { Id = "userId2", UserName = "User2" }
                };

            mockUserManagementDa.Setup(m => m.GetAllIntanetPortalUsers()).ReturnsAsync(users);
            mockMapper.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<ApplicationUser>>())).Returns(userDTOs);

            // Act
            ResultDTO<List<UserDTO>> result = await service.GetAllIntanetPortalUsers();

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, u => u.Id == "userId1" && u.UserName == "User1");
            Assert.Contains(result.Data, u => u.Id == "userId2" && u.UserName == "User2");
        }


        [Fact]
        public async Task GetAllIntanetPortalUsers_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            mockUserManagementDa.Setup(m => m.GetAllIntanetPortalUsers()).ReturnsAsync(new List<ApplicationUser>());
            mockMapper.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<ApplicationUser>>())).Returns(new List<UserDTO>());

            // Act
            var result = await service.GetAllIntanetPortalUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetSuperAdminUserBySpecificClaim
        [Fact]
        public async Task GetSuperAdminUserBySpecificClaim_Success_ReturnsMappedUser()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var superAdmin = new ApplicationUser { Id = "superAdminId", UserName = "SuperAdmin" };
            var userDTO = new UserDTO { Id = "superAdminId", UserName = "SuperAdmin" };

            mockUserManagementDa.Setup(m => m.GetUserBySpecificClaim()).ReturnsAsync(superAdmin);
            mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<ApplicationUser>())).Returns(userDTO);

            // Act
            var result = await service.GetSuperAdminUserBySpecificClaim();

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal("superAdminId", result.Data.Id);
            Assert.Equal("SuperAdmin", result.Data.UserName);
        }

        [Fact]
        public async Task GetSuperAdminUserBySpecificClaim_NoUser_ReturnsFailure()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            // Mock GetUserBySpecificClaim to return null
            mockUserManagementDa.Setup(m => m.GetUserBySpecificClaim()).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await service.GetSuperAdminUserBySpecificClaim();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Super admin not found", result.ErrMsg);
            Assert.Null(result.Data);
        }

        #endregion

        #region FillUserManagementDto

        [Fact]
        public async Task FillUserManagementDto_NullDto_ReturnsPopulatedDto()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var userId = "userId1";
            var user = new ApplicationUser { Id = userId, UserName = "User1" };
            var userRoles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string> { UserId = userId, RoleId = "roleId1" }
                };
            var userClaims = new List<IdentityUserClaim<string>>
                {
                    new IdentityUserClaim<string> { ClaimValue = "Claim1", UserId = userId }
                };
            var allUsers = new List<ApplicationUser> { user };
            var roles = new List<IdentityRole>
                {
                    new IdentityRole { Id = "roleId1", Name = "Role1" }
                };
            var roleClaims = new List<IdentityRoleClaim<string>>
                {
                    new IdentityRoleClaim<string> { Id = 1, RoleId = "roleId1", ClaimValue = "TestClaimValue", ClaimType = "TestClaimType" }
                };

            mockUserManagementDa.Setup(m => m.GetUserById(userId)).ReturnsAsync(user);
            mockUserManagementDa.Setup(m => m.GetUserRolesByUserId(userId)).ReturnsAsync(userRoles);
            mockUserManagementDa.Setup(m => m.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim")).ReturnsAsync(userClaims);
            mockUserManagementDa.Setup(m => m.GetAllIntanetPortalUsers()).ReturnsAsync(allUsers);
            mockUserManagementDa.Setup(m => m.GetAllRoles()).ReturnsAsync(roles);
            mockUserManagementDa.Setup(m => m.GetAllRoleClaims()).ReturnsAsync(roleClaims);

            // Setup for password requirements
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(8));
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));

            mockMapper.Setup(m => m.Map<ApplicationUser, UserManagementDTO>(It.IsAny<ApplicationUser>(), It.IsAny<UserManagementDTO>()))
                      .Returns(new UserManagementDTO { Id = userId });
            mockMapper.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<ApplicationUser>>()))
                      .Returns(new List<UserDTO> { new UserDTO { Id = userId, UserName = "User1" } });
            mockMapper.Setup(m => m.Map<List<RoleDTO>>(It.IsAny<List<IdentityRole>>()))
                      .Returns(new List<RoleDTO> { new RoleDTO { Id = "roleId1", Name = "Role1" } });
            mockMapper.Setup(m => m.Map<List<RoleClaimDTO>>(It.IsAny<List<IdentityRoleClaim<string>>>()))
                      .Returns(new List<RoleClaimDTO> { new RoleClaimDTO { Id = 1, RoleId = "roleId1", ClaimValue = "TestClaimValue", ClaimType = "TestClaimType" } });

            // Act
            var result = await service.FillUserManagementDto(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            var userInAllUsers = result.Data.AllUsers.FirstOrDefault(u => u.Id == userId);
            Assert.NotNull(userInAllUsers);
            Assert.Equal(userId, userInAllUsers.Id);
            Assert.Equal("User1", userInAllUsers.UserName);
            Assert.Single(result.Data.RoleClaims);
            Assert.Equal("TestClaimValue", result.Data.RoleClaims.First().ClaimValue);
            Assert.Equal("TestClaimType", result.Data.RoleClaims.First().ClaimType);
            Assert.Equal(8, result.Data.PasswordMinLength);
            Assert.True(result.Data.PasswordMustHaveLetters);
            Assert.True(result.Data.PasswordMustHaveNumbers);
        }


        [Fact]
        public async Task FillUserManagementDto_UserNotFound_ReturnsFailResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            var userId = "userId1";

            mockUserManagementDa.Setup(m => m.GetUserById(userId)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await service.FillUserManagementDto(new UserManagementDTO { Id = userId });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task FillUserManagementDto_ExceptionThrown_ReturnsExceptionFailResult()
        {
            // Arrange
            var mockUserManagementDa = new Mock<IUserManagementDa>();
            var mockMapper = new Mock<IMapper>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

            // Mocking other required settings
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(8));
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));

            mockUserManagementDa.Setup(m => m.GetUserById(It.IsAny<string>()))
                                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await service.FillUserManagementDto(new UserManagementDTO { Id = "userId1" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
        }



        //[Fact]
        //public async Task FillUserManagementDto_NoRolesOrClaims_ReturnsPopulatedDto()
        //{
        //    // Arrange
        //    var mockUserManagementDa = new Mock<IUserManagementDa>();
        //    var mockMapper = new Mock<IMapper>();
        //    var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
        //    var service = new UserManagementService(mockUserManagementDa.Object, mockAppSettingsAccessor.Object, mockMapper.Object);

        //    var userId = "userId1";
        //    var user = new ApplicationUser { Id = userId, UserName = "User1" };
        //    var userRoles = new List<IdentityUserRole<string>>();
        //    var userClaims = new List<IdentityUserClaim<string>>();
        //    var allUsers = new List<ApplicationUser> { user };
        //    var roles = new List<IdentityRole>();
        //    var roleClaims = new List<IdentityRoleClaim<string>>();

        //    // Mocking data access methods
        //    mockUserManagementDa.Setup(m => m.GetUserById(userId)).ReturnsAsync(user);
        //    mockUserManagementDa.Setup(m => m.GetUserRolesByUserId(userId)).ReturnsAsync(userRoles);
        //    mockUserManagementDa.Setup(m => m.GetClaimsForUserByUserIdAndClaimType(userId, "AuthorizationClaim")).ReturnsAsync(userClaims);
        //    mockUserManagementDa.Setup(m => m.GetAllIntanetPortalUsers()).ReturnsAsync(allUsers);
        //    mockUserManagementDa.Setup(m => m.GetAllRoles()).ReturnsAsync(roles);
        //    mockUserManagementDa.Setup(m => m.GetAllRoleClaims()).ReturnsAsync(roleClaims);

        //    // Setup for password requirements
        //    mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
        //                           .ReturnsAsync(ResultDTO<int>.Ok(4));
        //    mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
        //                           .ReturnsAsync(ResultDTO<bool>.Ok(false));
        //    mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
        //                           .ReturnsAsync(ResultDTO<bool>.Ok(false));

        //    // Mocking mapping
        //    mockMapper.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<ApplicationUser>>()))
        //              .Returns(allUsers.Select(u => new UserDTO { Id = u.Id, UserName = u.UserName }).ToList());
        //    // Act
        //    var result = await service.FillUserManagementDto(new UserManagementDTO { Id = userId });
        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.NotNull(result.Data);

        //    // Check if roles and claims are empty
        //    Assert.Empty(result.Data.RolesInsert);
        //    Assert.Empty(result.Data.ClaimsInsert);

        //    // Check if all users is populated correctly
        //    var userInAllUsers = result.Data.AllUsers?.FirstOrDefault(u => u.Id == userId);
        //    Assert.NotNull(userInAllUsers);
        //    Assert.Equal(userId, userInAllUsers.Id);
        //    Assert.Equal("User1", userInAllUsers.UserName);

        //    // Check password requirements
        //    Assert.Equal(4, result.Data.PasswordMinLength);
        //    Assert.False(result.Data.PasswordMustHaveLetters);
        //    Assert.False(result.Data.PasswordMustHaveNumbers);
        //}

        #endregion


        #region Missing Tests Unsorted
        #region UpdateUserPassword
        [Fact]
        public async Task UpdateUserPassword_AllFieldsRequired_ReturnsFailure()
        {
            // Arrange
            var service = new UserManagementService(_mockUserManagementDa.Object, _mockAppSettingsAccessor.Object, _mockMapper.Object);

            // Act & Assert
            var result1 = await service.UpdateUserPassword(null, "current", "new");
            var result2 = await service.UpdateUserPassword("userId", null, "new");
            var result3 = await service.UpdateUserPassword("userId", "current", null);

            Assert.False(result1.IsSuccess);
            Assert.False(result2.IsSuccess);
            Assert.False(result3.IsSuccess);
            Assert.Equal("All fields are required", result1.ErrMsg);
            Assert.Equal("All fields are required", result2.ErrMsg);
            Assert.Equal("All fields are required", result3.ErrMsg);
        }
             
        [Fact]
        public async Task UpdateUserPassword_PasswordValidationFails_ReturnsFailure()
        {
            // Arrange
            var service = new UserManagementService(_mockUserManagementDa.Object, _mockAppSettingsAccessor.Object, _mockMapper.Object);
            var user = new ApplicationUser { Id = "userId", UserName = "testUser" };
            var userDto = new UserDTO { Id = "userId", UserName = "testUser" };

            _mockUserManagementDa.Setup(x => x.GetUserById("userId")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<ApplicationUser>())).Returns(userDto);
            _mockMapper.Setup(m => m.Map<ApplicationUser>(It.IsAny<UserDTO>())).Returns(user);

            // Setup password requirements to force validation failure
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(20)); // Force minimum length failure

            // Act
            var result = await service.UpdateUserPassword("userId", "current", "new");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Password does not meet requirements", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateUserPassword_UpdateFails_ReturnsFailure()
        {
            // Arrange
            var service = new UserManagementService(_mockUserManagementDa.Object, _mockAppSettingsAccessor.Object, _mockMapper.Object);
            var user = new ApplicationUser { Id = "userId", UserName = "testUser" };
            string pass = "Newpassword123";
            var userDto = new UserDTO { Id = "userId", UserName = "testUser", Password = pass, PasswordHash = pass };

            _mockUserManagementDa.Setup(x => x.GetUserById("userId")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<ApplicationUser>())).Returns(userDto);
            _mockMapper.Setup(m => m.Map<ApplicationUser>(It.IsAny<UserDTO>())).Returns(user);
            _mockUserManagementDa.Setup(x => x.UpdateUser(It.IsAny<ApplicationUser>())).ReturnsAsync(false);

            // Setup password requirements
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(4));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(false));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(false));

            // Act
            var result = await service.UpdateUserPassword("userId", "current", pass);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed while Updating User", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateUserPassword_Success_ReturnsSuccess()
        {
            // Arrange
            var service = new UserManagementService(_mockUserManagementDa.Object, _mockAppSettingsAccessor.Object, _mockMapper.Object);
            var user = new ApplicationUser { Id = "userId", UserName = "testUser" };
            string pass = "Newpassword123";
            var userDto = new UserDTO { Id = "userId", UserName = "testUser", Password = pass, PasswordHash = pass };

            _mockUserManagementDa.Setup(x => x.GetUserById("userId")).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<ApplicationUser>())).Returns(userDto);
            _mockMapper.Setup(m => m.Map<ApplicationUser>(It.IsAny<UserDTO>())).Returns(user);
            _mockUserManagementDa.Setup(x => x.UpdateUser(It.IsAny<ApplicationUser>())).ReturnsAsync(true);

            // Setup password requirements
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(4));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(false));

            // Act
            var result = await service.UpdateUserPassword("userId", "current", pass);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }
        #endregion


        #region FillRoleManagementDto Additional Tests
        [Fact]
        public async Task FillRoleManagementDto_WithNoExistingClaims_ReturnsDtoWithEmptyClaims()
        {
            // Arrange
            var roleId = "testRoleId";
            var role = new IdentityRole { Id = roleId, Name = "TestRole" };
            var dto = new RoleManagementDTO { Id = roleId };

            _mockUserManagementDa.Setup(x => x.GetRole(roleId)).ReturnsAsync(role);
            _mockUserManagementDa.Setup(x => x.GetClaimsForRoleByRoleIdAndClaimType(roleId, "AuthorizationClaim"))
                                .ReturnsAsync(new List<IdentityRoleClaim<string>>());
            _mockMapper.Setup(m => m.Map<RoleManagementDTO>(It.IsAny<IdentityRole>()))
                      .Returns(new RoleManagementDTO { Id = roleId, Name = "TestRole" });

            // Act
            var result = await _userManagementService.FillRoleManagementDto(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.ClaimsInsert);
        }

        [Fact]
        public async Task FillRoleManagementDto_WhenMappingFails_ReturnsFailure()
        {
            // Arrange
            var roleId = "testRoleId";
            var role = new IdentityRole { Id = roleId, Name = "TestRole" };
            var dto = new RoleManagementDTO { Id = roleId };

            _mockUserManagementDa.Setup(x => x.GetRole(roleId)).ReturnsAsync(role);
            _mockMapper.Setup(m => m.Map<RoleManagementDTO>(It.IsAny<IdentityRole>()))
                      .Throws(new Exception("Mapping failed"));

            // Act
            var result = await _userManagementService.FillRoleManagementDto(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }
        #endregion
        #endregion
    }
}
