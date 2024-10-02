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


    }
}
