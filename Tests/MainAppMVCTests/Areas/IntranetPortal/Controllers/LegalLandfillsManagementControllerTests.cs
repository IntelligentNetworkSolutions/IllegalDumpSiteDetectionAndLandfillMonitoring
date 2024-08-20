using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
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
        private readonly LegalLandfillsManagementController _controller;

        public LegalLandfillsManagementControllerTests()
        {
            _mockLegalLandfillService = new Mock<ILegalLandfillService>();
            _mockLegalLandfillPointCloudFileService = new Mock<ILegalLandfillPointCloudFileService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _controller = new LegalLandfillsManagementController(
                   _mockLegalLandfillService.Object,
                   _mockLegalLandfillPointCloudFileService.Object,
                   _mockConfiguration.Object,
                   _mockMapper.Object,
                   _mockAppSettingsAccessor.Object,
                   _mockWebHostEnvironment.Object);
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
        public void ProcessSelectedFiles_NullInput_ReturnsFailResult()
        {
            // Arrange
            List<Guid> selectedFiles = null;

            // Act
            var result = _controller.ProcessSelectedFiles(selectedFiles);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Preview_ReturnsBadRequest_WhenSelectedFilesIsNullOrEmpty()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);

            // Act
            var result = await _controller.Preview(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Preview_RedirectsToErrorView_WhenSelectedFilesIsNullOrEmpty()
        {
            // Arrange
            var errorPath = "/Error";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns(errorPath);

            // Act
            var result = await _controller.Preview(new List<string>());

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(errorPath, redirectResult.Url);
        }

        [Fact]
        public async Task Preview_ReturnsBadRequest_OnException()
        {
            // Arrange
            var selectedFiles = new List<string> { "encryptedGuid" };
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Preview(selectedFiles);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Preview_RedirectsToErrorView_OnException()
        {
            // Arrange
            var selectedFiles = new List<string> { "encryptedGuid" };
            var errorPath = "/Error";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns(errorPath);
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Preview(selectedFiles);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(errorPath, redirectResult.Url);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel();
            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid model", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenEntityNotFound()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Entity not found"));

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Entity not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenEntityDataIsNull()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(null));

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Object not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenDeletingFilesFromUploadsFails()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            var fileDTO = new LegalLandfillPointCloudFileDTO { Id = model.Id, LegalLandfillId = Guid.NewGuid(), FileName = "testfile.las" };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(fileDTO));

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromUploads(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Fail("Failed to delete files from uploads"));

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to delete files from uploads", result.ErrMsg);

        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenDeletingFilesFromConvertsFails()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            var fileDTO = new LegalLandfillPointCloudFileDTO { Id = model.Id, LegalLandfillId = Guid.NewGuid(), FileName = "testfile.las" };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(fileDTO));

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromUploads(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromConverts(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Fail("Failed to delete files from converts"));

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to delete files from converts", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsFailResult_WhenDeletingFromDatabaseFails()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            var fileDTO = new LegalLandfillPointCloudFileDTO { Id = model.Id, LegalLandfillId = Guid.NewGuid(), FileName = "testfile.las" };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(fileDTO));

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromUploads(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromConverts(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteLegalLandfillPointCloudFile(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO.Fail("Failed to delete from database"));

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to delete from database", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFileConfirmed_ReturnsSuccessResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var model = new LegalLandfillPointCloudFileViewModel { Id = Guid.NewGuid() };
            var fileDTO = new LegalLandfillPointCloudFileDTO { Id = model.Id, LegalLandfillId = Guid.NewGuid(), FileName = "testfile.las" };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetLegalLandfillPointCloudFilesById(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(fileDTO));

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromUploads(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteFilesFromConverts(It.IsAny<LegalLandfillPointCloudFileDTO>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.DeleteLegalLandfillPointCloudFile(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteLegalLandfillPointCloudFileConfirmed(model);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillPointCloudFileConfirmed_ModelStateInvalid_ReturnsFail()
        {
            // Arrange
            var mockService = new Mock<ILegalLandfillPointCloudFileService>();
            var mockMapper = new Mock<IMapper>();
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            _controller.ModelState.AddModelError("FileName", "Required");

            var viewModel = new LegalLandfillPointCloudFileViewModel();

            // Act
            var result = await _controller.EditLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Required", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillPointCloudFileConfirmed_FilePathIsNull_ReturnsFail()
        {
            // Arrange
            var mockService = new Mock<ILegalLandfillPointCloudFileService>();
            var mockMapper = new Mock<IMapper>();
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                FilePath = null
            };

            // Act
            var result = await _controller.EditLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("File path is null", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillPointCloudFileConfirmed_MappingFailed_ReturnsFail()
        {
            // Arrange
            var mockService = new Mock<ILegalLandfillPointCloudFileService>();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFileDTO>(It.IsAny<LegalLandfillPointCloudFileViewModel>()))
                      .Returns<LegalLandfillPointCloudFileDTO>(null);

            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                FilePath = "test_path"
            };

            // Act
            var result = await _controller.EditLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }


        [Fact]
        public async Task WasteVolumeDiffAnalysis_ReturnsFail_WhenGetFilteredLegalLandfillPointCloudFilesFails()
        {
            // Arrange
            var selectedFiles = new List<Guid> { Guid.NewGuid() };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetFilteredLegalLandfillPointCloudFiles(selectedFiles))
                .ReturnsAsync(ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Error fetching files"));

            // Act
            var result = await _controller.WasteVolumeDiffAnalysis(selectedFiles);

            // Assert
            var failResult = Assert.IsType<ResultDTO<WasteVolumeComparisonDTO>>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Error fetching files", failResult.ErrMsg);
        }

        [Fact]
        public async Task WasteVolumeDiffAnalysis_ReturnsFail_WhenFilteredDataIsNullOrInvalid()
        {
            // Arrange
            var selectedFiles = new List<Guid> { Guid.NewGuid() };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetFilteredLegalLandfillPointCloudFiles(selectedFiles))
                .ReturnsAsync(ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(null));

            // Act
            var result = await _controller.WasteVolumeDiffAnalysis(selectedFiles);

            // Assert
            var failResult = Assert.IsType<ResultDTO<WasteVolumeComparisonDTO>>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Expected data is null or does not have required number of elements.", failResult.ErrMsg);
        }

        [Fact]
        public async Task WasteVolumeDiffAnalysis_ReturnsFail_WhenCreateDiffWasteVolumeComparisonFileFails()
        {
            // Arrange
            var selectedFiles = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var mockFiles = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { FileName = "file1.las", ScanDateTime = DateTime.Now },
                new LegalLandfillPointCloudFileDTO { FileName = "file2.las", ScanDateTime = DateTime.Now.AddDays(1) }
            };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.GetFilteredLegalLandfillPointCloudFiles(selectedFiles))
                .ReturnsAsync(ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(mockFiles));
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.CreateDiffWasteVolumeComparisonFile(It.IsAny<List<LegalLandfillPointCloudFileDTO>>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Error creating file"));

            // Act
            var result = await _controller.WasteVolumeDiffAnalysis(selectedFiles);

            // Assert
            var failResult = Assert.IsType<ResultDTO<WasteVolumeComparisonDTO>>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Error creating file", failResult.ErrMsg);
        }

        [Fact]
        public async Task UploadConvertLegalLandfillPointCloudFileConfirmed_ReturnsFail_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                FileUpload = null // Invalid view model
            };
            _controller.ModelState.AddModelError("FileUpload", "File is required");

            // Act
            var result = await _controller.UploadConvertLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Contains("File is required", failResult.ErrMsg);
        }

        [Fact]
        public async Task UploadConvertLegalLandfillPointCloudFileConfirmed_ReturnsFail_WhenFileUploadIsNull()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                FileUpload = null
            };

            // Act
            var result = await _controller.UploadConvertLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Please insert file", failResult.ErrMsg);
        }

        [Fact]
        public async Task UploadConvertLegalLandfillPointCloudFileConfirmed_ReturnsFail_WhenFileTypeIsNotSupported()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                FileUpload = new Mock<IFormFile>().Object,
                FileName = "testFile",
                ScanDateTime = DateTime.Now,
                LegalLandfillId = Guid.NewGuid()
            };
            _mockLegalLandfillPointCloudFileService
                .Setup(s => s.CheckSupportingFiles(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Fail("Unsupported file type"));

            // Act
            var result = await _controller.UploadConvertLegalLandfillPointCloudFileConfirmed(viewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Unsupported file type", failResult.ErrMsg);
        }

     
    }
}
