using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class LegalLandfillsManagementControllerTests
    {
        private readonly Mock<ILegalLandfillService> _mockLegalLandfillService;
        private readonly Mock<ILegalLandfillPointCloudFileService> _mockLegalLandfillPointCloudFileService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        public LegalLandfillsManagementControllerTests()
        {
            _mockLegalLandfillService = new Mock<ILegalLandfillService>();
            _mockLegalLandfillPointCloudFileService = new Mock<ILegalLandfillPointCloudFileService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        }

        [Fact]
        public async Task ViewLegalLandfills_ReturnsViewResult_WithListOfLegalLandfills()
        {
            // Arrange
            var dtoList = new List<LegalLandfillDTO> { new LegalLandfillDTO() };
            var vmList = new List<LegalLandfillViewModel> { new LegalLandfillViewModel() };
            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills()).ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(dtoList));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillViewModel>>(dtoList)).Returns(vmList);

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.ViewLegalLandfills();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<LegalLandfillViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task CreateLegalLandfillConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel();
            var dto = new LegalLandfillDTO();
            _mockMapper.Setup(m => m.Map<LegalLandfillDTO>(viewModel)).Returns(dto);
            _mockLegalLandfillService.Setup(s => s.CreateLegalLandfill(dto)).ReturnsAsync(ResultDTO.Ok());

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.CreateLegalLandfillConfirmed(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfillConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel();
            var dto = new LegalLandfillDTO();
            _mockMapper.Setup(m => m.Map<LegalLandfillDTO>(viewModel)).Returns(dto);
            _mockLegalLandfillService.Setup(s => s.EditLegalLandfill(dto)).ReturnsAsync(ResultDTO.Ok());

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.EditLegalLandfillConfirmed(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }
               
        [Fact]
        public async Task GetLegalLandfillById_WithValidId_ReturnsLegalLandfillDTO()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var dto = new LegalLandfillDTO { Id = landfillId };
            _mockLegalLandfillService.Setup(s => s.GetLegalLandfillById(landfillId))
                .ReturnsAsync(ResultDTO<LegalLandfillDTO>.Ok(dto));

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.GetLegalLandfillById(landfillId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(landfillId, result.Data.Id);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFileById_WithValidId_ReturnsFileDTO()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var dto = new LegalLandfillPointCloudFileDTO { Id = fileId };
            _mockLegalLandfillPointCloudFileService.Setup(s => s.GetLegalLandfillPointCloudFilesById(fileId))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto));

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.GetLegalLandfillPointCloudFileById(fileId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(fileId, result.Data.Id);
        }

        [Fact]
        public async Task ViewPointCloudFiles_ReturnsViewResult_WithListOfPointCloudFiles()
        {
            // Arrange
            var dtoList = new List<LegalLandfillPointCloudFileDTO> { new LegalLandfillPointCloudFileDTO() };
            var vmList = new List<LegalLandfillPointCloudFileViewModel> { new LegalLandfillPointCloudFileViewModel() };
            _mockLegalLandfillPointCloudFileService.Setup(s => s.GetAllLegalLandfillPointCloudFiles())
                .ReturnsAsync(ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtoList));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillPointCloudFileViewModel>>(dtoList)).Returns(vmList);

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.ViewPointCloudFiles();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<LegalLandfillPointCloudFileViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task GetAllLegalLandfills_ReturnsListOfLegalLandfillDTOs()
        {
            // Arrange
            var dtoList = new List<LegalLandfillDTO> { new LegalLandfillDTO() };
            _mockLegalLandfillService.Setup(s => s.GetAllLegalLandfills())
                .ReturnsAsync(ResultDTO<List<LegalLandfillDTO>>.Ok(dtoList));

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.GetAllLegalLandfills();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            var dto = new LegalLandfillPointCloudFileDTO { Id = viewModel.Id, FilePath = "test/path", FileName = "test.file" };
            _mockLegalLandfillPointCloudFileService.Setup(s => s.GetLegalLandfillPointCloudFilesById(viewModel.Id))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto));
            _mockLegalLandfillPointCloudFileService.Setup(s => s.DeleteLegalLandfillPointCloudFile(viewModel.Id))
                .ReturnsAsync(ResultDTO.Ok());
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok("test/path"));
            _mockWebHostEnvironment.Setup(w => w.WebRootPath).Returns("test/root");

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.DeleteLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillConfirmed_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel();
            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.CreateLegalLandfillConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillConfirmed_WithMappingFailure_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel();
            _mockMapper.Setup(m => m.Map<LegalLandfillDTO>(viewModel)).Returns((LegalLandfillDTO)null);

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.EditLegalLandfillConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task UploadConvertLegalLandfillPointCloudFileConfirmed_WithInvalidFile_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                LegalLandfillId = Guid.NewGuid(),
                FileUpload = new Mock<IFormFile>().Object
            };
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(".las, .laz"));

            var controller = new LegalLandfillsManagementController(
                _mockLegalLandfillService.Object,
                _mockLegalLandfillPointCloudFileService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockAppSettingsAccessor.Object,
                _mockWebHostEnvironment.Object);

            // Act
            var result = await controller.UploadConvertLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Not supported file extension", result.ErrMsg);
        }
    }
}
