using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class MapConfigurationControllerTests
    {
        private readonly Mock<IMapConfigurationService> _mockMapConfigurationService;
        private readonly Mock<IMapLayersConfigurationService> _mockMapLayersConfigurationService;
        private readonly Mock<IMapLayerGroupsConfigurationService> _mockMapLayerGroupsConfigurationService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MapConfigurationController _controller;

        public MapConfigurationControllerTests()
        {
            _mockMapConfigurationService = new Mock<IMapConfigurationService>();
            _mockMapLayerGroupsConfigurationService = new Mock<IMapLayerGroupsConfigurationService>();
            _mockMapLayersConfigurationService = new Mock<IMapLayersConfigurationService>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new MapConfigurationController
                (
                _mockMapConfigurationService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockMapLayersConfigurationService.Object,
                _mockMapLayerGroupsConfigurationService.Object
                );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "userId")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }



        [Fact]
        public async Task CreateMapConfiguration_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var dto = new MapConfigurationDTO();
            var userId = "test-user-id";
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapConfigurationService.Setup(s => s.CreateMapConfiguration(dto)).ReturnsAsync(ResultDTO.Ok());

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };
            // Act
            var result = await _controller.CreateMapConfiguration(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateMapConfiguration_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapConfigurationViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };
            _controller.ModelState.AddModelError("MapName", "MapName is required");

            // Act
            var result = await _controller.CreateMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("MapName is required", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapConfiguration_UserNotAuthenticated_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var userId = string.Empty; // Simulate not authenticated

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }))
                }
            };

            // Act
            var result = await _controller.CreateMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapConfiguration_MappingFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var userId = "test-user-id";

            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns((MapConfigurationDTO)null); // Simulate mapping failure

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.CreateMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task CreateMapConfiguration_ServiceReturnsFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var dto = new MapConfigurationDTO();
            var userId = "test-user-id";

            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapConfigurationService.Setup(s => s.CreateMapConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Creation failed"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            // Act
            var result = await _controller.CreateMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var dto = new MapConfigurationDTO();
            _mockMapConfigurationService.Setup(s => s.EditMapConfiguration(dto)).ReturnsAsync(ResultDTO.Ok());
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns(dto);

            // Act
            var result = await _controller.EditMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task EditMapConfiguration_WithMappingFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns((MapConfigurationDTO)null);

            // Act
            var result = await _controller.EditMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_UserNotAuthenticated_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();

            // Act
            var result = await _controller.EditMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();

            _controller.ModelState.AddModelError("MapName", "MapName is required");

            // Act
            var result = await _controller.EditMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("MapName is required", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_ServiceReturnsFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var dto = new MapConfigurationDTO();
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapConfigurationService.Setup(s => s.EditMapConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Edit failed"));

            var userId = "testUserId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId),
                new Claim(nameof(SD.AuthClaims.EditMapConfigurations), "true")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.EditMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Edit failed", result.ErrMsg);

        }


        [Fact]
        public async Task DeleteMapConfiguration_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapConfigurationDTO { Id = viewModel.Id };

            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(dto));

            _mockMapConfigurationService.Setup(s => s.DeleteMapConfiguration(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteMapConfiguration_WithInvalidId_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel();
            var dto = new MapConfigurationDTO();
            _controller.ModelState.AddModelError("MapName", "MapName is required");

            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(viewModel)).Returns(dto);
            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("MapName is required", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapConfiguration_UserNotAuthenticated_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapConfigurationService
                .Setup(service => service.GetMapConfigurationById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Fail("User is not authenticated."));

            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapConfiguration_NoDataFound_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Fail("Data not found"));

            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapConfiguration_DataIsNull_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapConfiguration_ServiceReturnsFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new MapConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapConfigurationDTO { Id = viewModel.Id };

            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(dto));

            _mockMapConfigurationService.Setup(s => s.DeleteMapConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _controller.DeleteMapConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }


        [Fact]
        public async Task GetMapConfiguration_WithValidId_ReturnsMapConfigurationDTO()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            var dto = new MapConfigurationDTO { Id = mapId };
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.GetMapConfigurationById(mapId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mapId, result.Data.Id);
        }

        [Fact]
        public async Task GetMapConfigurationById_WhenResultIsNotSuccess_ReturnsFailureResult()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            var errorMessage = "Service error";

            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Fail(errorMessage));

            // Act
            var result = await _controller.GetMapConfigurationById(mapId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetMapConfigurationById_WhenDataIsNull_ReturnsFailureResult()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.GetMapConfigurationById(mapId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Map configuration is null", result.ErrMsg);
        }

        [Fact]
        public async Task Index_WithValidData_ReturnsViewWithMapConfigurationViewModelList()
        {
            // Arrange
            var dtoList = new List<MapConfigurationDTO> { new MapConfigurationDTO() };
            var vmList = new List<MapConfigurationViewModel> { new MapConfigurationViewModel() };

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Ok(dtoList));

            _mockMapper.Setup(m => m.Map<List<MapConfigurationViewModel>>(dtoList))
                .Returns(vmList);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<MapConfigurationViewModel>>(viewResult.ViewData.Model);

            Assert.Equal(vmList.Count, model.Count);
        }

        [Fact]
        public async Task Index_WhenResultDtoListIsNotSuccess_RedirectsToErrorPage()
        {
            // Arrange
            var errorPath = "/ErrorPage";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns(errorPath);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Fail("Service error"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(errorPath, redirectResult.Url);
        }

        [Fact]
        public async Task Index_WhenResultDtoListDataIsNull_RedirectsToError404()
        {
            // Arrange
            var error404Path = "/Error404";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns(error404Path);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Ok(null));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(error404Path, redirectResult.Url);
        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var dto = new MapLayerGroupConfigurationDTO { Id = groupId };

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.GetMapLayerGroupConfigurationById(groupId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(groupId, result.Data.Id);

        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_WhenResultIsNotSuccess_ReturnsFailureResult()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var errorMessage = "Service error";

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Fail(errorMessage));

            // Act
            var result = await _controller.GetMapLayerGroupConfigurationById(groupId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerGroupConfigurationById_WhenDataIsNull_ReturnsFailureResult()
        {
            // Arrange
            var groupId = Guid.NewGuid();

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.GetMapLayerGroupConfigurationById(groupId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Map configuration is null", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllGroupLayersById_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var dto = new MapLayerGroupConfigurationDTO { Id = groupId };
            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetAllGroupLayersById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.GetAllGroupLayersById(groupId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(groupId, result.Data.Id);
        }

        [Fact]
        public async Task GetAllGroupLayersById_WhenResultIsNotSuccess_ReturnsFailure()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var errorMessage = "Service error";
            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetAllGroupLayersById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Fail(errorMessage));

            // Act
            var result = await _controller.GetAllGroupLayersById(groupId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetAllGroupLayersById_WhenDataIsNull_ReturnsFailure()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetAllGroupLayersById(groupId))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.GetAllGroupLayersById(groupId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No layers in group ", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerGroupConfiguration_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel { GroupName = "Test Group" };
            var dto = new MapLayerGroupConfigurationDTO { GroupName = "Test Group" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapLayerGroupsConfigurationService.Setup(s => s.CreateMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.AddMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto.GroupName, result.Data.GroupName);
        }

        [Fact]
        public async Task AddMapLayerGroupConfiguration_WhenUserIsNotAuthenticated_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.AddMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerGroupConfiguration_WhenModelStateIsInvalid_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _controller.ModelState.AddModelError("GroupName", "Group Name is required.");

            // Act
            var result = await _controller.AddMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Group Name is required.", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerGroupConfiguration_WhenMappingFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns((MapLayerGroupConfigurationDTO)null);

            // Act
            var result = await _controller.AddMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerGroupConfiguration_WhenServiceFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel { GroupName = "Test Group" };
            var dto = new MapLayerGroupConfigurationDTO { GroupName = "Test Group" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapLayerGroupsConfigurationService.Setup(s => s.CreateMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Service failed"));

            // Act
            var result = await _controller.AddMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel { GroupName = "Updated Group" };
            var dto = new MapLayerGroupConfigurationDTO { GroupName = "Updated Group" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapLayerGroupsConfigurationService.Setup(s => s.EditMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_WhenModelStateIsInvalid_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _controller.ModelState.AddModelError("GroupName", "Group Name is required.");

            // Act
            var result = await _controller.EditMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Group Name is required.", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_WhenUserIsNotAuthenticated_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.EditMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_WhenMappingFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns((MapLayerGroupConfigurationDTO)null);

            // Act
            var result = await _controller.EditMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerGroupConfiguration_WhenServiceFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var viewModel = new MapLayerGroupConfigurationViewModel { GroupName = "Updated Group" };
            var dto = new MapLayerGroupConfigurationDTO { GroupName = "Updated Group" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerGroupConfigurationDTO>(viewModel)).Returns(dto);
            _mockMapLayerGroupsConfigurationService.Setup(s => s.EditMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Service failed"));

            // Act
            var result = await _controller.EditMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapLayerGroupConfigurationDTO { Id = viewModel.Id };

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto));
            _mockMapLayerGroupsConfigurationService.Setup(s => s.DeleteMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_WhenModelStateIsInvalid_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel();
            _controller.ModelState.AddModelError("Id", "Id is required.");

            // Act
            var result = await _controller.DeleteMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_WhenGetEntityFails_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Not found"));

            // Act
            var result = await _controller.DeleteMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_WhenDataIsNull_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.DeleteMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerGroupConfiguration_WhenDeleteFails_ReturnsFailure()
        {
            // Arrange
            var viewModel = new MapLayerGroupConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapLayerGroupConfigurationDTO { Id = viewModel.Id };

            _mockMapLayerGroupsConfigurationService.Setup(s => s.GetMapLayerGroupConfigurationById(viewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto));
            _mockMapLayerGroupsConfigurationService.Setup(s => s.DeleteMapLayerGroupConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _controller.DeleteMapLayerGroupConfiguration(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_WithValidId_ReturnsMapLayerConfigurationDTO()
        {
            // Arrange
            var mapLayerId = Guid.NewGuid();
            var dto = new MapLayerConfigurationDTO { Id = mapLayerId };

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerId))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.GetMapLayerConfigurationById(mapLayerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mapLayerId, result.Data.Id);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_WhenGetEntityFails_ReturnsFailure()
        {
            // Arrange
            var mapLayerId = Guid.NewGuid();

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerId))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Fail("Not found"));

            // Act
            var result = await _controller.GetMapLayerConfigurationById(mapLayerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetMapLayerConfigurationById_WhenDataIsNull_ReturnsFailure()
        {
            // Arrange
            var mapLayerId = Guid.NewGuid();

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerId))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.GetMapLayerConfigurationById(mapLayerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Map configuration is null", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerConfiguration_ValidModel_ReturnsMapLayerConfigurationDTO()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };
            var dto = new MapLayerConfigurationDTO { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns(dto);

            _mockMapLayersConfigurationService.Setup(s => s.CreateMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(dto));

            // Act
            var result = await _controller.AddMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task AddMapLayerConfiguration_UserNotAuthenticated_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.AddMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerConfiguration_InvalidModel_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.AddMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid model", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerConfiguration_MappingFailed_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns((MapLayerConfigurationDTO)null);

            // Act
            var result = await _controller.AddMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task AddMapLayerConfiguration_ServiceFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { };
            var dto = new MapLayerConfigurationDTO { };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns(dto);

            _mockMapLayersConfigurationService.Setup(s => s.CreateMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Fail("Service error"));

            // Act
            var result = await _controller.AddMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service error", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_ValidModel_ReturnsSuccess()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };
            var dto = new MapLayerConfigurationDTO { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns(dto);

            _mockMapLayersConfigurationService.Setup(s => s.EditMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_InvalidModel_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.EditMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid model", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_UserNotAuthenticated_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.EditMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User is not authenticated.", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_MappingFailed_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns((MapLayerConfigurationDTO)null);

            // Act
            var result = await _controller.EditMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapLayerConfiguration_ServiceFails_ReturnsFailure()
        {
            // Arrange
            var userId = "test-user-id";
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { };
            var dto = new MapLayerConfigurationDTO { };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }))
                }
            };

            _mockMapper.Setup(m => m.Map<MapLayerConfigurationDTO>(It.IsAny<MapLayerConfigurationViewModel>()))
                .Returns(dto);

            _mockMapLayersConfigurationService.Setup(s => s.EditMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Service error"));

            // Act
            var result = await _controller.EditMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_ValidModel_ReturnsSuccess()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapLayerConfigurationDTO { /* initialize properties */ };

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerConfigurationViewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(dto));

            _mockMapLayersConfigurationService.Setup(s => s.DeleteMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_InvalidModel_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { /* initialize properties */ };

            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.DeleteMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid model", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_GetEntityFails_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerConfigurationViewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Fail("Entity not found"));

            // Act
            var result = await _controller.DeleteMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Entity not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_DataIsNull_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { Id = Guid.NewGuid() };

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerConfigurationViewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(null));

            // Act
            var result = await _controller.DeleteMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteMapLayerConfiguration_DeleteFails_ReturnsFailure()
        {
            // Arrange
            var mapLayerConfigurationViewModel = new MapLayerConfigurationViewModel { Id = Guid.NewGuid() };
            var dto = new MapLayerConfigurationDTO { };

            _mockMapLayersConfigurationService.Setup(s => s.GetMapLayerConfigurationById(mapLayerConfigurationViewModel.Id))
                .ReturnsAsync(ResultDTO<MapLayerConfigurationDTO>.Ok(dto));

            _mockMapLayersConfigurationService.Setup(s => s.DeleteMapLayerConfiguration(dto))
                .ReturnsAsync(ResultDTO.Fail("Delete error"));

            // Act
            var result = await _controller.DeleteMapLayerConfiguration(mapLayerConfigurationViewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete error", result.ErrMsg);
        }

        [Fact]
        public async Task EditMapConfiguration_ValidModel_SuccessfulRetrievalAndMapping_ReturnsView()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            var dto = new MapConfigurationDTO(); // Assume this is a valid DTO
            var viewModel = new MapConfigurationViewModel();
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId)).ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(dto));
            _mockMapper.Setup(m => m.Map<MapConfigurationViewModel>(dto)).Returns(viewModel);

            // Act
            var result = await _controller.EditMapConfiguration(mapId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<MapConfigurationViewModel>(viewResult.Model);
        }
        [Fact]
        public async Task EditMapConfiguration_FailedRetrieval_RedirectsToErrorPath()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            var resultDto = ResultDTO<MapConfigurationDTO>.Fail("Error retrieving map configuration"); // Simulate failure
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId)).ReturnsAsync(resultDto);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns("/error");

            // Act
            var result = await _controller.EditMapConfiguration(mapId);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error", redirectResult.Url);
        }

        [Fact]
        public async Task EditMapConfiguration_NullData_RedirectsToError404Path()
        {
            // Arrange
            var mapId = Guid.NewGuid();
            var resultDto = ResultDTO<MapConfigurationDTO>.Ok((MapConfigurationDTO)null); // Simulate null data
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId)).ReturnsAsync(resultDto);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.EditMapConfiguration(mapId);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task Index_WhenErrorPathIsNull_ReturnsBadRequest()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Fail("Service error"));

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Index_WhenError404PathIsNullAndDataIsNull_ReturnsNotFound()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Ok(null));

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_WhenError404PathIsNullAndVmListIsNull_ReturnsNotFound()
        {
            // Arrange
            var dtoList = new List<MapConfigurationDTO> { new MapConfigurationDTO() };

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Ok(dtoList));

            _mockMapper.Setup(m => m.Map<List<MapConfigurationViewModel>>(dtoList))
                .Returns((List<MapConfigurationViewModel>)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_WhenVmListIsNullAndError404PathIsConfigured_RedirectsToError404()
        {
            // Arrange
            var dtoList = new List<MapConfigurationDTO> { new MapConfigurationDTO() };
            var error404Path = "/Error404";

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns(error404Path);

            _mockMapConfigurationService.Setup(s => s.GetAllMapConfigurations())
                .ReturnsAsync(ResultDTO<List<MapConfigurationDTO>>.Ok(dtoList));

            // Simulate `null` vmList by having the mapper return `null`
            _mockMapper.Setup(m => m.Map<List<MapConfigurationViewModel>>(dtoList))
                .Returns((List<MapConfigurationViewModel>)null);

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(error404Path, redirectResult.Url);
        }

        [Fact]
        public async Task EditMapConfiguration_WhenResultDataIsNullAndError404PathIsNull_ReturnsNotFound()
        {
            // Arrange
            var mapId = Guid.NewGuid();

            // Simulate a null `Data` in the `ResultDTO`
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Ok(null));

            // Configure `_configuration` to return null for the Error404 path
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            // Act
            var result = await _controller.EditMapConfiguration(mapId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditMapConfiguration_WhenResultNotSuccessAndErrorPathIsNull_ReturnsBadRequest()
        {
            // Arrange
            var mapId = Guid.NewGuid();

            // Simulate an unsuccessful result in the `ResultDTO`
            _mockMapConfigurationService.Setup(s => s.GetMapConfigurationById(mapId))
                .ReturnsAsync(ResultDTO<MapConfigurationDTO>.Fail("Error fetching map configuration"));

            // Configure `_configuration` to return null for the Error path
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);

            // Act
            var result = await _controller.EditMapConfiguration(mapId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditMapConfiguration_WhenMappingFails_ReturnsFailResult()
        {
            // Arrange
            var mapConfigurationViewModel = new MapConfigurationViewModel(); // Populate as needed
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(mapConfigurationViewModel)).Returns((MapConfigurationDTO)null); // Simulate mapping failure

            var userId = "test-user-id";

            // Mock User identity
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };


            // Act
            var result = await _controller.EditMapConfiguration(mapConfigurationViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess); // Ensure the result indicates failure
            Assert.Equal("Mapping failed", failResult.ErrMsg); // Check the error message
        }

        [Fact]
        public async Task EditMapConfiguration_WhenEditIsSuccessful_ReturnsOkResult()
        {
            // Arrange
            var mapConfigurationViewModel = new MapConfigurationViewModel(); // Populate as needed
            var userId = "test-user-id";

            // Mock User identity
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };

            var dto = new MapConfigurationDTO(); // Your DTO that will be mapped to
            _mockMapper.Setup(m => m.Map<MapConfigurationDTO>(mapConfigurationViewModel)).Returns(dto);

            var resultEdit = ResultDTO.Ok();
            _mockMapConfigurationService.Setup(s => s.EditMapConfiguration(dto)).ReturnsAsync(resultEdit);

            // Act
            var result = await _controller.EditMapConfiguration(mapConfigurationViewModel);

            // Assert
            var okResult = Assert.IsType<ResultDTO>(result);
            Assert.True(okResult.IsSuccess); // Ensure the result indicates success
        }


    }
}
