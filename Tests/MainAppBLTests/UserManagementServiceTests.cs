using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using MainApp.MVC.Mappers;
using Microsoft.AspNetCore.Identity;
using Moq;
using SD;
using Services;
using Services.Interfaces.Services;

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
            var appUser = new ApplicationUser() { Id = "TestId", UserName = "TestUser", Email = "testemail@testmailserver.test"};
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
            Assert.Equal("RolesInsert must not be null", result.ErrMsg);
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
        public async Task AddUserClaims_ReturnsOk_SuccessfullyAddsClaims()
        {
            // Arrange
            string appUserId = "user123";
            List<string> claimsInsert = new List<string> { "claim1", "claim2" };
            _mockMapper.Setup(m => m.Map<IdentityUserClaim<string>>(It.IsAny<string>()))
                       .Returns((string s) => new IdentityUserClaim<string> { ClaimValue = s });
            _mockUserManagementDa.Setup(d => d.AddClaimForUser(It.IsAny<IdentityUserClaim<string>>()))
                                 .Returns(Task.CompletedTask);

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
            _mockMapper.Setup(m => m.Map<IdentityUserClaim<string>>(It.IsAny<string>()))
                       .Throws(new Exception("Test exception"));

            // Act
            var result = await _userManagementService.AddUserClaims(appUserId, claimsInsert, "type");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ExObj);
            Assert.Equal("Test exception", result.ErrMsg);
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
            _mockMapper.Setup(m => m.Map<IdentityUserRole<string>>(It.IsAny<string>()))
                       .Returns((string s) => new IdentityUserRole<string> { UserId = appUserId, RoleId = s });
            _mockUserManagementDa.Setup(d => d.AddRoleForUser(It.IsAny<IdentityUserRole<string>>()))
                                 .Returns(Task.FromResult(new IdentityUserRole<string>()));

            // Act
            var result = await _userManagementService.AddUserRoles(appUserId, rolesInsert);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManagementDa.Verify(da => da.AddRoleForUser(It.IsAny<IdentityUserRole<string>>()), Times.Exactly(rolesInsert.Count));
        }

        [Fact]
        public async Task AddUserRoles_HandlesException()
        {
            // Arrange
            string appUserId = "user123";
            List<string> rolesInsert = new List<string> { "Admin" };
            _mockMapper.Setup(m => m.Map<IdentityUserRole<string>>(It.IsAny<string>()))
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
            Assert.Equal(expectedLanguage, result);
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
    }
}
