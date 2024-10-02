﻿using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using Services.Interfaces.Services;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DetectionControllerTests
    {

        private readonly Mock<IDetectionRunService> _mockDetectionRunService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly DetectionController _controller;

        public DetectionControllerTests()
        {
            _mockDetectionRunService = new Mock<IDetectionRunService>();
            _mockMapper = new Mock<IMapper>();
            _mockUserManagementService = new Mock<IUserManagementService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            _mockConfiguration.Setup(c => c["AppSettings:MMDetection:BaseSaveMMDetectionDirectoryAbsPath"])
                .Returns(@"C:\vs_code_workspaces\mmdetection\mmdetection\ins_development");
            _mockConfiguration.Setup(c => c["AppSettings:MVC:BaseDetectionRunCopyVisualizedOutputImagesDirectoryPath"])
                .Returns(@"detection-runs\outputs\visualized-images");
            _mockConfiguration.Setup(c => c["AppSettings:MVC:BaseDetectionRunInputImagesSaveDirectoryPath"])
                .Returns(@"detection-runs\input-images");

            _controller = new DetectionController(
                _mockUserManagementService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockWebHostEnvironment.Object,
                _mockAppSettingsAccessor.Object,
                _mockDetectionRunService.Object);
        }

        [Fact]
        public async Task DetectedZones_ReturnsViewResult_WithExpectedModel()
        {
            // Arrange
            var detectionRunsListDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "TestRun1" },
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "TestRun2" }
            };

            var detectionRunsViewModelList = new List<DetectionRunViewModel>
            {
                new DetectionRunViewModel { Id = detectionRunsListDTOs[0].Id, Name = "TestRun1" },
                new DetectionRunViewModel { Id = detectionRunsListDTOs[1].Id, Name = "TestRun2" }
            };

            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRunsListDTOs);
            _mockMapper.Setup(mapper => mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs))
                .Returns(detectionRunsViewModelList);

            // Act
            var result = await _controller.DetectedZones();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<DetectionRunViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
            Assert.Equal("TestRun1", model[0].Name);
            Assert.Equal("TestRun2", model[1].Name);
        }

        [Fact]
        public async Task DetectedZones_ThrowsException_WhenServiceReturnsNull()
        {
            // Arrange
            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync((List<DetectionRunDTO>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.DetectedZones());
        }

        [Fact]
        public async Task DetectedZones_ThrowsException_WhenMappingReturnsNull()
        {
            // Arrange
            var detectionRunsListDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "TestRun1" },
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "TestRun2" }
            };

            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRunsListDTOs);
            _mockMapper.Setup(mapper => mapper.Map<List<DetectionRunViewModel>>(detectionRunsListDTOs))
                .Returns((List<DetectionRunViewModel>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.DetectedZones());
        }

        [Fact]
        public async Task DetectedZones_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ThrowsAsync(new Exception("Service exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.DetectedZones());
        }

        [Fact]
        public async Task GetAllDetectionRuns_ReturnsListOfViewModels_WhenDataIsAvailable()
        {
            // Arrange
            var detectionRunDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "Run 1", Description = "Description 1" },
                new DetectionRunDTO { Id = Guid.NewGuid(), Name = "Run 2", Description = "Description 2" }
            };

            var detectionRunViewModels = new List<DetectionRunViewModel>
            {
                new DetectionRunViewModel { Id = detectionRunDTOs[0].Id, Name = "Run 1", Description = "Description 1" },
                new DetectionRunViewModel { Id = detectionRunDTOs[1].Id, Name = "Run 2", Description = "Description 2" }
            };

            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRunDTOs);

            _mockMapper.Setup(mapper => mapper.Map<List<DetectionRunViewModel>>(detectionRunDTOs))
                .Returns(detectionRunViewModels);

            // Act
            var result = await _controller.GetAllDetectionRuns();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(detectionRunViewModels, result);
        }

        [Fact]
        public async Task GetAllDetectionRuns_ThrowsException_WhenServiceReturnsNull()
        {
            // Arrange
            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync((List<DetectionRunDTO>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetAllDetectionRuns());
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task GetAllDetectionRuns_ThrowsException_WhenMappingFails()
        {
            // Arrange
            var detectionRunDTOs = new List<DetectionRunDTO> { new DetectionRunDTO { Id = Guid.NewGuid(), Name = "Run 1" } };

            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRunDTOs);

            _mockMapper.Setup(mapper => mapper.Map<List<DetectionRunViewModel>>(detectionRunDTOs))
                .Returns((List<DetectionRunViewModel>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetAllDetectionRuns());
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsSuccess_WhenDataIsAvailable()
        {
            // Arrange
            var detectionRunId1 = Guid.NewGuid();
            var detectionRunId2 = Guid.NewGuid();
            var detectionRunDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = detectionRunId1, Name = "Run 1" },
                new DetectionRunDTO { Id = detectionRunId2, Name = "Run 2" }
            };

            var resultDTO = ResultDTO<List<DetectionRunDTO>>.Ok(detectionRunDTOs);
            _mockDetectionRunService.Setup(service => service.GetSelectedDetectionRunsIncludingDetectedDumpSites(It.IsAny<List<Guid>>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.ShowDumpSitesOnMap(new List<Guid> { detectionRunId1, detectionRunId2 });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(detectionRunDTOs, result.Data);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFail_WhenServiceFails()
        {
            // Arrange
            var errorMessage = "Error retrieving detection runs";
            var resultDTO = ResultDTO<List<DetectionRunDTO>>.Fail(errorMessage);
            _mockDetectionRunService.Setup(service => service.GetSelectedDetectionRunsIncludingDetectedDumpSites(It.IsAny<List<Guid>>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.ShowDumpSitesOnMap(new List<Guid> { Guid.NewGuid() });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var resultDTO = ResultDTO<List<DetectionRunDTO>>.Ok(null as List<DetectionRunDTO>);
            _mockDetectionRunService.Setup(service => service.GetSelectedDetectionRunsIncludingDetectedDumpSites(It.IsAny<List<Guid>>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.ShowDumpSitesOnMap(new List<Guid> { Guid.NewGuid() });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateReport_ReturnsData_WhenDataIsAvailable()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var detectionRunsIds = new List<Guid> { detectionRunId };
            var detectionRunDTOs = new List<AreaComparisonAvgConfidenceRateReportDTO>
            {
                new AreaComparisonAvgConfidenceRateReportDTO
                {
                    DetectionRunId = detectionRunId,
                    DetectionRunName = "Run 1",
                    GroupedDumpSitesList = new List<GroupedDumpSitesListHistoricDataDTO>
                    {
                        new GroupedDumpSitesListHistoricDataDTO
                        {
                            ClassName = "Class A",
                            GeomAreas = new List<double> { 100 }
                        }
                    }
                }
            };

            _mockDetectionRunService.Setup(service => service.GenerateAreaComparisonAvgConfidenceRateData(It.IsAny<List<Guid>>()))
                .ReturnsAsync(detectionRunDTOs);

            // Act
            var result = await _controller.GenerateAreaComparisonAvgConfidenceRateReport(detectionRunsIds);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(detectionRunId, result.First().DetectionRunId);
        }

        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateReport_ReturnsEmptyList_WhenNoData()
        {
            // Arrange
            var detectionRunsIds = new List<Guid>();

            _mockDetectionRunService.Setup(service => service.GenerateAreaComparisonAvgConfidenceRateData(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<AreaComparisonAvgConfidenceRateReportDTO>());

            // Act
            var result = await _controller.GenerateAreaComparisonAvgConfidenceRateReport(detectionRunsIds);

            // Assert
            Assert.Empty(result);
        }

        #region InputImages

        [Fact]
        public async Task ViewAllImages_ReturnsViewWithImages_WhenServiceReturnsSuccess()
        {
            // Arrange
            var dtoList = new List<DetectionInputImageDTO> { new DetectionInputImageDTO() };
            var vmList = new List<DetectionInputImageViewModel> { new DetectionInputImageViewModel() };

            _mockDetectionRunService
                .Setup(service => service.GetAllImages())
                .ReturnsAsync(ResultDTO<List<DetectionInputImageDTO>>.Ok(dtoList));

            _mockMapper
                .Setup(m => m.Map<List<DetectionInputImageViewModel>>(dtoList))
                .Returns(vmList);

            // Act
            var result = await _controller.ViewAllImages();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<DetectionInputImageViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(vmList.Count, model.Count);
        }

        [Fact]
        public async Task ViewAllImages_RedirectsToErrorView_WhenServiceReturnsFailure()
        {
            // Arrange
            var errorPath = "/Error";
            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns(errorPath);

            _mockDetectionRunService
                .Setup(service => service.GetAllImages())
                .ReturnsAsync(ResultDTO<List<DetectionInputImageDTO>>.Fail("Error occurred"));

            // Act
            var result = await _controller.ViewAllImages();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(errorPath, redirectResult.Url);
        }

        [Fact]
        public async Task ViewAllImages_ReturnsBadRequest_WhenErrorPathIsNull()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(service => service.GetAllImages())
                .ReturnsAsync(ResultDTO<List<DetectionInputImageDTO>>.Fail("Error occurred"));

            _mockConfiguration.Setup(c => c["ErrorViewsPath:Error"]).Returns((string)null);

            // Act
            var result = await _controller.ViewAllImages();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetDetectionInputImageById_ReturnsFailResult_WhenServiceReturnsFailure()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            _mockDetectionRunService
                .Setup(service => service.GetDetectionInputImageById(detectionInputImageId))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Fail("Error occurred"));

            // Act
            var result = await _controller.GetDetectionInputImageById(detectionInputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error occurred", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionInputImageById_ReturnsFailResult_WhenDataIsNull()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            _mockDetectionRunService
                .Setup(service => service.GetDetectionInputImageById(detectionInputImageId))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Ok(null));

            // Act
            var result = await _controller.GetDetectionInputImageById(detectionInputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image Input is null", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionInputImageById_ReturnsOkResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            var detectionInputImageDTO = new DetectionInputImageDTO { Id = detectionInputImageId };

            _mockDetectionRunService
                .Setup(service => service.GetDetectionInputImageById(detectionInputImageId))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Ok(detectionInputImageDTO));

            // Act
            var result = await _controller.GetDetectionInputImageById(detectionInputImageId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(detectionInputImageDTO, result.Data);
        }

        //[Fact]
        //public async Task EditDetectionImageInput_WithValidModel_ReturnsOkResult()
        //{
        //    // Arrange
        //    var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid() };
        //    var dto = new DetectionInputImageDTO { Id = viewModel.Id };
        //    var userId = "test-user-id";

        //    _mockMapper.Setup(m => m.Map<DetectionInputImageDTO>(viewModel)).Returns(dto);
        //    _mockDetectionRunService.Setup(s => s.EditDetectionInputImage(dto)).ReturnsAsync(ResultDTO.Ok());


        //    // Act
        //    var result = await _controller.EditDetectionImageInput(viewModel);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //}

        //[Fact]
        //public async Task EditDetectionImageInput_WithMappingFailure_ReturnsFailResult()
        //{
        //    // Arrange
        //    var userId = "test-user-id";
        //    var viewModel = new DetectionInputImageViewModel();

        //    _mockMapper.Setup(m => m.Map<DetectionInputImageDTO>(viewModel)).Returns((DetectionInputImageDTO)null)
        //        ;
        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new("UserId", userId) }))
        //        }
        //    };

        //    // Act
        //    var result = await _controller.EditDetectionImageInput(viewModel);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Contains("Mapping failed", result.ErrMsg);
        //}

        [Fact]
        public async Task EditDetectionImageInput_UserNotAuthenticated_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()
                    )
                }
            };
            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User id not found", result.ErrMsg);
        }

        //[Fact]
        //public async Task EditDetectionImageInput_WithInvalidModel_ReturnsFailResult()
        //{
        //    // Arrange
        //    var userId = "test-user-id";
        //    var viewModel = new DetectionInputImageViewModel();

        //    _controller.ModelState.AddModelError("Name", "Name is required");

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity([new("UserId", userId)]))
        //        }
        //    };


        //    // Act
        //    var result = await _controller.EditDetectionImageInput(viewModel);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Contains("Name is required", result.ErrMsg);
        //}

        //[Fact]
        //public async Task EditDetectionImageInput_ServiceReturnsFailure_ReturnsFailResult()
        //{
        //    // Arrange
        //    var userId = "test-user-id";
        //    var viewModel = new DetectionInputImageViewModel();
        //    var dto = new DetectionInputImageDTO();

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity([new("UserId", userId)]))
        //        }
        //    };

        //    _mockMapper.Setup(m => m.Map<DetectionInputImageDTO>(viewModel)).Returns(dto);
        //    _mockDetectionRunService.Setup(s => s.EditDetectionInputImage(dto))
        //        .ReturnsAsync(ResultDTO.Fail("Edit failed"));

        //    // Act
        //    var result = await _controller.EditDetectionImageInput(viewModel);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal("Edit failed", result.ErrMsg);
        //}

        [Fact]
        public async Task DeleteDetectionImageInput_WithInvalidModel_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            _controller.ModelState.AddModelError("Id", "Id is required");

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Id is required", result.ErrMsg);
        }

        //[Fact]
        //public async Task DeleteDetectionImageInput_MappingFailure_ReturnsFailResult()
        //{
        //    // Arrange
        //    var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid() };
        //    _mockMapper.Setup(m => m.Map<DetectionInputImageDTO>(viewModel)).Returns((DetectionInputImageDTO)null);

        //    // Act
        //    var result = await _controller.DeleteDetectionImageInput(viewModel);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Contains("Mapping failed", result.ErrMsg);
        //}


        #endregion

    }
}
