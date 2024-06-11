using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using Entities;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SD;
using Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAppSettingsAccessor> _appSettingsAccessorMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userManagerMock = MockUserManager<ApplicationUser>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _configurationMock = new Mock<IConfiguration>();
            _appSettingsAccessorMock = new Mock<IAppSettingsAccessor>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", "test-user-id")
            }, "mock"));

            var httpContext = new DefaultHttpContext() { User = user };
            _controller = new AccountController(userManagerMock.Object, null, null, _configurationMock.Object, null, _userManagementServiceMock.Object, _appSettingsAccessorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }


        [Fact]
        public async Task UpdatePassword_ReturnsJsonResult_WhenUserIdIsNull()
        {
            // Act
            var result = await _controller.UpdatePassword(null, "password", "password", null);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { wrongUserId = true });
            var actualJson = JsonConvert.SerializeObject(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsJsonResult_WhenPasswordFieldsAreEmpty()
        {
            // Act
            var result = await _controller.UpdatePassword(null, null, null, "userId");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { passwordFieldsEmpty = true });
            var actualJson = JsonConvert.SerializeObject(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsJsonResult_WhenPasswordsDoNotMatch()
        {
            // Act
            var result = await _controller.UpdatePassword("currentPassword", "password", "differentPassword", "userId");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { passwordMissmatch = true });
            var actualJson = JsonConvert.SerializeObject(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsJsonResult_WhenCurrentPasswordFails()
        {
            // Arrange
            _userManagementServiceMock.Setup(x => x.UpdateUserPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Fail("Error"));

            // Act
            var result = await _controller.UpdatePassword("currentPassword", "password", "password", "userId");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { currentPasswordFailed = true });
            var actualJson = JsonConvert.SerializeObject(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsJsonResult_WhenPasswordUpdatedSuccessfully()
        {
            // Arrange
            _userManagementServiceMock.Setup(x => x.UpdateUserPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.UpdatePassword("currentPassword", "password", "password", "userId");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { passwordUpdatedSuccessfully = true });
            var actualJson = JsonConvert.SerializeObject(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task SetCulture_ReturnsRedirect_WhenUserIdIsNull()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/404");

            // Act
            var result = await _controller.SetCulture("en", "/home");

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error/404", redirectResult.Url);
        }

        [Fact]
        public async Task SetCulture_ReturnsNotFound_WhenUserIdIsNullAndErrorPathIsNull()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            // Act
            var result = await _controller.SetCulture("en", "/home");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SetCulture_ReturnsRedirect_WhenUserNotFound()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("UserId", "1") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            _userManagementServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync((UserDTO)null);
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/Error/404");

            // Act
            var result = await _controller.SetCulture("en", "/home");

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error/404", redirectResult.Url);
        }

        [Fact]
        public async Task SetCulture_ReturnsNotFound_WhenUserNotFoundAndErrorPathIsNull()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("UserId", "1") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            _userManagementServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync((UserDTO)null);
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            // Act
            var result = await _controller.SetCulture("en", "/home");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SetCulture_HandlesErrorInAddingLanguageClaim()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("UserId", "1") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            var user = new UserDTO { Id = "1" };
            _userManagementServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync(user);
            _userManagementServiceMock.Setup(s => s.AddLanguageClaimForUser("1", "en")).ReturnsAsync(ResultDTO.Fail("Error"));

            // Act
            var result = await _controller.SetCulture("en", "/home");

            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task GetAllLanguages_ReturnsData_WhenResultIsSuccess()
        {
            // Arrange
            var expectedLanguages = "en,fr,de";
            var resultDto = ResultDTO<string?>.Ok(expectedLanguages);
            _appSettingsAccessorMock
                .Setup(a => a.GetApplicationSettingValueByKey<string?>("MainApplicationLanguages", null))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAllLanguages();

            // Assert
            Assert.Equal(expectedLanguages, result);
        }

        [Fact]
        public async Task GetAllLanguages_ReturnsNull_WhenResultIsFailure()
        {
            // Arrange
            var resultDto = ResultDTO<string?>.Fail("Error retrieving setting");
            _appSettingsAccessorMock
                .Setup(a => a.GetApplicationSettingValueByKey<string?>("MainApplicationLanguages", null))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAllLanguages();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MyProfile_ReturnsRedirect_WhenUserIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext.HttpContext.User = user;
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.MyProfile();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task MyProfile_ReturnsNotFound_WhenUserIdIsNullAndErrorPathIsNotConfigured()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext.HttpContext.User = user;
            _configurationMock.Setup(c => c["ErrorViewsPath:Error404"]).Returns(string.Empty);

            // Act
            var result = await _controller.MyProfile();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
             

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }
    }
}
