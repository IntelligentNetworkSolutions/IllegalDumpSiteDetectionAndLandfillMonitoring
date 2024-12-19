using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using Hangfire;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using Services.Interfaces.Services;
using System.Reflection;
using System.Security.Claims;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DetectionControllerTests
    {

        private readonly Mock<IDetectionRunService> _mockDetectionRunService;
        private readonly Mock<IDetectionIgnoreZoneService> _mockDetectionIgnoreZoneService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<ITrainedModelService> _mockTrainedModelService;
        private readonly DetectionController _controller;

        public DetectionControllerTests()
        {
            _mockDetectionRunService = new Mock<IDetectionRunService>();
            _mockDetectionIgnoreZoneService = new Mock<IDetectionIgnoreZoneService>();
            _mockMapper = new Mock<IMapper>();
            _mockUserManagementService = new Mock<IUserManagementService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockTrainedModelService = new Mock<ITrainedModelService>();

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
                _mockDetectionRunService.Object,
                _mockBackgroundJobClient.Object,
                _mockDetectionIgnoreZoneService.Object,
                _mockTrainedModelService.Object);

            var claims = new List<Claim>
            {
                new Claim("UserId", "test-user-id")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

        }



        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsFail_WhenDataIsUninitialized()
        {
            // Arrange
            var result = ResultDTO<List<TrainedModelDTO>>.Ok(null);

            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Null(response.Data);
            Assert.Equal("Trained model list not found", response.ErrMsg);
        }

        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsFail_WhenDataIsEmptyButIsSuccessTrue()
        {
            // Arrange
            var result = ResultDTO<List<TrainedModelDTO>>.Ok(new List<TrainedModelDTO>());

            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Empty(response.Data);
        }

        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsOkResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var trainedModels = new List<TrainedModelDTO>
            {
                new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model 1" },
                new TrainedModelDTO { Id = Guid.NewGuid(), Name = "Model 2" }
            };

            var result = ResultDTO<List<TrainedModelDTO>>.Ok(trainedModels);
            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal(trainedModels, response.Data);
        }

        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsExceptionFailResult_WhenServiceThrowsException()
        {
            // Arrange
            var exceptionMessage = "An unexpected error occurred";
            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal(exceptionMessage, response.ErrMsg);
            Assert.IsType<Exception>(response.ExObj);
        }

        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsFailResult_WhenServiceReturnsNullData()
        {
            // Arrange
            var result = ResultDTO<List<TrainedModelDTO>>.Ok(null); 

            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess); 
            Assert.Null(response.Data);
            Assert.Equal("Trained model list not found", response.ErrMsg); 
        }


        [Fact]
        public async Task GetPublishedTrainedModels_ReturnsFailResult_WhenServiceReturnsSuccessWithEmptyData()
        {
            // Arrange
            var result = ResultDTO<List<TrainedModelDTO>>.Ok(new List<TrainedModelDTO>());

            _mockTrainedModelService.Setup(service => service.GetPublishedTrainedModelsIncludingTrainRuns())
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetPublishedTrainedModels();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Empty(response.Data);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsOkResult_WhenDetectionRunExists()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var detectionRunDTO = new DetectionRunDTO();
            var result = ResultDTO<DetectionRunDTO>.Ok(detectionRunDTO);

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                        .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal(detectionRunDTO, response.Data);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailResult_WhenDetectionRunNotFound()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var result = ResultDTO<DetectionRunDTO>.Fail("Detection run not found");

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                      .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Detection run not found", response.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var result = ResultDTO<DetectionRunDTO>.Fail("Service error");

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                        .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Service error", response.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailResult_WhenDataIsNull()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var result = ResultDTO<DetectionRunDTO>.Ok(null);

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                        .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Detection run not found", response.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailResult_WhenServiceReturnsErrorWithoutMessage()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var result = ResultDTO<DetectionRunDTO>.Fail(null);

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                        .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailResult_WhenDetectionRunIdIsDefaultGuid()
        {
            // Arrange
            var detectionRunId = Guid.Empty;
            var result = ResultDTO<DetectionRunDTO>.Fail("Detection run not found");

            _mockDetectionRunService.Setup(service => service.GetDetectionRunById(detectionRunId, false))
                        .ReturnsAsync(result);

            // Act
            var response = await _controller.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("Detection run not found", response.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionRun_ReturnsFail_WhenDetectionRunIdIsEmpty()
        {
            // Act
            var result = await _controller.DeleteDetectionRun(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid detection run id", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunErrorLogMessage_ShouldReturnFail_WhenAppSettingsFail()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to retrieve settings"));

            // Act
            var result = await _controller.GetDetectionRunErrorLogMessage(detectionRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionRunErrorLogMessage_ShouldReturnFail_WhenDirectoryPathIsEmpty()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(string.Empty));

            // Act
            var result = await _controller.GetDetectionRunErrorLogMessage(detectionRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Directory path not found", result.ErrMsg);
        }

        //[Fact]
        //public async Task GetDetectionRunErrorLogMessage_ReturnsFileContent_WhenFileExists()
        //{
        //    // Arrange
        //    var detectionRunId = Guid.NewGuid();
        //    var filePath = Path.Combine("Uploads", "DetectionUploads", "DetectionRunsErrorLogs");
        //    var fileName = $"{detectionRunId}_errMsg.txt";

        //    _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", It.IsAny<string>()))
        //        .ReturnsAsync(ResultDTO<string>.Ok(filePath));

        //    _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("wwwroot");

        //    var fullFilePath = Path.Combine("wwwroot", filePath, fileName);

        //    File.WriteAllText(fullFilePath, "Sample error message");

        //    // Act
        //    var result = await _controller.GetDetectionRunErrorLogMessage(detectionRunId);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal("Sample error message", result.Data);

        //    // Clean up
        //    File.Delete(fullFilePath);
        //}

        [Fact]
        public async Task StartDetectionRun_ShouldReturnOk_WhenAllStepsSucceed()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = detectionRunId,
                TrainedModelId = Guid.NewGuid(),
                InputImgPath = "some/path"
            };

            _mockDetectionRunService
                .Setup(x => x.UpdateStatus(detectionRunDTO.Id.Value, It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockTrainedModelService
                .Setup(x => x.GetTrainedModelById(detectionRunDTO.TrainedModelId.Value, false))
                .ReturnsAsync(ResultDTO<TrainedModelDTO>.Ok(new TrainedModelDTO()));

            _mockDetectionRunService
                .Setup(x => x.StartDetectionRun(It.IsAny<DetectionRunDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDetectionRunService
                .Setup(x => x.IsCompleteUpdateDetectionRun(It.IsAny<DetectionRunDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDetectionRunService
                .Setup(x => x.GetRawDetectionRunResultPathsByRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<string>.Ok("some/path"));

            _mockDetectionRunService
                .Setup(x => x.GetBBoxResultsDeserialized(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<DetectionRunFinishedResponse>.Ok(new DetectionRunFinishedResponse()));

            _mockDetectionRunService
                .Setup(x => x.ConvertBBoxResultToImageProjection(It.IsAny<string>(), It.IsAny<DetectionRunFinishedResponse>()))
                .ReturnsAsync(ResultDTO<DetectionRunFinishedResponse>.Ok(new DetectionRunFinishedResponse()));

            _mockDetectionRunService
                .Setup(x => x.CreateDetectedDumpsSitesFromDetectionRun(It.IsAny<Guid>(), It.IsAny<DetectionRunFinishedResponse>()))
                .ReturnsAsync(ResultDTO<List<DetectedDumpSite>>.Ok(new List<DetectedDumpSite>()));

            // Act
            var result = await _controller.StartDetectionRun(detectionRunDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(detectionRunId.ToString(), result.Data);
        }

        [Fact]
        public async Task StartDetectionRun_ShouldReturnExceptionFail_WhenExceptionOccurs()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid(),
                InputImgPath = "some/path"
            };

            _mockDetectionRunService
                .Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Some exception"));

            // Act
            var result = await _controller.StartDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Some exception", result.ErrMsg);
        }

        [Fact]
        public async Task CreateErrMsgFile_ShouldReturnFail_WhenAppSettingFails()
        {
            // Arrange
            Guid detectionRunId = Guid.NewGuid();
            string errMsg = "Error message";
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Error fetching setting"));

            // Use reflection to call private method
            var privateMethod = typeof(DetectionController)
                .GetMethod("CreateErrMsgFile", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var resultTask = (Task<ResultDTO>)privateMethod.Invoke(_controller, new object[] { detectionRunId, errMsg });
            var result = await resultTask;

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task CreateErrMsgFile_ShouldReturnFail_WhenDirectoryPathIsEmpty()
        {
            // Arrange
            Guid detectionRunId = Guid.NewGuid();
            string errMsg = "Error message";
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>("DetectionRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(string.Empty));

            // Use reflection to call private method
            var privateMethod = typeof(DetectionController)
                .GetMethod("CreateErrMsgFile", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var resultTask = (Task<ResultDTO>)privateMethod.Invoke(_controller, new object[] { detectionRunId, errMsg });
            var result = await resultTask;

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Directory path not found", result.ErrMsg);
        }

        [Fact]
        public async Task ScheduleDetectionRun_InvalidModelState_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required.");
            var viewModel = new DetectionRunViewModel();

            // Act
            var result = await _controller.ScheduleDetectionRun(viewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Name is required.", failResult.ErrMsg);
        }

        [Fact]
        public async Task ScheduleDetectionRun_InputImageNotFound_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionRunViewModel
            {
                SelectedInputImageId = Guid.NewGuid(),
                Name = "Test Run",
                Description = "Test Description"
            };

            _mockDetectionRunService
                .Setup(s => s.GetDetectionInputImageById(viewModel.SelectedInputImageId))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Fail("Image not found"));

            // Act
            var result = await _controller.ScheduleDetectionRun(viewModel);

            // Assert
            var failResult = Assert.IsType<ResultDTO>(result);
            Assert.False(failResult.IsSuccess);
            Assert.Equal("Image not found", failResult.ErrMsg);
        }

      
        [Fact]
        public async Task CreateDetectionRun_Returns_ViewResult()
        {
            // Act
            var result = await _controller.CreateDetectionRun();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task CreateDetectionRun_ModelStateIsValid()
        {
            // Act
            var result = await _controller.CreateDetectionRun();

            // Assert
            Assert.IsType<ViewResult>(result);
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
                new DetectionRunViewModel { Id = detectionRunsListDTOs[0].Id, Name = "TestRun1", IsCompleted = true, Status = "Success" },
                new DetectionRunViewModel { Id = detectionRunsListDTOs[1].Id, Name = "TestRun2", IsCompleted = true, Status = "Success" }
            };

            _mockDetectionRunService.Setup(service => service.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRunsListDTOs));

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
        public async Task DetectedZones_RedirectsToError_WhenServiceFails()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(s => s.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Fail("Service error"));

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error"])
                .Returns("/Error");

            // Act
            var result = await _controller.DetectedZones();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task DetectedZones_RedirectsToError404_WhenDataIsNull()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(s => s.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(null));

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error404"])
                .Returns("/Error404");

            // Act
            var result = await _controller.DetectedZones();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }

        [Fact]
        public async Task DetectedZones_RedirectsToError404_WhenViewModelIsNull()
        {
            // Arrange
            var detectionRuns = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), IsCompleted = true, Status = "Success" }
            };

            _mockDetectionRunService
                .Setup(s => s.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));

            _mockMapper
                .Setup(m => m.Map<List<DetectionRunViewModel>>(detectionRuns))
                .Returns((List<DetectionRunViewModel>)null);

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error404"])
                .Returns("/Error404");

            // Act
            var result = await _controller.DetectedZones();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }

        [Fact]
        public async Task DetectedZones_RedirectsToError_WhenExceptionOccurs()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(s => s.GetDetectionRunsWithClasses())
                .ThrowsAsync(new Exception("Test exception"));

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error"])
                .Returns("/Error");

            // Act
            var result = await _controller.DetectedZones();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task GetAllDetectionRuns_SuccessfulScenario_ReturnsFilteredCompletedRuns()
        {
            // Arrange
            var detectionRunDtos = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { IsCompleted = true, Status = "Success" },
                new DetectionRunDTO { IsCompleted = false, Status = "Pending" },
                new DetectionRunDTO { IsCompleted = true, Status = "Failure" }
            };

            var detectionRunViewModels = new List<DetectionRunViewModel>
            {
                new DetectionRunViewModel { IsCompleted = true, Status = "Success" }
            };

            _mockDetectionRunService
                .Setup(x => x.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRunDtos));

            _mockMapper
                .Setup(x => x.Map<List<DetectionRunViewModel>>(detectionRunDtos))
                .Returns(detectionRunViewModels);

            // Act
            var result = await _controller.GetAllDetectionRuns();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Data);
            Assert.All(result.Data, x =>
            {
                Assert.True(x.IsCompleted);
                Assert.Equal("Success", x.Status);
            });
        }

        [Fact]
        public async Task GetAllDetectionRuns_ServiceReturnsError_ReturnsFailure()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(x => x.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Fail("Service error"));

            // Act
            var result = await _controller.GetAllDetectionRuns();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDetectionRuns_NullDataFromService_ReturnsFailure()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(x => x.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(null));

            // Act
            var result = await _controller.GetAllDetectionRuns();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Detection run list not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDetectionRuns_MappingFails_ReturnsFailure()
        {
            // Arrange
            var detectionRunDtos = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { IsCompleted = true, Status = "Success" }
            };

            _mockDetectionRunService
                .Setup(x => x.GetDetectionRunsWithClasses())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRunDtos));

            _mockMapper
                .Setup(x => x.Map<List<DetectionRunViewModel>>(detectionRunDtos))
                .Returns((List<DetectionRunViewModel>)null);

            // Act
            var result = await _controller.GetAllDetectionRuns();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping to list of detection run view model failed", result.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFail_WhenServiceFails()
        {
            // Arrange
            var errorMessage = "Error retrieving detection runs";
            var resultDTO = ResultDTO<List<DetectionRunDTO>>.Fail(errorMessage);

            var model = new DetectionRunShowOnMapViewModel
            {
                selectedDetectionRunsIds = new List<Guid> { Guid.NewGuid() },
                selectedConfidenceRates = new List<ConfidenceRateDTO>
                {
                    new ConfidenceRateDTO { detectionRunId = Guid.NewGuid(), confidenceRate = 0.75 }
                }
            };

            _mockDetectionRunService.Setup(service => service.GetSelectedDetectionRunsIncludingDetectedDumpSites(
                It.IsAny<List<Guid>>(), It.IsAny<List<ConfidenceRateDTO>>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
            _mockDetectionRunService.Verify(service =>
                service.GetSelectedDetectionRunsIncludingDetectedDumpSites(
                    model.selectedDetectionRunsIds, model.selectedConfidenceRates), Times.Once);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFail_WhenDataIsNull()
        {
            // Arrange
            var resultDTO = ResultDTO<List<DetectionRunDTO>>.Ok(null as List<DetectionRunDTO>);

            var model = new DetectionRunShowOnMapViewModel
            {
                selectedDetectionRunsIds = new List<Guid> { Guid.NewGuid() },
                selectedConfidenceRates = null
            };

            _mockDetectionRunService.Setup(service => service.GetSelectedDetectionRunsIncludingDetectedDumpSites(
                It.IsAny<List<Guid>>(), It.IsAny<List<ConfidenceRateDTO>>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No confidence rates selected", result.ErrMsg);
            _mockDetectionRunService.Verify(service =>
                service.GetSelectedDetectionRunsIncludingDetectedDumpSites(
                    model.selectedDetectionRunsIds, It.IsAny<List<ConfidenceRateDTO>>()), Times.Never);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFailResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model state");
            var model = new DetectionRunShowOnMapViewModel();

            // Act
            var response = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Contains("Invalid model state", response.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFailResult_WhenSelectedDetectionRunsIdsIsNull()
        {
            // Arrange
            var model = new DetectionRunShowOnMapViewModel { selectedDetectionRunsIds = null, selectedConfidenceRates = new List<ConfidenceRateDTO>() };

            // Act
            var response = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("No detection run selected", response.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFailResult_WhenSelectedDetectionRunsIdsIsEmpty()
        {
            // Arrange
            var model = new DetectionRunShowOnMapViewModel { selectedDetectionRunsIds = new List<Guid>(), selectedConfidenceRates = new List<ConfidenceRateDTO>() };

            // Act
            var response = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("No detection run selected", response.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFailResult_WhenSelectedConfidenceRatesIsNull()
        {
            // Arrange
            var model = new DetectionRunShowOnMapViewModel { selectedDetectionRunsIds = new List<Guid> { Guid.NewGuid() }, selectedConfidenceRates = null };

            // Act
            var response = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("No confidence rates selected", response.ErrMsg);
        }

        [Fact]
        public async Task ShowDumpSitesOnMap_ReturnsFailResult_WhenSelectedConfidenceRatesIsEmpty()
        {
            // Arrange
            var model = new DetectionRunShowOnMapViewModel { selectedDetectionRunsIds = new List<Guid> { Guid.NewGuid() }, selectedConfidenceRates = new List<ConfidenceRateDTO>() };

            // Act
            var response = await _controller.ShowDumpSitesOnMap(model);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal("No confidence rates selected", response.ErrMsg);
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

            _mockDetectionRunService
                .Setup(service => service.GenerateAreaComparisonAvgConfidenceRateData(It.IsAny<List<Guid>>(), It.IsAny<int>()))
                .ReturnsAsync(ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.Ok(detectionRunDTOs));

            // Act
            var result = await _controller.GenerateAreaComparisonAvgConfidenceRateReport(detectionRunsIds, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(detectionRunId, result.Data.First().DetectionRunId);
        }

        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateReport_ReturnsEmptyList_WhenNoData()
        {
            // Arrange
            var detectionRunsIds = new List<Guid>();

            // Corrected mock setup for GenerateAreaComparisonAvgConfidenceRateData
            _mockDetectionRunService
                .Setup(service => service.GenerateAreaComparisonAvgConfidenceRateData(It.IsAny<List<Guid>>(), It.IsAny<int>()))
                .ReturnsAsync(ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>.Ok(new List<AreaComparisonAvgConfidenceRateReportDTO>()));


            // Act
            var result = await _controller.GenerateAreaComparisonAvgConfidenceRateReport(detectionRunsIds, 1);

            // Assert
            Assert.Empty(result.Data);
        }

        #region InputImages

        [Fact]
        public async Task DetectionInputImages_ReturnsViewWithImages_WhenServiceReturnsSuccess()
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
            var result = await _controller.DetectionInputImages();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<DetectionInputImageViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(vmList.Count, model.Count);
        }

        [Fact]
        public async Task DetectionInputImages_ServiceFailure_ReturnsError404()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Fail("Service failed");            

            _mockDetectionRunService
                .Setup(s => s.GetAllImages())
                .ReturnsAsync(resultDto);

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error404"])
                .Returns("/error/404");

            // Act
            var result = await _controller.DetectionInputImages();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error/404", redirectResult.Url);
        }

        [Fact]
        public async Task DetectionInputImages_MapperReturnsNull_ReturnsError404()
        {
            // Arrange
            var mockImageDtos = new List<DetectionInputImageDTO>
            {
                new DetectionInputImageDTO { Id = Guid.NewGuid(), Name = "Image1" }
            };

            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Ok(mockImageDtos);            

            _mockDetectionRunService
                .Setup(s => s.GetAllImages())
                .ReturnsAsync(resultDto);

            _mockMapper
                .Setup(m => m.Map<List<DetectionInputImageViewModel>>(mockImageDtos))
                .Returns((List<DetectionInputImageViewModel>)null);

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error404"])
                .Returns("/error/404");

            // Act
            var result = await _controller.DetectionInputImages();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error/404", redirectResult.Url);
        }

        [Fact]
        public async Task DetectionInputImages_ExceptionThrown_ReturnsError400()
        {
            // Arrange
            _mockDetectionRunService
                .Setup(s => s.GetAllImages())
                .ThrowsAsync(new Exception("Test exception"));

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error"])
                .Returns("/error/general");

            // Act
            var result = await _controller.DetectionInputImages();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/error/general", redirectResult.Url);
        }

        [Fact]
        public async Task DetectionInputImages_NoConfiguredErrorPath_ReturnsAppropriateStatusCode()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Fail("Configuration error path");
            
            _mockDetectionRunService
                .Setup(s => s.GetAllImages())
                .ReturnsAsync(resultDto);

            _mockConfiguration
                .Setup(c => c["ErrorViewsPath:Error404"])
                .Returns((string)null);

            // Act
            var result = await _controller.DetectionInputImages();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
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
            var detectionInputImageDTO = new DetectionInputImageDTO
            {
                Id = detectionInputImageId,
                ImageFileName = "image.jpg"
            };

            _mockDetectionRunService
                .Setup(service => service.GetDetectionInputImageById(detectionInputImageId))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Ok(detectionInputImageDTO));

            var thumbnailsFolderPath = "Uploads\\DetectionUploads\\InputImageThumbnails";
            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(thumbnailsFolderPath));

            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            var mockUriHelper = new Mock<IUrlHelper>();

            mockRequest.Setup(x => x.Scheme).Returns("https");
            mockRequest.Setup(x => x.Host).Returns(new HostString("localhost"));
            mockRequest.Setup(x => x.PathBase).Returns(new PathString("/"));

            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = await _controller.GetDetectionInputImageById(detectionInputImageId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(detectionInputImageDTO.Id, result.Data.Id);
            Assert.NotNull(result.Data.ThumbnailFilePath);
            Assert.Contains("_thumbnail.jpg", result.Data.ThumbnailFilePath);
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

        [Fact]
        public async Task GetAllDetectionInputImages_ReturnsOkResult_WhenImagesExist()
        {
            // Arrange
            var images = new List<DetectionInputImageDTO> { new DetectionInputImageDTO { Id = Guid.NewGuid(), ImagePath = "path1" } };
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Ok(images);

            _mockDetectionRunService.Setup(service => service.GetAllImages())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAllDetectionInputImages();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(images, result.Data);
        }

        [Fact]
        public async Task GetAllDetectionInputImages_ReturnsFailResult_WhenImagesNotFound()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Ok(null);

            _mockDetectionRunService.Setup(service => service.GetAllImages())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAllDetectionInputImages();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Detection input images are not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDetectionInputImages_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Fail("Service failure");

            _mockDetectionRunService.Setup(service => service.GetAllImages())
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetAllDetectionInputImages();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failure", result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedDetectionInputImages_ReturnsOkResult_WhenImagesExist()
        {
            // Arrange
            var selectedImageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var images = new List<DetectionInputImageDTO>
            {
                new DetectionInputImageDTO { Id = selectedImageIds[0], ImagePath = "path1" },
                new DetectionInputImageDTO { Id = selectedImageIds[1], ImagePath = "path2" }
            };
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Ok(images);

            _mockDetectionRunService.Setup(service => service.GetSelectedInputImagesById(selectedImageIds))
                .ReturnsAsync(resultDto);

            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();

            mockRequest.Setup(x => x.Scheme).Returns("https");
            mockRequest.Setup(x => x.Host).Returns(new HostString("localhost"));
            mockRequest.Setup(x => x.PathBase).Returns(new PathString("/"));

            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = await _controller.GetSelectedDetectionInputImages(selectedImageIds);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(images, result.Data);
            Assert.All(result.Data, item => Assert.Contains("/", item.ImagePath));
        }


        [Fact]
        public async Task GetSelectedDetectionInputImages_ReturnsFailResult_WhenImagesNotFound()
        {
            // Arrange
            var selectedImageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Ok(null);

            _mockDetectionRunService.Setup(service => service.GetSelectedInputImagesById(selectedImageIds))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetSelectedDetectionInputImages(selectedImageIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Detection input images are not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedDetectionInputImages_ReturnsFailResult_WhenServiceFails()
        {
            // Arrange
            var selectedImageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var resultDto = ResultDTO<List<DetectionInputImageDTO>>.Fail("Service failure");

            _mockDetectionRunService.Setup(service => service.GetSelectedInputImagesById(selectedImageIds))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.GetSelectedDetectionInputImages(selectedImageIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service failure", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_InvalidModelState_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("test", "Test error");
            var viewModel = new DetectionInputImageViewModel();

            // Act
            var result = await _controller.AddImage(viewModel, null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test error", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_FailedToGetAppSettings_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Fail("Settings error"));

            // Act
            var result = await _controller.AddImage(viewModel, null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_EmptyFolderPath_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Ok(null));

            // Act
            var result = await _controller.AddImage(viewModel, null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image folder path not found", result.ErrMsg);
        }


        [Fact]
        public async Task EditDetectionImageInput_InvalidModelState_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("test", "Test error");
            var viewModel = new DetectionInputImageViewModel();

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test error", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionImageInput_NoUserClaims_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User id not found", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionImageInput_UserNotFound_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            _mockUserManagementService
                .Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok((UserDTO?)null));

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionImageInput_MappingFailed_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            var userDto = new UserDTO { Id = "test-user-id" };

            _mockUserManagementService
                .Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(userDto));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns((DetectionInputImageDTO?)null);

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionImageInput_EditServiceFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            var userDto = new UserDTO { Id = "test-user-id" };
            var dto = new DetectionInputImageDTO();

            _mockUserManagementService
                .Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(userDto));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(dto);
            _mockDetectionRunService
                .Setup(x => x.EditDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Edit failed"));

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Edit failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionImageInput_SuccessfulEdit_ReturnsSuccessResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            var userDto = new UserDTO { Id = "test-user-id" };
            var dto = new DetectionInputImageDTO();

            _mockUserManagementService
                .Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(userDto));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(dto);
            _mockDetectionRunService
                .Setup(x => x.EditDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditDetectionImageInput_VerifyUpdateFields_SetsCorrectValues()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel();
            var userDto = new UserDTO { Id = "test-user-id" };
            var dto = new DetectionInputImageDTO();
            var startTime = DateTime.UtcNow;

            _mockUserManagementService
                .Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok((userDto)));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(dto);
            _mockDetectionRunService
                .Setup(x => x.EditDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.EditDetectionImageInput(viewModel);
            var endTime = DateTime.UtcNow;

            // Assert
            Assert.Equal("test-user-id", viewModel.UpdatedById);
            Assert.True(viewModel.UpdatedOn >= startTime);
            Assert.True(viewModel.UpdatedOn <= endTime);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_InvalidModelState_ReturnsFailResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("test", "Test error");
            var viewModel = new DetectionInputImageViewModel();

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test error", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_GetDetectionRunsFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid(), ImageFileName = "image.tif" };
            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Fail("Failed to get detection runs"));

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get detection runs", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_DetectionRunDataNull_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid(), ImageFileName = "image.tif" };
            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(null!));

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_ImageStillInUse_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid(), ImageFileName = "image.tif" };
            var detectionRuns = new List<DetectionRunDTO> { new DetectionRunDTO() };
            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("This image is still used in the detection run", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_MappingFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid(), ImageFileName = "image.tif" };
            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(new List<DetectionRunDTO>()));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns((DetectionInputImageDTO?)null);

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_DeleteServiceFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel { Id = Guid.NewGuid(), ImageFileName = "image.tif" };
            var dto = new DetectionInputImageDTO();

            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(new List<DetectionRunDTO>()));
            _mockMapper
                .Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(dto);
            _mockDetectionRunService
                .Setup(x => x.DeleteDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_InvalidFileExtension_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel
            {
                Id = Guid.NewGuid(),
                ImageFileName = "invalid_image.jpg"
            };

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid image extension .jpg", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionImageInput_ValidFilePathAndExtension_ProceedsWithNextSteps()
        {
            // Arrange
            var viewModel = new DetectionInputImageViewModel
            {
                ImagePath = "uploads/valid_image.tif", 
                ImageFileName = "valid_image.tif"
            };

            _mockWebHostEnvironment
                .Setup(env => env.WebRootPath)
                .Returns("/var/www/app_root");

            _mockDetectionRunService
                .Setup(x => x.GetDetectionInputImageByDetectionRunId(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(new List<DetectionRunDTO>()));

            // Act
            var result = await _controller.DeleteDetectionImageInput(viewModel);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GenerateThumbnail_InvalidPath_ReturnsErrorMessage()
        {
            // Arrange
            string absGeoTiffPath = null;

            // Act
            var result = await _controller.GenerateThumbnail(absGeoTiffPath) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("isSuccess").GetValue(result.Value));
            Assert.Equal("Invalid image path.", result.Value.GetType().GetProperty("errMsg").GetValue(result.Value));
        }

        [Fact]
        public async Task GenerateThumbnail_AppSettingError_ReturnsErrorMessage()
        {
            // Arrange
            string absGeoTiffPath = "valid/path/to/image.tif";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Fail("Failed"));

            // Act
            var result = await _controller.GenerateThumbnail(absGeoTiffPath) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("isSuccess").GetValue(result.Value));
            Assert.Equal("Cannot get the application setting for thumbnails folder", result.Value.GetType().GetProperty("errMsg").GetValue(result.Value));
        }

        [Fact]
        public async Task GenerateThumbnail_NullThumbnailsFolder_ReturnsErrorMessage()
        {
            // Arrange
            string absGeoTiffPath = "valid/path/to/image.tif";
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>("DetectionInputImageThumbnailsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Ok(null));

            // Act
            var result = await _controller.GenerateThumbnail(absGeoTiffPath) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result.Value.GetType().GetProperty("isSuccess").GetValue(result.Value));
            Assert.Equal("Detection input image thumbnails folder value is null", result.Value.GetType().GetProperty("errMsg").GetValue(result.Value));
        }

        [Fact]
        public async Task AddImage_InvalidModelState_ReturnsFail()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "test error");
            var viewModel = new DetectionInputImageViewModel();

            // Act
            var result = await _controller.AddImage(viewModel, null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("test error", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_ApplicationSettingsFailure_ReturnsFail()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>(
                "DetectionInputImagesFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Fail("Settings error"));

            // Act
            var result = await _controller.AddImage(new DetectionInputImageViewModel(), null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_EmptyFolderPath_ReturnsFail()
        {
            // Arrange
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>(
                "DetectionInputImagesFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Ok(string.Empty));

            // Act
            var result = await _controller.AddImage(new DetectionInputImageViewModel(), null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image folder path not found", result.ErrMsg);
        }

        [Fact]
        public async Task AddImage_UserIdIsMissing_ReturnsFailure()
        {
            // Arrange
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok("SomeFolder"));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(new DetectionInputImageDTO());

            var mockDetectionRunService = new Mock<IDetectionRunService>();
            mockDetectionRunService.Setup(x => x.CreateDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Ok(new DetectionInputImageDTO()));

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.WebRootPath).Returns("C:\\wwwroot");

            var viewModel = new DetectionInputImageViewModel();
            IFormFile file = null;

            // Create a user with NO UserId claim
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await _controller.AddImage(viewModel, file);

            // Assert
            Assert.False(result.IsSuccess);
        }


        [Fact]
        public async Task AddImage_SuccessfulFileUpload_ReturnsSuccess()
        {
            // Arrange
            const string webRootPath = "/webroot";
            const string uploadFolder = "Uploads/DetectionUploads/InputImages";
            const string fileName = "test.jpg";

            SetupSuccessfulUserAndSettings();

            Mock<IWebHostEnvironment> mockWebEnv = new Mock<IWebHostEnvironment>();
            mockWebEnv.Setup(x => x.WebRootPath).Returns(Path.Combine(Path.GetTempPath(), "mockwebhostenv"));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns(Task.CompletedTask);

            var dto = new DetectionInputImageDTO { ImagePath = Path.Combine(uploadFolder, fileName) };
            _mockMapper.Setup(x => x.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImageViewModel>()))
                .Returns(dto);

            _mockDetectionRunService.Setup(x => x.CreateDetectionInputImage(It.IsAny<DetectionInputImageDTO>()))
                .ReturnsAsync(ResultDTO<DetectionInputImageDTO>.Ok(dto));

            DetectionController controla = new DetectionController(
                _mockUserManagementService.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                mockWebEnv.Object,
                _mockAppSettingsAccessor.Object,
                _mockDetectionRunService.Object,
                _mockBackgroundJobClient.Object,
                _mockDetectionIgnoreZoneService.Object,
                _mockTrainedModelService.Object);

            List<Claim> claims = [new Claim("UserId", "test-user-id")];
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            controla.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };

            // Act
            ResultDTO<string> result = await controla.AddImage(new DetectionInputImageViewModel(), fileMock.Object);

            // Assert
            Assert.True(result.IsSuccess);
        }

        private void SetupSuccessfulUserAndSettings()
        {
            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>(
                "DetectionInputImagesFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Ok("Uploads/DetectionUploads/InputImages"));

            Mock<ClaimsPrincipal> _mockUser = new Mock<ClaimsPrincipal>();
            _mockUser.Setup(x => x.FindFirst("UserId"))
                .Returns(new Claim("UserId", "testId"));

            _mockUserManagementService.Setup(x => x.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(new UserDTO { Id = "testId" }));

            _mockWebHostEnvironment.Setup(x => x.WebRootPath)
                .Returns("/webroot");
        }
        #endregion

    }
}
