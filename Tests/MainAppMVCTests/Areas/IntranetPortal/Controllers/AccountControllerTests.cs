using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SD;
using Services.Interfaces;
using Services.Interfaces.Services;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IIntranetPortalUsersTokenDa> _intranetPortalUsersTokenDaMock;
        private readonly Mock<IForgotResetPasswordService> _forgotResetPasswordServiceMock;
        private readonly Mock<IAppSettingsAccessor> _appSettingsAccessorMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userManagerMock = MockUserManager<ApplicationUser>();
            _intranetPortalUsersTokenDaMock = new Mock<IIntranetPortalUsersTokenDa>();
            _forgotResetPasswordServiceMock = new Mock<IForgotResetPasswordService>();
            _appSettingsAccessorMock = new Mock<IAppSettingsAccessor>();
            _configurationMock = new Mock<IConfiguration>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _userManagementServiceMock = new Mock<IUserManagementService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim("UserId", "test-user-id")
            }, "mock"));

            var httpContext = new DefaultHttpContext() { User = user };

            _controller = new AccountController(
                _userManagerMock.Object,
                _intranetPortalUsersTokenDaMock.Object,
                _forgotResetPasswordServiceMock.Object,
                _configurationMock.Object,
                _webHostEnvironmentMock.Object,
                _userManagementServiceMock.Object,
                _appSettingsAccessorMock.Object)
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
        public async Task MyProfile_ReturnsView_WithCorrectModel_WhenUserIdIsValid()
        {
            // Arrange
            var userId = "validUserId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("UserId", userId) }));
            _controller.ControllerContext.HttpContext.User = user;

            var appUser = new UserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserName = "johndoe",
                Id = userId
            };

            _userManagementServiceMock.Setup(um => um.GetUserById(userId)).ReturnsAsync(appUser);
            _userManagementServiceMock.Setup(um => um.GetPreferredLanguageForUser(userId)).ReturnsAsync("English");
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                                   .ReturnsAsync(ResultDTO<int>.Ok(8));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Act
            var result = await _controller.MyProfile();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MyProfileViewModel>(viewResult.Model);
            Assert.Equal(appUser.FirstName, model.FirstName);
            Assert.Equal(appUser.LastName, model.LastName);
            Assert.Equal(appUser.Email, model.Email);
            Assert.Equal(appUser.UserName, model.Username);
            Assert.Equal(appUser.Id, model.UserId);
            Assert.Equal(8, model.PasswordMinLength);
            Assert.True(model.PasswordMustHaveLetters);
            Assert.True(model.PasswordMustHaveNumbers);
            Assert.Equal("English", model.PreferredLanguage);
        }

        //check this later
        [Fact]
        public async Task MyProfile_ThrowsException_WhenUserManagementServiceReturnsNull()
        {
            // Arrange
            var userId = "validUserId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("UserId", userId) }));
            _controller.ControllerContext.HttpContext.User = user;

            // Set up the application settings accessor to return valid values
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                       .ReturnsAsync(ResultDTO<int>.Ok(8));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Mock the user management service to throw an exception
            _userManagementServiceMock.Setup(um => um.GetUserById(userId)).ThrowsAsync(new Exception("User not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.MyProfile());
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

            var userManagerMock = MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                UserName = username,
                IsActive = true,
                Id = "testUserId",
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "test@test.com"
            };

            userManagerMock
                .Setup(um => um.FindByNameAsync(username))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            var userManagementServiceMock = new Mock<IUserManagementService>();

            userManagementServiceMock.Setup(s => s.GetUserClaims(user.Id))
                .ReturnsAsync(new List<UserClaimDTO>());

            var superAdmin = new UserDTO { UserName = "superadmin" };
            userManagementServiceMock.Setup(s => s.GetSuperAdminUserBySpecificClaim())
                .ReturnsAsync(superAdmin);

            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            var httpContextMock = new Mock<HttpContext>();
            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock
                .Setup(s => s.SignInAsync(
                    httpContextMock.Object,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            httpContextMock
                .Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/Home/Index");

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var controller = new AccountController(
                userManagerMock.Object,
                null,
                null,
                null,
                null,
                userManagementServiceMock.Object,
                null
            )
            {
                ControllerContext = controllerContext,
                Url = urlHelperMock.Object
            };

            // Act
            var result = await controller.Login(username, password, remember, returnUrl);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            Assert.Equal("Common", redirectResult.RouteValues["area"]);
        }


        [Fact]
        public void GetUserIdentityClaims_ReturnsEmptyList_WhenUserIsNull()
        {
            // Act
            var result = _controller.GetUserIdentityClaims(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
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
            Assert.Equal(5, result.Count);

            Assert.Contains(result, c => c.Type == ClaimTypes.Name && c.Value == user.Email);
            Assert.Contains(result, c => c.Type == "FirsName" && c.Value == user.FirstName);
            Assert.Contains(result, c => c.Type == "Username" && c.Value == user.UserName);
            Assert.Contains(result, c => c.Type == "UserId" && c.Value == user.Id);
            Assert.Contains(result, c => c.Type == "LastName" && c.Value == user.LastName);
        }


        //[Fact]
        //public async Task ForgotPassword_UserFoundByEmail_EmailSent_ReturnsResetPasswordConfirmationView()
        //{
        //    // Arrange
        //    var model = new IntranetUsersForgotPasswordViewModel { UsernameOrEmail = "validUser@test.com" };

        //    var user = new ApplicationUser
        //    {
        //        Email = "validUser@test.com",
        //        UserName = "validUser",
        //        Id = "userId123"
        //    };

        //    var token = Guid.NewGuid().ToString();
        //    var domain = "example.com";
        //    var webRootPath = "wwwroot";
        //    var url = $"https://{domain}/IntranetPortal/Account/ResetPassword?userId={user.Id}&token={token}";

        //    var userManagerMock = MockUserManager<ApplicationUser>();

        //    userManagerMock
        //        .Setup(um => um.FindByEmailAsync(model.UsernameOrEmail))
        //        .ReturnsAsync(user);


        //    // Mock the token creation
        //    _intranetPortalUsersTokenDaMock
        //        .Setup(da => da.CreateIntranetPortalUserToken(token, user.Id))
        //        .ReturnsAsync(1);

        //    // Mock sending the email
        //    _forgotResetPasswordServiceMock
        //        .Setup(s => s.SendPasswordResetEmail(user.Email, user.UserName, url, webRootPath))
        //        .ReturnsAsync(true);

        //    // Mock the configuration settings
        //    _configurationMock
        //        .Setup(c => c["DomainSettings:MainDomain"])
        //        .Returns(domain);

        //    _webHostEnvironmentMock
        //        .Setup(he => he.WebRootPath)
        //        .Returns(webRootPath);


        //    // Act
        //    var result = await _controller.ForgotPassword(model);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("ResetPasswordConfirmation", viewResult.ViewName);
        //}

        [Fact]
        public async Task ResetPassword_TokenIsNotUsed_ReturnsViewWithModel()
        {
            // Arrange
            var userId = "testUserId";
            var token = "validToken";
            var mockTokenCheck = true;
            _intranetPortalUsersTokenDaMock.Setup(x => x.IsTokenNotUsed(token, userId)).ReturnsAsync(mockTokenCheck);
            _configurationMock.Setup(c => c["ErrorViewsPath:Error403"]).Returns("");
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                       .ReturnsAsync(ResultDTO<int>.Ok(10));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                                   .ReturnsAsync(ResultDTO<bool>.Ok(true));


            // Act
            var result = await _controller.ResetPassword(userId, token);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IntranetUsersResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal(token, model.Token);
        }
        [Fact]
        public async Task ResetPassword_TokenIsUsed_RedirectsToError403()
        {
            // Arrange
            var userId = "testUserId";
            var token = "usedToken";
            var mockTokenCheck = false;

            _intranetPortalUsersTokenDaMock.Setup(x => x.IsTokenNotUsed(token, userId)).ReturnsAsync(mockTokenCheck);
            _configurationMock.Setup(c => c["ErrorViewsPath:Error403"]).Returns("/Error403");


            // Act
            var result = await _controller.ResetPassword(userId, token);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error403", redirectResult.Url);
        }

        [Fact]
        public async Task ResetPassword_TokenIsUsed_NoErrorPath_Retuns403Status()
        {
            // Arrange
            var userId = "testUserId";
            var token = "usedToken";
            var mockTokenCheck = false;

            _intranetPortalUsersTokenDaMock.Setup(x => x.IsTokenNotUsed(token, userId)).ReturnsAsync(mockTokenCheck);
            _configurationMock.Setup(c => c["ErrorViewsPath:Error403"]).Returns("");

            // Act
            var result = await _controller.ResetPassword(userId, token);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, statusCodeResult.StatusCode);
        }


        [Fact]
        public async Task ResetPassword_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new IntranetUsersResetPasswordViewModel
            {
                UserId = "userId",
                Token = "token",
                NewPassword = "short" // Invalid password for testing
            };
            _controller.ModelState.AddModelError("NewPassword", "The password is too short.");

            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<int>("PasswordMinLength", It.IsAny<int>()))
                .ReturnsAsync(ResultDTO<int>.Ok(10));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", It.IsAny<bool>()))
                .ReturnsAsync(ResultDTO<bool>.Ok(true));
            _appSettingsAccessorMock.Setup(x => x.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", It.IsAny<bool>()))
                .ReturnsAsync(ResultDTO<bool>.Ok(true));

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<IntranetUsersResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(10, returnedModel.PasswordMinLength);
            Assert.True(returnedModel.PasswordMustHaveLetters);
            Assert.True(returnedModel.PasswordMustHaveNumbers);
        }

        [Fact]
        public async Task ResetPassword_RedirectsToLogin_WhenPasswordIsUpdatedSuccessfully()
        {
            // Arrange
            var model = new IntranetUsersResetPasswordViewModel
            {
                UserId = "userId",
                Token = "token",
                NewPassword = "ValidPassword123" // Valid password
            };

            var user = new ApplicationUser { Id = "userId" };
            _intranetPortalUsersTokenDaMock.Setup(da => da.GetUser(model.UserId)).ReturnsAsync(user);
            _intranetPortalUsersTokenDaMock.Setup(da => da.UpdateAndHashUserPassword(user, model.NewPassword)).ReturnsAsync(1);
            _intranetPortalUsersTokenDaMock.Setup(da => da.UpdateIsTokenUsedForUser(model.Token, model.UserId)).ReturnsAsync(1);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_RedirectsToLogin_WhenUserNotFound()
        {
            // Arrange
            var model = new IntranetUsersResetPasswordViewModel
            {
                UserId = "invalidUserId",
                Token = "token",
                NewPassword = "ValidPassword123" // Valid password
            };

            _intranetPortalUsersTokenDaMock.Setup(da => da.GetUser(model.UserId)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }


        [Fact]
        public async Task ResetPassword_ThrowsException_WhenUpdatePasswordFails()
        {
            // Arrange
            var model = new IntranetUsersResetPasswordViewModel
            {
                UserId = "userId",
                Token = "token",
                NewPassword = "ValidPassword123" // Valid password
            };

            var user = new ApplicationUser { Id = "userId" };
            _intranetPortalUsersTokenDaMock.Setup(da => da.GetUser(model.UserId)).ReturnsAsync(user);
            _intranetPortalUsersTokenDaMock.Setup(da => da.UpdateAndHashUserPassword(user, model.NewPassword)).ThrowsAsync(new Exception("Error updating password"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.ResetPassword(model));
        }



        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }


    }
}
