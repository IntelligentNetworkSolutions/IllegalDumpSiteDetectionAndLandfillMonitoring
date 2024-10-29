using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.Helpers;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using SD;
using Services.Interfaces.Services;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class UserManagementControllerTests
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly IConfiguration _configuration;
        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mapper _mapper;

        private readonly UserManagementController _controller;

        public UserManagementControllerTests()
        {
            var configDict = new Dictionary<string, string>
            {
                {"AppSettings:Modules:0", "UserManagement"},
                {"AppSettings:Modules:1", "AuditLog"},
                {"AppSettings:Modules:2", "Admin"},
                {"AppSettings:Modules:3", "SpecialActions"}
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(configDict);
            IConfigurationRoot config = configBuilder.Build();
            _configuration = config;
            _modulesAndAuthClaimsHelper = new ModulesAndAuthClaimsHelper(_configuration);
            _mockUserManagementService = new Mock<IUserManagementService>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<UserManagementProfile>());
            _mapper = new Mapper(mapperConfig);
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _controller = new UserManagementController(_modulesAndAuthClaimsHelper, _configuration,
                _mockUserManagementService.Object, _mockAppSettingsAccessor.Object, _mapper);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            var users = new List<UserDTO>() { new UserDTO() { Id = "123", UserName = "TestUser", Email = "testuser@gmail.com" } };
            var roles = new List<RoleDTO>();
            var userRoles = new List<UserRoleDTO>();
            _mockUserManagementService.Setup(x => x.GetAllIntanetPortalUsers()).ReturnsAsync(users);
            _mockUserManagementService.Setup(x => x.GetAllRoles()).ReturnsAsync(roles);
            _mockUserManagementService.Setup(x => x.GetAllUserRoles()).ReturnsAsync(userRoles);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task FillUserManagementCreateUserViewModel_ReturnsOkResult_WithValidModel()
        {
            // Arrange
            var dto = new UserManagementDTO();
            var viewModel = new UserManagementCreateUserViewModel();
            viewModel.IsActive = false;
            viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            var claims = new List<AuthClaim>();

            _mockUserManagementService.Setup(s => s.FillUserManagementDto(null))
                .ReturnsAsync(ResultDTO<UserManagementDTO>.Ok(dto));

            //_mockMapper.Setup(m => m.Map<UserManagementCreateUserViewModel>(It.IsAny<UserManagementDTO>()))
            //    .Returns(viewModel);

            //_mockModulesAndAuthClaimsHelper.Setup(h => h.GetAuthClaims())
            //  .ReturnsAsync(claims);

            // Act
            var result = await _controller.FillUserManagementCreateUserViewModel();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equivalent(viewModel, result.Data);
        }

        [Fact]
        public async Task FillUserManagementCreateUserViewModel_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            _mockUserManagementService.Setup(s => s.FillUserManagementDto(null))
                .ReturnsAsync(ResultDTO<UserManagementDTO>.Fail("Error message"));

            // Act
            var result = await _controller.FillUserManagementCreateUserViewModel();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error message", result.ErrMsg);
        }

        [Fact]
        public async Task FillUserManagementCreateUserViewModel_ReturnsExceptionFailResult_WhenExceptionThrown()
        {
            // Arrange
            _mockUserManagementService.Setup(s => s.FillUserManagementDto(null))
                .ThrowsAsync(new Exception("Exception message"));

            // Act
            var result = await _controller.FillUserManagementCreateUserViewModel();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Exception message", result.ErrMsg);
        }

        [Fact]
        public async Task GetViewWithUserManagementCreateVM_WhenResultFillModelIsNotSuccess_ShouldAddModelError()
        {
            // Arrange
            _mockUserManagementService.Setup(x => x.FillUserManagementDto(null))
                                        .ReturnsAsync(ResultDTO<UserManagementDTO>.Fail("Error message"));

            // Act
            var result = await _controller.GetViewWithUserManagementCreateVM();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("ModelOnly"));
            Assert.Equal("Error message", _controller.ModelState["ModelOnly"].Errors.First().ErrorMessage);
        }

        [Fact]
        public async Task GetViewWithUserManagementCreateVM_WhenResultFillModelIsSuccess_ShouldReturnViewResult()
        {
            // Arrange
            _mockUserManagementService.Setup(x => x.FillUserManagementDto(null))
                                        .ReturnsAsync(ResultDTO<UserManagementDTO>.Ok(new UserManagementDTO()));

            //mapperMock
            //  .Setup(x => x.Map<UserManagementCreateUserViewModel>(It.IsAny<UserManagementDTO>()))
            //.Returns(new UserManagementCreateUserViewModel());

            // Act
            var result = await _controller.GetViewWithUserManagementCreateVM();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateUser_WhenModelStateIsNotValid_ShouldReturnViewResult()
        {
            // Arrange
            _mockUserManagementService.Setup(x => x.FillUserManagementDto(null))
                                        .ReturnsAsync(ResultDTO<UserManagementDTO>.Ok(new UserManagementDTO()));
            _controller.ModelState.AddModelError("Test", "Test error message");

            // Act
            var result = await _controller.CreateUser();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateUser_WhenModelStateIsValid_ShouldReturnRedirectToActionResult()
        {
            // Arrange
            UserManagementDTO dto = new UserManagementDTO()
            {
                Roles = new List<RoleDTO>(),
                RolesInsert = new List<string>(),
                RoleClaims = new List<RoleClaimDTO>(),
                ClaimsInsert = new List<string>(),
                Password = "password123",
                ConfirmPassword = "password123",
                PasswordMinLength = 8,
                PasswordMustHaveNumbers = true,
                PasswordMustHaveLetters = true,
                AllUsers = new List<UserDTO>(),
                Email = "example@example.com",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                PhoneNumber = "1234567890",
                UserName = "johndoe123"
            };

            UserManagementCreateUserViewModel viewModel = new UserManagementCreateUserViewModel()
            {
                Roles = new List<RoleDTO>(),
                RolesInsert = new List<string>(),
                Claims = new List<AuthClaim>(),
                RoleClaims = new List<RoleClaimDTO>(),
                ClaimsInsert = new List<string>(),
                Password = "password123",
                ConfirmPassword = "password123",
                PasswordMinLength = 8,
                PasswordMustHaveNumbers = true,
                PasswordMustHaveLetters = true,
                AllUsers = new List<UserDTO>(),
                Email = "example@example.com",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                PhoneNumber = "1234567890",
                UserName = "johndoe123"
            };

            _mockUserManagementService.Setup(x => x.FillUserManagementDto(null))
                                        .ReturnsAsync(ResultDTO<UserManagementDTO>.Ok(dto));

            //_mockUserManagementService.Setup(x => x.AddUser(an))
            _mockUserManagementService.Setup(x => x.AddUser(It.IsAny<UserManagementDTO>())).ReturnsAsync(new ResultDTO(true, null, null));

            // Act
            var result = await _controller.CreateUser(viewModel);

            // Assert
            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task EditUser_Get_ReturnsViewResult_WhenIdIsValid()
        {
            // Arrange
            string userId = "testUserId";
            var dto = new UserManagementDTO { Id = userId };
            _mockUserManagementService.Setup(x => x.FillUserManagementDto(It.IsAny<UserManagementDTO>()))
                .ReturnsAsync(ResultDTO<UserManagementDTO>.Ok(dto));

            // Act
            var result = await _controller.EditUser(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<UserManagementEditUserViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task EditUser_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new UserManagementEditUserViewModel
            {
                Id = "testUserId",
                Email = "test@example.com",
                UserName = "testuser"
            };
            _mockUserManagementService.Setup(x => x.UpdateUser(It.IsAny<UserManagementDTO>()))
                .ReturnsAsync(new ResultDTO(true, null, null));

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateRole_ReturnsViewResult_WithViewModel()
        {
            // Act
            var result = await _controller.CreateRole();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<UserManagementCreateRoleViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task CreateRole_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new UserManagementCreateRoleViewModel
            {
                Name = "TestRole",
                ClaimsInsert = new List<string> { "Claim1", "Claim2" }
            };
            _mockUserManagementService.Setup(x => x.AddRole(It.IsAny<RoleManagementDTO>()))
                .ReturnsAsync(new ResultDTO(true, null, null));

            // Act
            var result = await _controller.CreateRole(viewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditRole_ReturnsViewResult_WhenIdIsValid()
        {
            // Arrange
            string roleId = "testRoleId";
            var dto = new RoleManagementDTO { Id = roleId };
            _mockUserManagementService.Setup(x => x.FillRoleManagementDto(It.IsAny<RoleManagementDTO>()))
                .ReturnsAsync(ResultDTO<RoleManagementDTO>.Ok(dto));

            // Act
            var result = await _controller.EditRole(roleId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<UserManagementEditRoleViewModel>(viewResult.Model);
        }

        //[Fact]
        //public async Task EditRole_ReturnsViewResult_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var viewModel = new UserManagementEditRoleViewModel { Id = "testRoleId" };
        //    _controller.ModelState.AddModelError("SomeError", "Invalid Model");

        //    var dto = new RoleManagementDTO { Id = viewModel.Id };
        //    var resultFillModel = ResultDTO<UserManagementEditRoleViewModel>.Ok(viewModel);
        //    _mockUserManagementService.Setup(x => x.FillRoleManagementDto(dto))
        //        .ReturnsAsync(ResultDTO<RoleManagementDTO>.Ok(dto));

        //    // Act
        //    var result = await _controller.EditRole(viewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<UserManagementEditRoleViewModel>(viewResult.Model);
        //    Assert.Equal(viewModel.Id, model.Id);
        //}

        //[Fact]
        //public async Task EditRole_ReturnsViewResult_WhenUpdateRoleFails()
        //{
        //    // Arrange
        //    var viewModel = new UserManagementEditRoleViewModel { Id = "testRoleId" };
        //    var roleManagementDTO = new RoleManagementDTO { Id = viewModel.Id };
        //    var resultUpdate = ResultDTO.Fail("Error updating role");
        //    _mapper.Setup(x => x.Map<RoleManagementDTO>(viewModel)).Returns(roleManagementDTO);
        //    _mockUserManagementService.Setup(x => x.UpdateRole(roleManagementDTO)).ReturnsAsync(resultUpdate);

        //    var dto = new RoleManagementDTO { Id = viewModel.Id };
        //    var resultFillModel = ResultDTO<UserManagementEditRoleViewModel>.Ok(viewModel);
        //    _mockUserManagementService.Setup(x => x.FillRoleManagementDto(dto))
        //        .ReturnsAsync(ResultDTO<RoleManagementDTO>.Ok(dto));

        //    // Act
        //    var result = await _controller.EditRole(viewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.NotNull(viewResult.Model);
        //    Assert.IsType<UserManagementEditRoleViewModel>(viewResult.Model);
        //    Assert.Equal(viewModel.Id, ((UserManagementEditRoleViewModel)viewResult.Model).Id);
        //}

        //[Fact]
        //public async Task EditRole_RedirectsToIndex_WhenUpdateRoleSucceeds()
        //{
        //    // Arrange
        //    var viewModel = new UserManagementEditRoleViewModel { Id = "testRoleId" };
        //    var roleManagementDTO = new RoleManagementDTO { Id = viewModel.Id };
        //    var resultUpdate = ResultDTO.Ok();

        //    //_mapper.Setup(x => x.Map<RoleManagementDTO>(viewModel)).Returns(roleManagementDTO);
        //    _mockUserManagementService.Setup(x => x.UpdateRole(roleManagementDTO)).ReturnsAsync(resultUpdate);

        //    // Act
        //    var result = await _controller.EditRole(viewModel);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //}

        [Fact]
        public async Task DeleteRole_ReturnsRoleDTO_WhenIdIsValid()
        {
            // Arrange
            string roleId = "testRoleId";
            var expectedRole = new RoleDTO { Id = roleId, Name = "TestRole" };
            _mockUserManagementService.Setup(x => x.GetRoleById(roleId))
                .ReturnsAsync(expectedRole);

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRole.Id, result.Id);
            Assert.Equal(expectedRole.Name, result.Name);
        }

        [Fact]
        public void GetModules_ShouldReturnCorrectModules()
        {
            // Arrange
            var expectedModules = new[] { "UserManagement", "AuditLog", "Admin", "SpecialActions" };

            // Act
            var activeModules = _modulesAndAuthClaimsHelper.GetModules().Result;

            // Assert
            Assert.Equal(expectedModules.Length, activeModules.Count);
            Assert.All(expectedModules, module =>
                Assert.Contains(activeModules, m => m.Value == module));
        }

        [Fact]
        public void HasModule_ShouldReturnTrue_WhenModuleIsActive()
        {
            // Arrange
            var moduleToCheck = new Module { Value = "UserManagement" };

            // Act
            var result = _modulesAndAuthClaimsHelper.HasModule(moduleToCheck);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasModule_ShouldReturnFalse_WhenModuleIsNotActive()
        {
            // Arrange
            var moduleToCheck = new Module { Value = "NonExistentModule" };

            // Act
            var result = _modulesAndAuthClaimsHelper.HasModule(moduleToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteRoleConfirmed_CallsDeleteRoleAndRedirectsToIndex()
        {
            // Arrange
            var roleId = "testRoleId";
            _mockUserManagementService.Setup(s => s.DeleteRole(roleId))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteRoleConfirmed(roleId);

            // Assert
            _mockUserManagementService.Verify(s => s.DeleteRole(roleId), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteUser_ReturnsUserDTO_WhenUserExists()
        {
            // Arrange
            var userId = "testUserId";
            var expectedUser = new UserDTO { Id = userId, UserName = "TestUser", Email = "testuser@example.com" };
            _mockUserManagementService.Setup(s => s.GetUserById(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            _mockUserManagementService.Verify(s => s.GetUserById(userId), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.UserName, result.UserName);
            Assert.Equal(expectedUser.Email, result.Email);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistentUserId";
            _mockUserManagementService.Setup(s => s.GetUserById(userId))
                .ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            _mockUserManagementService.Verify(s => s.GetUserById(userId), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUserConfirmed_WhenCheckUserBeforeDeleteFails_ShouldReturnJsonWithErrorMessage()
        {
            // Arrange
            string userId = "testUserId";
            var errorMessage = "Error retrieving data";
            _mockUserManagementService.Setup(x => x.CheckUserBeforeDelete(userId))
                .ReturnsAsync(ResultDTO<bool>.Fail(errorMessage));

            // Act
            var result = await _controller.DeleteUserConfirmed(userId) as JsonResult;

            // Assert
            Assert.NotNull(result); // Ensure that the result is not null
            var jsonData = JObject.FromObject(result.Value); // Convert to JObject for easy access

            // Check for the error message in the JSON result
            Assert.True(jsonData.ContainsKey("errorRetrievingData"));
            Assert.Equal(errorMessage, (string)jsonData["errorRetrievingData"]); // Assert the error message matches
        }

        [Fact]
        public async Task DeleteUserConfirmed_WhenUserHasEntry_ShouldReturnJsonWithUserHasEntry()
        {
            // Arrange
            string userId = "testUserId";
            var resultDTO = ResultDTO<bool>.Ok(true); // This represents the user having entries
            _mockUserManagementService.Setup(x => x.CheckUserBeforeDelete(userId))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.DeleteUserConfirmed(userId) as JsonResult;

            // Assert
            Assert.NotNull(result); // Ensure that the result is not null
            var jsonData = JObject.FromObject(result.Value);

            // Check if the JSON contains the expected key and value
            Assert.True(jsonData.ContainsKey("userHasEntry"));

            // Access the nested properties
            var userHasEntryData = jsonData["userHasEntry"] as JObject; // Cast to JObject to access properties
            Assert.NotNull(userHasEntryData); // Ensure userHasEntry object is not null
            Assert.True((bool)userHasEntryData["IsSuccess"]); // Check IsSuccess is true
            Assert.True((bool)userHasEntryData["Data"]); // Check Data is true
        }





        [Fact]
        public async Task DeleteUserConfirmed_WhenUserDeletedSuccessfully_ShouldRedirectToIndex()
        {
            // Arrange
            string userId = "testUserId";
            _mockUserManagementService.Setup(x => x.CheckUserBeforeDelete(userId))
                .ReturnsAsync(ResultDTO<bool>.Ok(false));
            _mockUserManagementService.Setup(x => x.DeleteUser(userId))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteUserConfirmed(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        //[Fact]
        //public void GetRoleClaims_ShouldReturnRoleClaims_WhenRoleIdIsValid()
        //{
        //    // Arrange
        //    string roleId = "validRoleId";
        //    var roleClaims = new List<AuthClaim> { new AuthClaim { ClaimValue = "Claim1" }, new AuthClaim { ClaimValue = "Claim2" } };
        //    var authClaims = new List<AuthClaim> { new AuthClaim { Value = "Claim1" }, new AuthClaim { Value = "Claim2" } };

        //    _mockUserManagementService.Setup(x => x.GetRoleClaims(roleId)).ReturnsAsync(roleClaims);
        //    _modulesAndAuthClaimsHelper.Setup(h => h.GetAuthClaims()).ReturnsAsync(authClaims);

        //    // Act
        //    var result = _controller.GetRoleClaims(roleId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Count);
        //    Assert.All(result, claim => Assert.Contains(claim.ClaimValue, roleClaims.Select(rc => rc.ClaimValue)));
        //}

        //[Fact]
        //public void GetRoleClaims_ShouldThrowException_WhenRoleClaimsNotFound()
        //{
        //    // Arrange
        //    string roleId = "invalidRoleId";
        //    _mockUserManagementService.Setup(x => x.GetRoleClaims(roleId)).ReturnsAsync((List<RoleClaimDTO>)null);

        //    // Act & Assert
        //    var exception = Assert.Throws<Exception>(() => _controller.GetRoleClaims(roleId));
        //    Assert.Equal("Role claims not found", exception.Message);
        //}

        //[Fact]
        //public void GetUserClaims_ReturnsMappedUserClaims_WhenUserClaimsExist()
        //{
        //    // Arrange
        //    var userId = "testUserId";
        //    var userClaims = new List<UserClaimDTO>
        //{
        //    new UserClaimDTO { ClaimValue = "Claim1" },
        //    new UserClaimDTO { ClaimValue = "Claim2" }
        //};

        //    var authClaims = new List<AuthClaim>
        //{
        //    new AuthClaim { Value = "Claim1" },
        //    new AuthClaim { Value = "Claim3" }
        //};

        //    _mockUserManagementService
        //        .Setup(service => service.GetUserClaims(userId)).ReturnsAsync(userClaims);


        //    // Act
        //    var result = _controller.GetUserClaims(userId);

        //    // Assert
        //    Assert.NotEmpty(result);
        //    Assert.Equal(1, result.Count);
        //    Assert.Equal("Claim1", result.First().ClaimValue);
        //}

        //[Fact]
        //public void GetUserClaims_ThrowsException_WhenUserClaimsNotFound()
        //{
        //    // Arrange
        //    var userId = "testUserId";

        //    _mockUserManagementService.Setup(service => service.GetUserClaims(userId)).ReturnsAsync(new List<UserClaimDTO>());

        //    // Act & Assert
        //    var ex = Assert.Throws<Exception>(() => _controller.GetUserClaims(userId));
        //    Assert.Equal("User claims not found", ex.Message);
        //}

        //[Fact]
        //public void GetUserClaims_HandlesServiceException()
        //{
        //    // Arrange
        //    var userId = "testUserId";

        //    _mockUserManagementService
        //        .Setup(service => service.GetUserClaims(userId))
        //        .ThrowsAsync(new Exception("Service error"));

        //    // Act & Assert
        //    var ex = Assert.Throws<AggregateException>(() => _controller.GetUserClaims(userId));
        //    Assert.Equal(ex.InnerException.Message, "Service error");
        //}

        [Fact]
        public async Task GetUserRoles_ShouldReturnUserRoles_WhenUserIdIsValid()
        {
            // Arrange
            string userId = "validUserId";
            var roles = new List<RoleDTO>
        {
            new RoleDTO { Id = "role1", Name = "Admin" },
            new RoleDTO { Id = "role2", Name = "User" }
        };

            _mockUserManagementService.Setup(x => x.GetRolesForUser(userId)).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetUserRoles(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, role => Assert.NotNull(role));
        }

        [Fact]
        public async Task GetUserRoles_ShouldThrowException_WhenRoleIdsNotFound()
        {
            // Arrange
            string userId = "invalidUserId";
            _mockUserManagementService.Setup(x => x.GetRolesForUser(userId)).ThrowsAsync(new Exception("Role ids not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetUserRoles(userId));
            Assert.Equal("Role ids not found", exception.Message);
        }


    }
}
