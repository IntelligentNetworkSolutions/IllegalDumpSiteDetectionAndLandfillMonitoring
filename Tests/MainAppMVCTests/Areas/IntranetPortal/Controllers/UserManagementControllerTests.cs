﻿using AutoMapper;
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
            _mockUserManagementService.Setup(x => x.GetAllIntanetPortalUsers()).ReturnsAsync(ResultDTO<List<UserDTO>>.Ok(users));
            _mockUserManagementService.Setup(x => x.GetAllRoles()).ReturnsAsync(ResultDTO<List<RoleDTO>>.Ok(roles));
            _mockUserManagementService.Setup(x => x.GetAllUserRoles()).ReturnsAsync(ResultDTO<List<UserRoleDTO>>.Ok(userRoles));

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

        [Fact]
        public async Task EditRole_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new UserManagementEditRoleViewModel { Id = "448cbeb7-964e-4527-9a6c-cea969c05ae4", Name = "test name" };

            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            var roleManagementDTO = new RoleManagementDTO { Id = viewModel.Id, Name = "filled name" };
            _mockUserManagementService.Setup(x => x.FillRoleManagementDto(It.IsAny<RoleManagementDTO>()))
                .ReturnsAsync(ResultDTO<RoleManagementDTO>.Ok(roleManagementDTO));

            var claims = new List<AuthClaim> { new AuthClaim { } };

            // Act
            var result = await _controller.EditRole(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserManagementEditRoleViewModel>(viewResult.Model);
            Assert.Equal(roleManagementDTO.Id, model.Id);
            Assert.Equal(roleManagementDTO.Name, model.Name);
        }

        [Fact]
        public async Task EditRole_ReturnsView_WhenUpdateRoleFails()
        {
            // Arrange
            var viewModel = new UserManagementEditRoleViewModel { Id = "448cbeb7-964e-4527-9a6c-cea969c05ae4", Name = "test name" };
            var roleManagementDTO = new RoleManagementDTO { Id = viewModel.Id, Name = "test name" };

            // Mocking UpdateRole to return a failure
            _mockUserManagementService.Setup(x => x.UpdateRole(It.IsAny<RoleManagementDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            var filledRoleManagementDTO = new RoleManagementDTO { Id = viewModel.Id, Name = "filled name" };
            _mockUserManagementService.Setup(x => x.FillRoleManagementDto(It.IsAny<RoleManagementDTO>()))
                .ReturnsAsync(ResultDTO<RoleManagementDTO>.Ok(filledRoleManagementDTO));

            // Mocking claims
            var claims = new List<AuthClaim> { new AuthClaim { } };

            // Act
            var result = await _controller.EditRole(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserManagementEditRoleViewModel>(viewResult.Model);
            Assert.Equal(filledRoleManagementDTO.Id, model.Id);
            Assert.Equal(filledRoleManagementDTO.Name, model.Name);
            Assert.True(_controller.ModelState.ContainsKey("ModelOnly"));
            Assert.Equal("Update failed", _controller.ModelState["ModelOnly"].Errors[0].ErrorMessage);
        }


        [Fact]
        public async Task EditRole_RedirectsToIndex_WhenUpdateRoleSucceeds()
        {
            // Arrange
            var viewModel = new UserManagementEditRoleViewModel { Id = "448cbeb7-964e-4527-9a6c-cea969c05ae4", Name = "test name" };
            var roleManagementDTO = new RoleManagementDTO { Id = viewModel.Id, Name = "test name" };

            _mockUserManagementService
                .Setup(x => x.UpdateRole(It.Is<RoleManagementDTO>(dto => dto.Id == viewModel.Id && dto.Name == viewModel.Name)))
                .ReturnsAsync(ResultDTO.Ok());
            // Act
            var result = await _controller.EditRole(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteRole_ReturnsRoleDTO_WhenIdIsValid()
        {
            // Arrange
            string roleId = "testRoleId";
            var expectedRole = new RoleDTO { Id = roleId, Name = "TestRole" };
            _mockUserManagementService.Setup(x => x.GetRoleById(roleId))
                .ReturnsAsync(ResultDTO<RoleDTO>.Ok(expectedRole));

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRole.Id, result.Data.Id);
            Assert.Equal(expectedRole.Name, result.Data.Name);
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
        public async Task DeleteRoleConfirmed_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var roleId = "123";
            var exceptionMessage = "Unexpected error";
            _mockUserManagementService
                .Setup(service => service.DeleteRole(roleId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteRoleConfirmed(roleId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }

        [Fact]
        public async Task DeleteUser_ReturnsUserDTO_WhenUserExists()
        {
            // Arrange
            var userId = "testUserId";
            var expectedUser = new UserDTO { Id = userId, UserName = "TestUser", Email = "testuser@example.com" };
            _mockUserManagementService.Setup(s => s.GetUserById(userId))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(expectedUser));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            _mockUserManagementService.Verify(s => s.GetUserById(userId), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Data.Id);
            Assert.Equal(expectedUser.UserName, result.Data.UserName);
            Assert.Equal(expectedUser.Email, result.Data.Email);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistentUserId";
            _mockUserManagementService.Setup(s => s.GetUserById(userId))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok((UserDTO ?)null));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            _mockUserManagementService.Verify(s => s.GetUserById(userId), Times.Once);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteUserConfirmed_WhenCheckUserBeforeDeleteFails_ShouldReturnFailResultWithErrorMessage()
        {
            // Arrange
            string userId = "testUserId";
            string errorMessage = "Error retrieving data";

            _mockUserManagementService.Setup(x => x.CheckUserBeforeDelete(userId))
                .ReturnsAsync(ResultDTO<bool>.Fail(errorMessage));

            // Act
            var result = await _controller.DeleteUserConfirmed(userId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg); 
        }

        [Fact]
        public async Task DeleteUserConfirmed_WhenUserHasEntry_ShouldReturnFailResultWithSpecificMessage()
        {
            // Arrange
            string userId = "testUserId";
            string expectedMessage = "You can not delete this user because there are data entries connected with the same user!";
            var resultDTO = ResultDTO<bool>.Ok(true);

            _mockUserManagementService.Setup(x => x.CheckUserBeforeDelete(userId))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.DeleteUserConfirmed(userId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.ErrMsg);
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

            _mockUserManagementService.Setup(x => x.GetRolesForUser(userId)).ReturnsAsync(ResultDTO<List<RoleDTO>>.Ok(roles));

            // Act
            var result = await _controller.GetUserRoles(userId);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.All(result.Data, role => Assert.NotNull(role));
        }

       
        [Fact]
        public async Task GetUserRoles_ShouldReturnFail_WhenDataIsNull()
        {
            // Arrange
            var userId = "123";             
            var resultDto = ResultDTO<List<RoleDTO>>.Ok(null);
            _mockUserManagementService
                .Setup(service => service.GetRolesForUser(userId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetUserRoles(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No data found", result.ErrMsg);
        }

        [Fact]
        public async Task GetUserRoles_ShouldReturnOk_WhenDataIsNotNull()
        {
            // Arrange
            var userId = "123";
            var rolesData = new List<RoleDTO>() {new RoleDTO() { Name="Role 1"},new RoleDTO() { Name = "Role 2"} };
            var resultDto = ResultDTO<List<RoleDTO>>.Ok(rolesData);
            _mockUserManagementService
                .Setup(service => service.GetRolesForUser(userId))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetUserRoles(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(rolesData, result.Data);
        }

        [Fact]
        public async Task GetUserRoles_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = "123";
            var exceptionMessage = "Unexpected error";
            _mockUserManagementService
                .Setup(service => service.GetRolesForUser(userId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetUserRoles(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }


        [Fact]
        public async Task FillUserManagementEditUserViewModelFromDto_ThrowsException_ReturnsFailureResult()
        {
            // Arrange
            var dto = new UserManagementDTO { };
            var expectedErrorMessage = "Test exception message";

            // Mock the service to throw an exception
            _mockUserManagementService.Setup(x => x.FillUserManagementDto(It.IsAny<UserManagementDTO>()))
                .ThrowsAsync(new Exception(expectedErrorMessage));

            // Act
            var result = await _controller.FillUserManagementEditUserViewModelFromDto(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.ErrMsg);
        }

    }
}
