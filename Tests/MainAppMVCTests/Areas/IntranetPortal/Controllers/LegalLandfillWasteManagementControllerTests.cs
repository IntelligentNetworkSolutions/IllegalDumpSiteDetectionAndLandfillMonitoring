using AutoMapper;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class LegalLandfillWasteManagementControllerTests
    {
        private readonly Mock<ILegalLandfillTruckService> _mockLegalLandfillTruckService;
        private readonly Mock<ILegalLandfillWasteTypeService> _mockLegalLandfillWasteTypeService;
        private readonly Mock<ILegalLandfillWasteImportService> _mockLegalLandfillWasteImportService;
        private readonly Mock<ILegalLandfillService> _mockLegalLandfillService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly LegalLandfillsWasteManagementController _controller;

        public LegalLandfillWasteManagementControllerTests()
        {
            _mockLegalLandfillTruckService = new Mock<ILegalLandfillTruckService>();
            _mockLegalLandfillWasteTypeService = new Mock<ILegalLandfillWasteTypeService>();
            _mockLegalLandfillWasteImportService = new Mock<ILegalLandfillWasteImportService>();
            _mockLegalLandfillService = new Mock<ILegalLandfillService>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new LegalLandfillsWasteManagementController(
                _mockLegalLandfillTruckService.Object,
                _mockLegalLandfillWasteTypeService.Object,
                _mockLegalLandfillWasteImportService.Object,
                _mockLegalLandfillService.Object,
                _mockMapper.Object,
                _mockConfiguration.Object
                );
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks()
        {
            //Arrange
            var dtoList = new List<LegalLandfillTruckDTO> { new LegalLandfillTruckDTO() };
            var vmList = new List<LegalLandfillTruckViewModel> { new LegalLandfillTruckViewModel() };
            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(dtoList));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillTruckViewModel>>(dtoList)).Returns(vmList);

            // Act

            var result = await _controller.ViewLegalLandfillTrucks();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<LegalLandfillTruckViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_Redirects_WhenServiceFails()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Fail("Fail");

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns("/Error");

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_ReturnsView_WithValidData()
        {
            // Arrange
            var trucks = new List<LegalLandfillTruckDTO> { new LegalLandfillTruckDTO() };
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(trucks);

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(resultDtoList);
            _mockMapper.Setup(m => m.Map<List<LegalLandfillTruckViewModel>>(It.IsAny<List<LegalLandfillTruckDTO>>()))
                       .Returns(new List<LegalLandfillTruckViewModel>());

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_RedirectsToNotFound_WhenNoDataReturned()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(null);

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_RedirectsToNotFound_WhenMapperReturnsNull()
        {
            // Arrange
            var trucks = new List<LegalLandfillTruckDTO> { new LegalLandfillTruckDTO() };
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(trucks);

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(resultDtoList);
            _mockMapper.Setup(m => m.Map<List<LegalLandfillTruckViewModel>>(It.IsAny<List<LegalLandfillTruckDTO>>()))
                       .Returns((List<LegalLandfillTruckViewModel>)null);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_ReturnsBadRequest_WhenErrorPathIsMissing()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Fail("Fail");

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(resultDtoList);

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateLegalLandfillTruckConfirmed_WithValidModel_ReturnsOkResult()
        {
            //Arrange
            var viewModel = new LegalLandfillTruckViewModel { Id = Guid.NewGuid(), Name = "test", IsEnabled = true, PayloadWeight = 100, UnladenWeight = 101, Capacity = 100 };
            var dto = new LegalLandfillTruckDTO { Id = viewModel.Id, Name = viewModel.Name, IsEnabled = viewModel.IsEnabled, PayloadWeight = viewModel.PayloadWeight, UnladenWeight = viewModel.UnladenWeight, Capacity = viewModel.Capacity };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(viewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.CreateLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.CreateLegalLandfillTruckConfirmed(viewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.True(resultDto.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillTruckConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns((LegalLandfillTruckDTO)null);

            // Act
            var result = await _controller.CreateLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillTruckConfirmed_ReturnsFail_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Capacity = null,
                UnladenWeight = null,
                PayloadWeight = null
            };

            var dto = new LegalLandfillTruckDTO { IsEnabled = true };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);

            // Act
            var result = await _controller.CreateLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Capacity, Unladen Weight and Payload Weight must be provided.", resultDto.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillTruckConfirmed_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            var dto = new LegalLandfillTruckDTO { Capacity = 1000, UnladenWeight = 500, PayloadWeight = 500, IsEnabled = true };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.CreateLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Fail("Service error"));

            // Act
            var result = await _controller.CreateLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruckConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel { Id = Guid.NewGuid(), Name = "test", IsEnabled = true, PayloadWeight = 100, UnladenWeight = 101, Capacity = 100 };
            var dto = new LegalLandfillTruckDTO { Id = viewModel.Id, Name = viewModel.Name, IsEnabled = viewModel.IsEnabled, PayloadWeight = viewModel.PayloadWeight, UnladenWeight = viewModel.UnladenWeight, Capacity = viewModel.Capacity };
            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(viewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.EditLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditLegalLandfillTruckConfirmed(viewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.True(resultDto.IsSuccess);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_WithValidId_ReturnsLegalLandfillDTO()
        {
            var landfillTruckId = Guid.NewGuid();
            var dto = new LegalLandfillTruckDTO { Id = landfillTruckId };
            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(landfillTruckId))
                .ReturnsAsync(ResultDTO<LegalLandfillTruckDTO>.Ok(dto));

            // Act
            var result = await _controller.GetLegalLandfillTruckById(landfillTruckId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(landfillTruckId, result.Data.Id);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ReturnsOk_WhenRetrievalIsSuccessful()
        {
            // Arrange
            var legalLandfillTruckId = Guid.NewGuid();
            var legalLandfillTruckDto = new LegalLandfillTruckDTO
            {
                Id = legalLandfillTruckId,
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            var resultDto = ResultDTO<LegalLandfillTruckDTO>.Ok(legalLandfillTruckDto);

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(legalLandfillTruckId))
                                           .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillTruckById(legalLandfillTruckId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillTruckDTO>>(result);
            Assert.True(resultData.IsSuccess);
            Assert.Equal(legalLandfillTruckDto, resultData.Data);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillTruckId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillTruckDTO>.Fail("Service error");

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(legalLandfillTruckId))
                                           .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillTruckById(legalLandfillTruckId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillTruckDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Service error", resultData.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillTruckId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillTruckDTO>.Ok(null);

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(legalLandfillTruckId))
                                           .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillTruckById(legalLandfillTruckId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillTruckDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Landfill is null", resultData.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ReturnsFail_WhenServiceHandlesError()
        {
            // Arrange
            var legalLandfillTruckId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillTruckDTO>.Fail("Error handling");

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(legalLandfillTruckId))
                                           .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillTruckById(legalLandfillTruckId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillTruckDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Error handling", resultData.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillTruckConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel();

            _controller.ModelState.AddModelError("Name", "Name is reqiered");

            //Act
            var result = await _controller.CreateLegalLandfillTruckConfirmed(viewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruckConfirmed_WithMappingFailure_ReturnsFailResult()
        {
            //Arrange
            var viewModel = new LegalLandfillTruckViewModel();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(viewModel)).Returns((LegalLandfillTruckDTO)null);

            // Act
            var result = await _controller.EditLegalLandfillTruckConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruckConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel();

            _controller.ModelState.AddModelError("Name", "Name is reqiered");

            //Act
            var result = await _controller.EditLegalLandfillTruckConfirmed(viewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel { Id = Guid.NewGuid() };
            var dto = new LegalLandfillTruckDTO { Id = viewModel.Id };

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(viewModel.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillTruckDTO>.Ok(dto));

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(viewModel)).Returns(dto);

            _mockLegalLandfillTruckService.Setup(s => s.DeleteLegalLandfillTruck(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel();

            _controller.ModelState.AddModelError("Id", "Id is required");

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_WhenTruckNotFound_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel { Id = Guid.NewGuid() };

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(viewModel.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillTruckDTO>.Fail("Truck not found"));

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Truck not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_WithMappingFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillTruckViewModel { Id = Guid.NewGuid() };

            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(viewModel.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillTruckDTO>.Ok(new LegalLandfillTruckDTO()));

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(viewModel)).Returns((LegalLandfillTruckDTO)null);

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_WhenSuccess_ReturnsViewResult()
        {
            // Arrange
            var dtoList = new List<LegalLandfillWasteTypeDTO>
    {
        new LegalLandfillWasteTypeDTO { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" }
    };
            var resultDto = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(dtoList);
            var vmList = new List<LegalLandfillWasteTypeViewModel>
    {
        new LegalLandfillWasteTypeViewModel { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" }
    };

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(resultDto);

            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteTypeViewModel>>(resultDto.Data))
                .Returns(vmList);

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(vmList, viewResult.Model);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_WhenError_ReturnsBadRequest()
        {
            // Arrange
            var resultDto = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Some error");
            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_ReturnsView_WithValidData()
        {
            // Arrange
            var wasteTypes = new List<LegalLandfillWasteTypeDTO> { new LegalLandfillWasteTypeDTO() };
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(wasteTypes);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(resultDtoList);
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteTypeViewModel>>(It.IsAny<List<LegalLandfillWasteTypeDTO>>()))
                       .Returns(new List<LegalLandfillWasteTypeViewModel>());

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_Redirects_WhenServiceFails()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Fail");

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns("/Error");

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_RedirectsToNotFound_WhenNoDataReturned()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(null);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_RedirectsToNotFound_WhenMapperReturnsNull()
        {
            // Arrange
            var wasteTypes = new List<LegalLandfillWasteTypeDTO> { new LegalLandfillWasteTypeDTO() };
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(wasteTypes);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(resultDtoList);
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteTypeViewModel>>(It.IsAny<List<LegalLandfillWasteTypeDTO>>()))
                       .Returns((List<LegalLandfillWasteTypeViewModel>)null);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_ReturnsBadRequest_WhenErrorPathIsMissing()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Fail");

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(resultDtoList);

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteTypeConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var vm = new LegalLandfillWasteTypeViewModel { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" };
            var dto = new LegalLandfillWasteTypeDTO { Id = vm.Id, Name = "Type1", Description = "Desc1" };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(vm)).Returns(dto);

            _mockLegalLandfillWasteTypeService.Setup(s => s.CreateLegalLandfillWasteType(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.CreateLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteTypeConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var vm = new LegalLandfillWasteTypeViewModel();

            // Act
            var result = await _controller.CreateLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Required", result.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel();

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns((LegalLandfillWasteTypeDTO)null);

            // Act
            var result = await _controller.CreateLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Name = "Test Waste Type",
                Description = "Test Description"
            };

            var dto = new LegalLandfillWasteTypeDTO { Name = "Test Waste Type", Description = "Test Description" };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns(dto);
            _mockLegalLandfillWasteTypeService.Setup(s => s.CreateLegalLandfillWasteType(dto)).ReturnsAsync(ResultDTO.Fail("Service error"));

            // Act
            var result = await _controller.CreateLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenServiceHandlesError()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Name = "Test Waste Type",
                Description = "Test Description"
            };

            var dto = new LegalLandfillWasteTypeDTO { Name = "Test Waste Type", Description = "Test Description" };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns(dto);
            _mockLegalLandfillWasteTypeService.Setup(s => s.CreateLegalLandfillWasteType(dto)).ReturnsAsync(ResultDTO.Fail("Error handling"));

            // Act
            var result = await _controller.CreateLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Error handling", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var vm = new LegalLandfillWasteTypeViewModel { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" };
            var dto = new LegalLandfillWasteTypeDTO { Id = vm.Id };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(vm)).Returns(dto);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(vm.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteTypeDTO>.Ok(dto));

            _mockLegalLandfillWasteTypeService.Setup(s => s.DeleteLegalLandfillWasteType(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_WhenDataNotFound_ReturnsFailResult()
        {
            // Arrange
            var vm = new LegalLandfillWasteTypeViewModel { Id = Guid.NewGuid() };

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(vm.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Not found"));

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenServiceCheckFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Service error");

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeViewModel.Id))
                                               .ReturnsAsync(resultCheckForFiles);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteTypeDTO>.Ok(null);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeViewModel.Id))
                                               .ReturnsAsync(resultCheckForFiles);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Data is null", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteTypeDTO>.Ok(new LegalLandfillWasteTypeDTO());

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeViewModel.Id))
                                               .ReturnsAsync(resultCheckForFiles);
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns((LegalLandfillWasteTypeDTO)null);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteTypeById_WhenSuccess_ReturnsOkResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" };

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(dto.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteTypeDTO>.Ok(dto));

            // Act
            var result = await _controller.GetLegalLandfillWasteTypeById(dto.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillWasteTypeById_WhenDataNotFound_ReturnsFailResult()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(id))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Not found"));

            // Act
            var result = await _controller.GetLegalLandfillWasteTypeById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteTypeById_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillWasteTypeId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillWasteTypeDTO>.Ok(null);

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeId))
                                               .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillWasteTypeDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Landfill is null", resultData.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillWasteTypeConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var vm = new LegalLandfillWasteTypeViewModel { Id = Guid.NewGuid(), Name = "Type1", Description = "Desc1" };
            var dto = new LegalLandfillWasteTypeDTO { Id = vm.Id, Name = vm.Name, Description = vm.Description };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(vm)).Returns(dto);

            _mockLegalLandfillWasteTypeService.Setup(s => s.EditLegalLandfillWasteType(dto))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfillWasteTypeConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var vm = new LegalLandfillWasteTypeViewModel();

            // Act
            ResultDTO result = await _controller.EditLegalLandfillWasteTypeConfirmed(vm);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Required", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel();

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns((LegalLandfillWasteTypeDTO)null);

            // Act
            var result = await _controller.EditLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Updated Waste Type",
                Description = "Updated Description"
            };

            var dto = new LegalLandfillWasteTypeDTO { Id = legalLandfillWasteTypeViewModel.Id, Name = "Updated Waste Type", Description = "Updated Description" };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns(dto);
            _mockLegalLandfillWasteTypeService.Setup(s => s.EditLegalLandfillWasteType(dto)).ReturnsAsync(ResultDTO.Fail("Service error"));

            // Act
            var result = await _controller.EditLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillWasteTypeConfirmed_ReturnsFail_WhenServiceHandlesError()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Updated Waste Type",
                Description = "Updated Description"
            };

            var dto = new LegalLandfillWasteTypeDTO { Id = legalLandfillWasteTypeViewModel.Id, Name = "Updated Waste Type", Description = "Updated Description" };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns(dto);
            _mockLegalLandfillWasteTypeService.Setup(s => s.EditLegalLandfillWasteType(dto)).ReturnsAsync(ResultDTO.Fail("Error handling"));

            // Act
            var result = await _controller.EditLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Error handling", resultDto.ErrMsg);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_ReturnsViewResult_WithListOfWasteImports()
        {
            // Arrange
            var dtoList = new List<LegalLandfillWasteImportDTO> { new LegalLandfillWasteImportDTO() };
            var vmList = new List<LegalLandfillWasteImportViewModel> { new LegalLandfillWasteImportViewModel() };
            _mockLegalLandfillWasteImportService.Setup(s => s.GetAllLegalLandfillWasteImports())
                .ReturnsAsync(ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(dtoList));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteImportViewModel>>(dtoList)).Returns(vmList);

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<LegalLandfillWasteImportViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_Redirects_WhenServiceFails()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Fail("Fail");

            _mockLegalLandfillWasteImportService.Setup(s => s.GetAllLegalLandfillWasteImports()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns("/Error");

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_RedirectsToNotFound_WhenNoDataReturned()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(null);

            _mockLegalLandfillWasteImportService.Setup(s => s.GetAllLegalLandfillWasteImports()).ReturnsAsync(resultDtoList);
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_RedirectsToNotFound_WhenMapperReturnsNull()
        {
            // Arrange
            var wasteImports = new List<LegalLandfillWasteImportDTO> { new LegalLandfillWasteImportDTO() };
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(wasteImports);

            _mockLegalLandfillWasteImportService.Setup(s => s.GetAllLegalLandfillWasteImports()).ReturnsAsync(resultDtoList);
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteImportViewModel>>(It.IsAny<List<LegalLandfillWasteImportDTO>>()))
                       .Returns((List<LegalLandfillWasteImportViewModel>)null);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/NotFound");

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/NotFound", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_ReturnsBadRequest_WhenErrorPathIsMissing()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Fail("Fail");

            _mockLegalLandfillWasteImportService.Setup(s => s.GetAllLegalLandfillWasteImports()).ReturnsAsync(resultDtoList);

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ValidModel_DeletesWasteImport()
        {
            // Arrange
            var wasteImportVM = new LegalLandfillWasteImportViewModel { Id = Guid.NewGuid() };
            var wasteImportDTO = new LegalLandfillWasteImportDTO { Id = wasteImportVM.Id };
            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(wasteImportVM.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteImportDTO>.Ok(wasteImportDTO));
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(wasteImportVM)).Returns(wasteImportDTO);
            _mockLegalLandfillWasteImportService.Setup(s => s.DeleteLegalLandfillWasteImport(wasteImportDTO))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(wasteImportVM);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ReturnsFail_WhenModelStateIsInvalid()
        {
            // Arrange
            var legalLandfillWasteImportViewModel = new LegalLandfillWasteImportViewModel();
            _controller.ModelState.AddModelError("Id", "Id is required.");

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(legalLandfillWasteImportViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Id is required.", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ReturnsFail_WhenServiceCheckFails()
        {
            // Arrange
            var legalLandfillWasteImportViewModel = new LegalLandfillWasteImportViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteImportDTO>.Fail("Service error");

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportViewModel.Id))
                                                .ReturnsAsync(resultCheckForFiles);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(legalLandfillWasteImportViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillWasteImportViewModel = new LegalLandfillWasteImportViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteImportDTO>.Ok(null);

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportViewModel.Id))
                                               .ReturnsAsync(resultCheckForFiles);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(legalLandfillWasteImportViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Data is null", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillWasteImportViewModel = new LegalLandfillWasteImportViewModel
            {
                Id = Guid.NewGuid()
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteImportDTO>.Ok(new LegalLandfillWasteImportDTO());

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportViewModel.Id))
                                               .ReturnsAsync(resultCheckForFiles);
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(legalLandfillWasteImportViewModel)).Returns((LegalLandfillWasteImportDTO)null);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(legalLandfillWasteImportViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_ReturnsViewResult_WithPopulatedViewModel()
        {
            // Arrange
            var userId = "test-user-id";

            var trucksResult = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>());
            var landfillsResult = ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>());
            var wasteTypesResult = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>());

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(trucksResult);
            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills()).ReturnsAsync(landfillsResult);
            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(wasteTypesResult);

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            // Act
            var result = await _controller.CreateLegalLandfillWasteImport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LegalLandfillWasteImportViewModel>(viewResult.Model);
            Assert.Empty(model.LegalLandfillTrucks);
            Assert.Empty(model.LegalLandfills);
            Assert.Empty(model.LegalLandfillWasteTypes);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_Redirects_WhenTruckServiceFails()
        {
            // Arrange
            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks())
                                           .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Fail("Service error"));

            var claims = new List<Claim>
            {
                new Claim("UserId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_Redirects_WhenLandfillsServiceFails()
        {
            // Arrange
            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks())
                                           .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>()));

            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills())
                                     .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Fail("Service error"));

            var claims = new List<Claim>
            {
                new Claim("UserId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_ValidModel_ReturnsRedirectToAction()
        {
            var currentTime = DateTime.UtcNow;
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                Capacity = 100,
                Weight = 200,
                ImportExportStatus = 1,
                CreatedById = "test-user-id",
                CreatedOn = currentTime
            };

            _mockLegalLandfillWasteImportService.Setup(s => s.CreateLegalLandfillWasteImport(It.IsAny<LegalLandfillWasteImportDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_Redirects_WhenWasteTypesServiceFails()
        {
            // Arrange
            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks())
                                           .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>()));
            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills())
                                     .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>()));

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes())
                                               .ReturnsAsync(ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Service error"));

            var claims = new List<Claim>
            {
               new Claim("UserId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();
            _controller.ModelState.AddModelError("Capacity", "Capacity is required.");

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_ServiceCallFails_ReturnsViewWithModelStateError()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Capacity = 1000,
                Weight = 500,
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                CreatedById = "user-123"
            };

            var dto = new LegalLandfillWasteImportDTO
            {
                Capacity = viewModel.Capacity,
                Weight = viewModel.Weight,
                LegalLandfillId = viewModel.LegalLandfillId,
                LegalLandfillTruckId = viewModel.LegalLandfillTruckId,
                LegalLandfillWasteTypeId = viewModel.LegalLandfillWasteTypeId,
                CreatedById = viewModel.CreatedById
            };

            // Simulate service failure
            _mockLegalLandfillWasteImportService.Setup(s => s.CreateLegalLandfillWasteImport(It.IsAny<LegalLandfillWasteImportDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Service failed"));

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Equal("An error occurred while processing your request.", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_Success_RedirectsToViewWasteImports()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Capacity = 1000,
                Weight = 500,
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                CreatedById = "user-123"
            };

            var dto = new LegalLandfillWasteImportDTO
            {
                Capacity = viewModel.Capacity,
                Weight = viewModel.Weight,
                LegalLandfillId = viewModel.LegalLandfillId,
                LegalLandfillTruckId = viewModel.LegalLandfillTruckId,
                LegalLandfillWasteTypeId = viewModel.LegalLandfillWasteTypeId,
                CreatedById = viewModel.CreatedById
            };

            // Simulate successful service call
            _mockLegalLandfillWasteImportService.Setup(s => s.CreateLegalLandfillWasteImport(It.IsAny<LegalLandfillWasteImportDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.CreateLegalLandfillWasteImport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
            Assert.Equal("LegalLandfillsWasteManagement", redirectResult.ControllerName);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_ReturnsViewResult_WithPopulatedViewModel()
        {
            // Arrange
            var wasteImportId = Guid.NewGuid();
            var userId = "test-user-id";

            var wasteImportDTO = new LegalLandfillWasteImportDTO
            {
                Id = wasteImportId,
                Capacity = 100,
                Weight = 200,
                CreatedById = userId,
                ImportExportStatus = 1,
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
            };
            var trucksResult = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>());
            var landfillsResult = ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>());
            var wasteTypesResult = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>());

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(wasteImportId))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteImportDTO>.Ok(wasteImportDTO));
            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks()).ReturnsAsync(trucksResult);
            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills()).ReturnsAsync(landfillsResult);
            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes()).ReturnsAsync(wasteTypesResult);

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(wasteImportId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LegalLandfillWasteImportViewModel>(viewResult.Model);
            Assert.Equal(wasteImportDTO.Id, model.Id);
            Assert.Equal(wasteImportDTO.Capacity, model.Capacity);
            Assert.Equal(wasteImportDTO.Weight, model.Weight);
            Assert.Equal(wasteImportDTO.CreatedById, model.CreatedById);
            Assert.Equal(wasteImportDTO.ImportExportStatus, model.ImportExportStatus);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_Redirects_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                                                .ReturnsAsync(ResultDTO<LegalLandfillWasteImportDTO>.Fail("Service error"));

            var claims = new List<Claim>
            {
                new Claim("UserId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(legalLandfillWasteImportId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_Success_ReturnsViewWithViewModel()
        {
            // Arrange
            var importId = Guid.NewGuid();
            var importEntity = new LegalLandfillWasteImportDTO { Id = importId, Capacity = 500, Weight = 1200 };

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(importId))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteImportDTO>.Ok(importEntity));

            _mockLegalLandfillTruckService.Setup(s => s.GetAllLegalLandfillTrucks())
                .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>()));

            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills())
                .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>()));

            _mockLegalLandfillWasteTypeService.Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>()));

            // Mock User Identity
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim("UserId", "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(importId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<LegalLandfillWasteImportViewModel>(viewResult.Model);

            Assert.Equal(importEntity.Id, viewModel.Id);
            Assert.Equal(importEntity.Capacity, viewModel.Capacity);
            Assert.Equal("test-user-id", viewModel.CreatedById);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Id = Guid.NewGuid(),
                Capacity = 100,
                Weight = 200,
                ImportExportStatus = 1
            };
            var dto = new LegalLandfillWasteImportDTO
            {
                Id = viewModel.Id,
                Capacity = viewModel.Capacity,
                Weight = viewModel.Weight,
                ImportExportStatus = viewModel.ImportExportStatus
            };

            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(viewModel)).Returns(dto);
            _mockLegalLandfillWasteImportService.Setup(s => s.EditLegalLandfillWasteImport(dto))
                .ReturnsAsync(ResultDTO.Ok());  // Note the use of ReturnsAsync for Task<ResultDTO>

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_InvalidModelState_ReturnsErrorOrBadRequest()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();
            _controller.ModelState.AddModelError("Capacity", "Capacity is required");

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns("/ErrorPage");

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/ErrorPage", redirectResult.Url);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_InvalidModelState_NoErrorPath_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();
            _controller.ModelState.AddModelError("Capacity", "Capacity is required");

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(viewModel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_ValidModelState_RedirectsToViewWasteImports()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Capacity = 1000,
                Weight = 500,
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                CreatedById = "user-123"
            };

            var dto = new LegalLandfillWasteImportDTO
            {
                Capacity = viewModel.Capacity,
                Weight = viewModel.Weight,
                LegalLandfillId = viewModel.LegalLandfillId,
                LegalLandfillTruckId = viewModel.LegalLandfillTruckId,
                LegalLandfillWasteTypeId = viewModel.LegalLandfillWasteTypeId,
                CreatedById = viewModel.CreatedById
            };

            // Setup mapping
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(viewModel)).Returns(dto);

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLegalLandfillWasteImports", redirectResult.ActionName);
        }

        [Fact]
        public async Task DisableLegalLandfillTruckConfirmed_ReturnsOk_WhenDisablingIsSuccessful()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                // Populate with valid data
                Id = Guid.NewGuid(),
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            var dto = new LegalLandfillTruckDTO
            {
                Id = Guid.NewGuid(),
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.DisableLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DisableLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.True(resultDto.IsSuccess);
        }

        [Fact]
        public async Task DisableLegalLandfillTruckConfirmed_ReturnsFail_WhenModelStateIsInvalid()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel();
            _controller.ModelState.AddModelError("Id", "Id is required.");

            // Act
            var result = await _controller.DisableLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Id is required.", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DisableLegalLandfillTruckConfirmed_ReturnsFail_WhenMappingFails()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns((LegalLandfillTruckDTO)null);

            // Act
            var result = await _controller.DisableLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Contains("Mapping failed", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DisableLegalLandfillTruckConfirmed_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Id = Guid.NewGuid(),
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            var dto = new LegalLandfillTruckDTO { Id = Guid.NewGuid(), Capacity = 1000, UnladenWeight = 500, PayloadWeight = 500 };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.DisableLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Fail("Service error"));

            // Act
            var result = await _controller.DisableLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Service error", resultDto.ErrMsg);
        }

        [Fact]
        public async Task DisableLegalLandfillTruckConfirmed_ReturnsFail_WhenServiceHandlesError()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Id = Guid.NewGuid(),
                Capacity = 1000,
                UnladenWeight = 500,
                PayloadWeight = 500
            };

            var dto = new LegalLandfillTruckDTO { Id = Guid.NewGuid(), Capacity = 1000, UnladenWeight = 500, PayloadWeight = 500 };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);
            _mockLegalLandfillTruckService.Setup(s => s.DisableLegalLandfillTruck(dto)).ReturnsAsync(ResultDTO.Fail("Error handling"));

            // Act
            var result = await _controller.DisableLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var resultDto = Assert.IsType<ResultDTO>(result);
            Assert.False(resultDto.IsSuccess);
            Assert.Equal("Error handling", resultDto.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_ReturnsOk_WhenRetrievalIsSuccessful()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();
            var legalLandfillWasteImportDto = new LegalLandfillWasteImportDTO
            {
                Id = legalLandfillWasteImportId,
                ImportExportStatus = 1
            };

            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Ok(legalLandfillWasteImportDto);

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                                               .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillWasteImportDTO>>(result);
            Assert.True(resultData.IsSuccess);
            Assert.Equal(legalLandfillWasteImportDto, resultData.Data);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_ReturnsFail_WhenServiceCallFails()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Fail("Service error");

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                                               .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillWasteImportDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Service error", resultData.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Ok(null);

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                                               .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillWasteImportDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Landfill is null", resultData.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_ReturnsFail_WhenServiceHandlesError()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();

            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Fail("Error handling");

            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                                               .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);

            // Assert
            var resultData = Assert.IsType<ResultDTO<LegalLandfillWasteImportDTO>>(result);
            Assert.False(resultData.IsSuccess);
            Assert.Equal("Error handling", resultData.ErrMsg);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_NullData_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(null);

            _mockLegalLandfillTruckService
                .Setup(s => s.GetAllLegalLandfillTrucks())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillTrucks_EmptyViewModelList_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>());

            _mockLegalLandfillTruckService
                .Setup(s => s.GetAllLegalLandfillTrucks())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");
            _mockMapper.Setup(m => m.Map<List<LegalLandfillTruckViewModel>>(It.IsAny<List<LegalLandfillTruckDTO>>()))
                       .Returns((List<LegalLandfillTruckViewModel>)null);

            // Act
            var result = await _controller.ViewLegalLandfillTrucks();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_NullData_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(null);

            _mockLegalLandfillWasteTypeService
                .Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteTypes_EmptyViewModelList_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>());

            _mockLegalLandfillWasteTypeService
                .Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteTypeViewModel>>(It.IsAny<List<LegalLandfillWasteTypeDTO>>()))
                       .Returns((List<LegalLandfillWasteTypeViewModel>)null);

            // Act
            var result = await _controller.ViewLegalLandfillWasteTypes();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_NullData_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(null);

            _mockLegalLandfillWasteImportService
                .Setup(s => s.GetAllLegalLandfillWasteImports())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task ViewLegalLandfillWasteImports_EmptyViewModelList_RedirectsToError404Path()
        {
            // Arrange
            var resultDtoList = ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(new List<LegalLandfillWasteImportDTO>());

            _mockLegalLandfillWasteImportService
                .Setup(s => s.GetAllLegalLandfillWasteImports())
                .ReturnsAsync(resultDtoList);

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteImportViewModel>>(It.IsAny<List<LegalLandfillWasteImportDTO>>()))
                       .Returns((List<LegalLandfillWasteImportViewModel>)null);

            // Act
            var result = await _controller.ViewLegalLandfillWasteImports();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_EntityDataNull_RedirectsToError404Path()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();
            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Ok(null);

            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            _mockLegalLandfillWasteImportService
                .Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                .ReturnsAsync(resultDto);

            _mockLegalLandfillTruckService
                .Setup(s => s.GetAllLegalLandfillTrucks())
                .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>()));

            _mockLegalLandfillService
                .Setup(s => s.GetAllLegalLandfills())
                .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>()));

            _mockLegalLandfillWasteTypeService
                .Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>()));

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns("/error404");

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(legalLandfillWasteImportId);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error404", redirectResult.Url);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_EntityDataNull_NoErrorPath_ReturnsNotFound()
        {
            // Arrange
            var legalLandfillWasteImportId = Guid.NewGuid();
            var resultDto = ResultDTO<LegalLandfillWasteImportDTO>.Ok(null);

            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            _mockLegalLandfillWasteImportService
                .Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportId))
                .ReturnsAsync(resultDto);

            _mockLegalLandfillTruckService
                .Setup(s => s.GetAllLegalLandfillTrucks())
                .ReturnsAsync(ResultDTO<List<LegalLandfillTruckDTO>>.Ok(new List<LegalLandfillTruckDTO>()));

            _mockLegalLandfillService
                .Setup(s => s.GetAllLegalLandfills())
                .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(new List<LegalLandfillDTO>()));

            _mockLegalLandfillWasteTypeService
                .Setup(s => s.GetAllLegalLandfillWasteTypes())
                .ReturnsAsync(ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(new List<LegalLandfillWasteTypeDTO>()));

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error404"]).Returns((string)null);

            // Act
            var result = await _controller.EditLegalLandfillWasteImport(legalLandfillWasteImportId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImportConfirmed_ResultDeleteFails_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillWasteImportViewModel = new LegalLandfillWasteImportViewModel
            {
                Id = Guid.NewGuid(),
                ImportExportStatus = 1,
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid(),
                Capacity = 100,
                Weight = 50,
                CreatedById = "user-id"
            };

            var dto = new LegalLandfillWasteImportDTO();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(legalLandfillWasteImportViewModel)).Returns(dto);

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteImportDTO>.Ok(new LegalLandfillWasteImportDTO());
            _mockLegalLandfillWasteImportService.Setup(s => s.GetLegalLandfillWasteImportById(legalLandfillWasteImportViewModel.Id))
                                                .ReturnsAsync(resultCheckForFiles);

            var resultDelete = ResultDTO.Fail("Delete failed");
            _mockLegalLandfillWasteImportService.Setup(s => s.DeleteLegalLandfillWasteImport(dto)).ReturnsAsync(resultDelete);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteImportConfirmed(legalLandfillWasteImportViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Delete failed", failResult.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_ModelStateInvalid_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid(),
                // Other properties...
            };

            _controller.ModelState.AddModelError("SomeKey", "Some error message");

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Contains("Some error message", failResult.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteTypeConfirmed_ResultDeleteFails_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillWasteTypeViewModel = new LegalLandfillWasteTypeViewModel
            {
                Id = Guid.NewGuid(),
            };

            var dto = new LegalLandfillWasteTypeDTO();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel)).Returns(dto);

            var resultCheckForFiles = ResultDTO<LegalLandfillWasteTypeDTO>.Ok(new LegalLandfillWasteTypeDTO());
            _mockLegalLandfillWasteTypeService.Setup(s => s.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeViewModel.Id))
                                              .ReturnsAsync(resultCheckForFiles);

            var resultDelete = ResultDTO.Fail("Delete failed");
            _mockLegalLandfillWasteTypeService.Setup(s => s.DeleteLegalLandfillWasteType(dto)).ReturnsAsync(resultDelete);

            // Act
            var result = await _controller.DeleteLegalLandfillWasteTypeConfirmed(legalLandfillWasteTypeViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Delete failed", failResult.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruckConfirmed_InvalidWeights_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                // Set properties except Capacity, UnladenWeight, PayloadWeight...
            };

            // Simulate an invalid model state
            _controller.ModelState.AddModelError("Capacity", "Capacity is required.");

            // Act
            var result = await _controller.EditLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Contains("Capacity is required.", failResult.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruckConfirmed_ResultEditFails_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Id = Guid.NewGuid(),
                Capacity = 10,
                UnladenWeight = 5,
                PayloadWeight = 3,
                // Other properties...
            };

            var dto = new LegalLandfillTruckDTO(); // Simulate the mapped DTO
            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);

            // Simulate valid DTO properties
            dto.Capacity = 10;
            dto.UnladenWeight = 5;
            dto.PayloadWeight = 3;

            var resultEdit = ResultDTO.Fail("Edit failed"); // Simulate a failure during edit
            _mockLegalLandfillTruckService.Setup(s => s.EditLegalLandfillTruck(dto)).ReturnsAsync(resultEdit);

            // Act
            var result = await _controller.EditLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Edit failed", failResult.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_InvalidModelState_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                // Set properties as needed...
            };

            // Simulate an invalid model state
            _controller.ModelState.AddModelError("Id", "Id is required.");

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Contains("Id is required.", failResult.ErrMsg);
        }


        [Fact]
        public async Task DeleteLegalLandfillTruckConfirmed_DeleteFails_ReturnsFailResult()
        {
            // Arrange
            var legalLandfillTruckViewModel = new LegalLandfillTruckViewModel
            {
                Id = Guid.NewGuid(),
                // Other properties...
            };

            var resultCheckForFiles = ResultDTO<LegalLandfillTruckDTO>.Ok(new LegalLandfillTruckDTO()); // Simulate valid response
            _mockLegalLandfillTruckService.Setup(s => s.GetLegalLandfillTruckById(legalLandfillTruckViewModel.Id)).ReturnsAsync(resultCheckForFiles);

            var dto = new LegalLandfillTruckDTO(); // Simulate mapped DTO
            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel)).Returns(dto);

            var resultDelete = ResultDTO.Fail("Delete failed"); // Simulate a failure during delete
            _mockLegalLandfillTruckService.Setup(s => s.DeleteLegalLandfillTruck(dto)).ReturnsAsync(resultDelete);

            // Act
            var result = await _controller.DeleteLegalLandfillTruckConfirmed(legalLandfillTruckViewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Delete failed", failResult.ErrMsg);
        }


    }
}
