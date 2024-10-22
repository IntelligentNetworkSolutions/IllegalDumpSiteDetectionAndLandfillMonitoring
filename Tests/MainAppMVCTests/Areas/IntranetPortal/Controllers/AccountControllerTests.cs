using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using Entities;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SD;
using Services.Interfaces.Services;
using System.Security.Claims;

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

        [Fact]
        public async Task Login_ReturnsViewIfNotAuthenticated()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsViewWithModelError_IfUsernameOrPasswordIsEmpty()
        {
            // Act
            var result = await _controller.Login("", "", false, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("msgError"));
        }

        [Fact]
        public async Task Login_ReturnsViewWithModelError_IfUserIsNull()
        {
            // Arrange
            var username = "nonexistentUser";
            var password = "somePassword";
            var remember = false;
            string returnUrl = null;

            var userManagerMock = MockUserManager<ApplicationUser>();
            userManagerMock
                .Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            // Act
            var result = await _controller.Login(username, password, remember, returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("msgError"));
            Assert.Equal("Wrong username", viewResult.ViewData.ModelState["msgError"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_ReturnsViewWithModelError_IfPasswordIsIncorrect()
        {
            // Arrange
            var username = "existingUser";
            var password = "wrongPassword";
            var remember = false;
            string returnUrl = null;

            var userManagerMock = MockUserManager<ApplicationUser>();

            var user = new ApplicationUser { UserName = username, Id = "testUserId" };

            userManagerMock
                .Setup(um => um.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(false);

            // Create the controller with the mocked UserManager
            var controller = new AccountController(
                userManagerMock.Object,
                null,
                null,
                null,
                null,
                null,
                null
            );

            // Act
            var result = await controller.Login(username, password, remember, returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("msgError"));
            Assert.Equal("Wrong password", viewResult.ViewData.ModelState["msgError"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_ReturnsViewWithModelError_IfUserIsInactive()
        {
            // Arrange
            var username = "inactiveUser";
            var password = "somePassword";
            var remember = false;
            string returnUrl = null;

            var userManagerMock = MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                UserName = username,
                IsActive = false
            };

            userManagerMock
                .Setup(um => um.FindByNameAsync(username))
                .ReturnsAsync(user);
            userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new AccountController(
                userManagerMock.Object,
                null,
                null,
                null,
                null,
                null,
                null
            );

            // Act
            var result = await controller.Login(username, password, remember, returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("msgError"));
            Assert.Equal("Unable to log in because this user account is currently disabled!", viewResult.ViewData.ModelState["msgError"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_RedirectsToHome_WhenLoginIsSuccessful()
        {
            // Arrange
            var username = "validUser";
            var password = "validPassword";
            var remember = false;
            string returnUrl = null;

            // Create a mock UserManager
            var userManagerMock = MockUserManager<ApplicationUser>();

            // Simulate finding a user with the specified username
            var user = new ApplicationUser
            {
                UserName = username,
                IsActive = true, // Simulate an active user
                Id = "testUserId",
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "test@test.com"
            };

            // Setup to return the user when FindByNameAsync is called with the correct username
            userManagerMock
                .Setup(um => um.FindByNameAsync(username))
                .ReturnsAsync(user);

            // Setup to return true for password check
            userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            // Mock other dependencies if necessary
            var userManagementServiceMock = new Mock<IUserManagementService>();

            // Setup for GetUserClaims to return an empty list
            userManagementServiceMock.Setup(s => s.GetUserClaims(user.Id))
                .ReturnsAsync(new List<UserClaimDTO>());

            // Mock for GetSuperAdminUserBySpecificClaim
            var superAdmin = new UserDTO { UserName = "superadmin" }; // Mock a super admin user
            userManagementServiceMock.Setup(s => s.GetSuperAdminUserBySpecificClaim())
                .ReturnsAsync(superAdmin);

            // Mock the HttpContext and SignInAsync method
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            var httpContextMock = new Mock<HttpContext>();
            var authenticationServiceMock = new Mock<IAuthenticationService>();

            // Setup the HttpContext.SignInAsync method
            authenticationServiceMock
                .Setup(s => s.SignInAsync(
                    httpContextMock.Object,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask); // Mimic the sign-in process

            httpContextMock
                .Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            // Mock IUrlHelper and set it up
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/Home/Index");

            // Create the controller with the mocked HttpContext and UrlHelper
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var controller = new AccountController(
                userManagerMock.Object,
                null, // Mock other dependencies as necessary
                null,
                null,
                null,
                userManagementServiceMock.Object,
                null
            )
            {
                ControllerContext = controllerContext,
                Url = urlHelperMock.Object // Set the mock UrlHelper
            };

            // Act
            var result = await controller.Login(username, password, remember, returnUrl);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); // Check that it redirects to the Index action
            Assert.Equal("Home", redirectResult.ControllerName); // Check that it redirects to the Home controller
            Assert.Equal("Common", redirectResult.RouteValues["area"]); // Check the area value if applicable
        }


        [Fact]
        public void GetUserIdentityClaims_ReturnsEmptyList_WhenUserIsNull()
        {
            // Act
            var result = _controller.GetUserIdentityClaims(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Ensure the result is an empty list
        }
        [Fact]
        public void GetUserIdentityClaims_ReturnsCorrectClaims_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@example.com",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                UserName = "TestUsername",
                Id = "123"
            };

            // Act
            var result = _controller.GetUserIdentityClaims(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count); // Ensure 5 claims are returned

            Assert.Contains(result, c => c.Type == ClaimTypes.Name && c.Value == user.Email);
            Assert.Contains(result, c => c.Type == "FirsName" && c.Value == user.FirstName);
            Assert.Contains(result, c => c.Type == "Username" && c.Value == user.UserName);
            Assert.Contains(result, c => c.Type == "UserId" && c.Value == user.Id);
            Assert.Contains(result, c => c.Type == "LastName" && c.Value == user.LastName);
        }




        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }


    }
}
