using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Helpers;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.Helpers;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
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
            var users = new List<UserDTO>() { new UserDTO() { Id = "123", UserName = "TestUser", Email = "testuser@gmail.com"} };
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

        
    }
}
