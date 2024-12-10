using AutoMapper;
using DAL.Interfaces.Helpers;
using Hangfire;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.BL.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.BL.Interfaces.Services.DetectionServices;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Training;
using Microsoft.AspNetCore.Mvc;
using SD;
using Entities.TrainingEntities;
using DTOs.MainApp.BL;
using SD.Enums;
using System.Linq.Expressions;
using System.Security.Claims;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Hangfire.Common;
using Hangfire.Storage.Monitoring;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.ObjectDetection.API;
using MainApp.BL.Services.TrainingServices;
using DAL.Interfaces.Repositories.TrainingRepositories;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class TrainingRunsControllerTests
    {
        private readonly Mock<ITrainingRunService> _mockTrainingRunService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IDatasetService> _mockDatasetService;
        private readonly Mock<IMMDetectionConfigurationService> _mockMMDetectionConfiguration;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IStorageConnection> _mockStorageConnection;
        private readonly Mock<ITrainingRunTrainParamsService> _mockTrainingRunTrainParamsService;
        private readonly TrainingRunsController _controller;
        private readonly string _userId = "test-user-id";

        public TrainingRunsControllerTests()
        {
            _mockTrainingRunService = new Mock<ITrainingRunService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockUserManagementService = new Mock<IUserManagementService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockDatasetService = new Mock<IDatasetService>();
            _mockMMDetectionConfiguration = new Mock<IMMDetectionConfigurationService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockStorageConnection = new Mock<IStorageConnection>();
            _mockTrainingRunTrainParamsService = new Mock<ITrainingRunTrainParamsService>();

            var mockJobStorage = new Mock<JobStorage>();
            mockJobStorage
                .Setup(x => x.GetConnection())
                .Returns(_mockStorageConnection.Object);
            JobStorage.Current = mockJobStorage.Object;

            _controller = new TrainingRunsController(
                _mockTrainingRunService.Object,
                _mockTrainingRunTrainParamsService.Object,
                _mockWebHostEnvironment.Object,
                _mockUserManagementService.Object,
                _mockBackgroundJobClient.Object,
                _mockAppSettingsAccessor.Object,
                _mockDatasetService.Object,
                _mockMMDetectionConfiguration.Object,
                _mockConfiguration.Object,
                _mockMapper.Object
            );

            var claims = new List<Claim>
            {
                new Claim("UserId", _userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithTrainingRunViewModels()
        {
            // Arrange
            var trainingRunsDto = ResultDTO<List<TrainingRunDTO>>.Ok(new List<TrainingRunDTO> { new TrainingRunDTO()});
            var viewModelList = new List<TrainingRunIndexViewModel> { new TrainingRunIndexViewModel() };

            _mockTrainingRunService.Setup(service => service.GetAllTrainingRuns())
                .ReturnsAsync(trainingRunsDto);
            _mockMapper.Setup(mapper => mapper.Map<List<TrainingRunIndexViewModel>>(trainingRunsDto.Data))
                .Returns(viewModelList);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModelList, viewResult.Model);
        }

        [Fact]
        public async Task Index_ReturnsBadRequest_WhenErrorViewPathNotConfigured()
        {
            // Arrange
            var trainingRunsDto = ResultDTO<List<TrainingRunDTO>>.Fail("Failed");
            _mockTrainingRunService.Setup(service => service.GetAllTrainingRuns())
                .ReturnsAsync(trainingRunsDto);
            _mockConfiguration.Setup(config => config["ErrorViewsPath:Error"]).Returns((string)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsRedirectToErrorPath_WhenServiceFails()
        {
            // Arrange
            var trainingRunsDto = ResultDTO<List<TrainingRunDTO>>.Fail("Failed");
            _mockTrainingRunService.Setup(service => service.GetAllTrainingRuns())
                .ReturnsAsync(trainingRunsDto);
            _mockConfiguration.Setup(config => config["ErrorViewsPath:Error"]).Returns("/Error");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error", redirectResult.Url);
        }

        [Fact]
        public async Task Index_ReturnsNotFound_WhenDataIsNull()
        {
            // Arrange
            var trainingRunsDto = ResultDTO<List<TrainingRunDTO>>.Ok(null);
            _mockTrainingRunService.Setup(service => service.GetAllTrainingRuns())
                .ReturnsAsync(trainingRunsDto);
            _mockConfiguration.Setup(config => config["ErrorViewsPath:Error404"]).Returns("/Error404");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }

        [Fact]
        public async Task Index_ReturnsNotFound_WhenMappingFails()
        {
            // Arrange
            var trainingRunsDto = ResultDTO<List<TrainingRunDTO>>.Ok(new List<TrainingRunDTO> { new TrainingRunDTO() });

            _mockTrainingRunService.Setup(service => service.GetAllTrainingRuns())
                .ReturnsAsync(trainingRunsDto);
            _mockMapper.Setup(mapper => mapper.Map<List<TrainingRunIndexViewModel>>(trainingRunsDto.Data))
                .Returns((List<TrainingRunIndexViewModel>)null);
            _mockConfiguration.Setup(config => config["ErrorViewsPath:Error404"]).Returns("/Error404");

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Error404", redirectResult.Url);
        }

        [Fact]
        public async Task CreateTrainingRun_ReturnsViewResult()
        {
            // Act
            var result = await _controller.CreateTrainingRun();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task CreateTrainingRun_ReturnsCorrectViewName()
        {
            // Arrange
            string expectedViewName = "CreateTrainingRun";

            // Act
            var result = await _controller.CreateTrainingRun();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == expectedViewName,
                $"Expected view name to be null or '{expectedViewName}', but got '{viewResult.ViewName}'");
        }

        [Fact]
        public async Task EditTrainingRun_InvalidModelState_ReturnsFailedResultDTO()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");
            var viewModel = new TrainingRunIndexViewModel();

            // Act
            var result = await _controller.EditTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainingRun_WhenModelStateIsInvalid_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunIndexViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true
            };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.EditTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainingRun_WhenServiceUpdateSucceeds_ReturnsSuccessResult()
        {
            // Arrange
            var viewModel = new TrainingRunIndexViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true
            };

            var successResult = ResultDTO.Ok();
            _mockTrainingRunService
                .Setup(s => s.UpdateTrainingRunEntity(
                    viewModel.Id,
                    It.IsAny<Guid?>(),  // trainedModelId
                    It.IsAny<string>(), // status
                    viewModel.IsCompleted,
                    viewModel.Name))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.EditTrainingRun(viewModel);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainingRun_WhenServiceUpdateFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunIndexViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true
            };
            var errorMessage = "Update failed";
            var failResult = ResultDTO.Fail(errorMessage);

            _mockTrainingRunService
                .Setup(s => s.UpdateTrainingRunEntity(
                    viewModel.Id,
                    It.IsAny<Guid?>(),  // trainedModelId
                    It.IsAny<string>(), // status
                    viewModel.IsCompleted,
                    viewModel.Name))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.EditTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task EditTrainingRun_VerifiesServiceParameters()
        {
            // Arrange
            var viewModel = new TrainingRunIndexViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Run",
                IsCompleted = true
            };

            _mockTrainingRunService
                .Setup(s => s.UpdateTrainingRunEntity(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            await _controller.EditTrainingRun(viewModel);

            // Assert
            _mockTrainingRunService.Verify(s =>
                s.UpdateTrainingRunEntity(
                    viewModel.Id,
                    null,           // trainedModelId
                    null,           // status
                    viewModel.IsCompleted,
                    viewModel.Name
                ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task EditTrainingRun_WhenNameIsInvalid_ReturnsFailResult(string invalidName)
        {
            // Arrange
            var viewModel = new TrainingRunIndexViewModel
            {
                Id = Guid.NewGuid(),
                Name = invalidName,
                IsCompleted = true
            };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.EditTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainingRunById_WhenRunExists_ReturnsSuccessResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                Name = "Test Run"
            };

            var successResult = ResultDTO<TrainingRunDTO>.Ok(trainingRunDto);
            _mockTrainingRunService
                .Setup(s => s.GetTrainingRunById(trainingRunId,false))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.GetTrainingRunById(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(trainingRunId, result.Data.Id);
            Assert.Equal("Test Run", result.Data.Name);
        }

        [Fact]
        public async Task GetTrainingRunById_WhenRunNotFound_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var notFoundResult = ResultDTO<TrainingRunDTO>.Ok(null);

            _mockTrainingRunService
                .Setup(s => s.GetTrainingRunById(trainingRunId, false))
                .ReturnsAsync(notFoundResult);

            // Act
            var result = await _controller.GetTrainingRunById(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Training run not found", result.ErrMsg);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetTrainingRunById_WhenServiceFails_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var errorMessage = "Service error occurred";
            var failResult = ResultDTO<TrainingRunDTO>.Fail(errorMessage);

            _mockTrainingRunService
                .Setup(s => s.GetTrainingRunById(trainingRunId, false))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.GetTrainingRunById(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetTrainingRunById_VerifiesServiceParameters()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                Name = "Test Run"
            };

            _mockTrainingRunService
                .Setup(s => s.GetTrainingRunById(It.IsAny<Guid>(),false))
                .ReturnsAsync(ResultDTO<TrainingRunDTO>.Ok(trainingRunDto));

            // Act
            await _controller.GetTrainingRunById(trainingRunId);

            // Assert
            _mockTrainingRunService.Verify(s =>
                s.GetTrainingRunById(trainingRunId, false),
                Times.Once
            );
        }

        [Fact]
        public async Task GetTrainingRunById_WithEmptyGuid_ReturnsFailResult()
        {
            // Arrange
            var emptyGuid = Guid.Empty;
            var errorMessage = "Service error occurred";
            var failResult = ResultDTO<TrainingRunDTO>.Fail(errorMessage);

            _mockTrainingRunService
                .Setup(s => s.GetTrainingRunById(emptyGuid,false))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.GetTrainingRunById(emptyGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task PublishTrainingRunTrainedModel_WithEmptyGuid_ReturnsFailResult()
        {
            // Arrange
            var emptyGuid = Guid.Empty;

            // Act
            var result = await _controller.PublishTrainingRunTrainedModel(emptyGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid training run id", result.ErrMsg);
            _mockTrainingRunService.Verify(s =>
                s.PublishTrainingRunTrainedModel(It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task PublishTrainingRunTrainedModel_WhenSuccessful_ReturnsSuccessResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockTrainingRunService
                .Setup(s => s.PublishTrainingRunTrainedModel(trainingRunId))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.PublishTrainingRunTrainedModel(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
            _mockTrainingRunService.Verify(s =>
                s.PublishTrainingRunTrainedModel(trainingRunId),
                Times.Once
            );
        }

        [Fact]
        public async Task PublishTrainingRunTrainedModel_WhenServiceFails_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var errorMessage = "Failed to publish trained model";
            _mockTrainingRunService
                .Setup(s => s.PublishTrainingRunTrainedModel(trainingRunId))
                .ReturnsAsync(ResultDTO.Fail(errorMessage));

            // Act
            var result = await _controller.PublishTrainingRunTrainedModel(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
            _mockTrainingRunService.Verify(s =>
                s.PublishTrainingRunTrainedModel(trainingRunId),
                Times.Once
            );
        }

        [Fact]
        public async Task PublishTrainingRunTrainedModel_VerifiesServiceParameters()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockTrainingRunService
                .Setup(s => s.PublishTrainingRunTrainedModel(It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            await _controller.PublishTrainingRunTrainedModel(trainingRunId);

            // Assert
            _mockTrainingRunService.Verify(s =>
                s.PublishTrainingRunTrainedModel(trainingRunId),
                Times.Once,
                "Service method should be called exactly once with the correct trainingRunId"
            );
        }

        [Fact]
        public async Task ScheduleTrainingRun_WhenModelStateIsInvalid_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Run",
                DatasetId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.ScheduleTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required", result.ErrMsg);
        }

        [Fact]
        public async Task ScheduleTrainingRun_WhenUserIdIsNull_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Run",
                DatasetId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };

            // Clear claims to simulate null userId
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.ScheduleTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User id is null", result.ErrMsg);
        }

        [Fact]
        public async Task ScheduleTrainingRun_WhenUserNotFound_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Run",
                DatasetId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };

            _mockUserManagementService
                .Setup(s => s.GetUserById(_userId))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok((UserDTO)null));

            // Act
            var result = await _controller.ScheduleTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.ErrMsg);
        }

        [Fact]
        public async Task ScheduleTrainingRun_WhenServiceFails_ReturnsFailResult()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Run",
                DatasetId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };

            var user = new UserDTO { Id = _userId, UserName = "Test User" };
            _mockUserManagementService
                .Setup(s => s.GetUserById(_userId))
                .ReturnsAsync(ResultDTO<UserDTO>.Fail("Services failed"));

            var errorMessage = "Services failed";
            _mockTrainingRunService
                .Setup(s => s.CreateTrainingRunWithBaseModel(It.IsAny<TrainingRunDTO>()))
                .ReturnsAsync(ResultDTO<TrainingRunDTO>.Fail(errorMessage));

            // Act
            var result = await _controller.ScheduleTrainingRun(viewModel);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task ScheduleTrainingRun_VerifiesTrainingRunCreation()
        {
            // Arrange
            var viewModel = new TrainingRunViewModel
            {
                Name = "Test Run",
                DatasetId = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };

            var user = new UserDTO { Id = _userId, UserName = "Test User" };
            _mockUserManagementService
                .Setup(s => s.GetUserById(_userId))
                .ReturnsAsync(ResultDTO<UserDTO>.Ok(user));

            TrainingRunDTO capturedDto = null;
            _mockTrainingRunService
                .Setup(s => s.CreateTrainingRunWithBaseModel(It.IsAny<TrainingRunDTO>()))
                .Callback<TrainingRunDTO>(dto => capturedDto = dto)
                .ReturnsAsync((TrainingRunDTO dto) => ResultDTO<TrainingRunDTO>.Ok(dto));

            var trainParamsDto = new TrainingRunTrainParamsDTO();
            _mockTrainingRunTrainParamsService
                .Setup(s => s.CreateTrainingRunTrainParams(viewModel.NumEpochs, viewModel.BatchSize, viewModel.NumFrozenStages, It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(ResultDTO<TrainingRunTrainParamsDTO>.Ok(trainParamsDto));

            // Act
            await _controller.ScheduleTrainingRun(viewModel);

            // Assert
            Assert.NotNull(capturedDto);
            Assert.Equal(viewModel.Name, capturedDto.Name);
            Assert.Equal(user.Id, capturedDto.CreatedById);
            Assert.Equal(viewModel.DatasetId, capturedDto.DatasetId);
            Assert.Equal(viewModel.TrainedModelId, capturedDto.TrainedModelId);
            Assert.Equal(nameof(ScheduleRunsStatus.Waiting), capturedDto.Status);
        }

        [Fact]
        public async Task CreateErrMsgFile_ShouldReturnFail_WhenAppSettingFails()
        {
            // Arrange
            Guid detectionRunId = Guid.NewGuid();
            string errMsg = "Error message";
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Error fetching setting"));

            // Use reflection to call private method
            var privateMethod = typeof(TrainingRunsController)
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
                .Setup(x => x.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(string.Empty));

            // Use reflection to call private method
            var privateMethod = typeof(TrainingRunsController)
                .GetMethod("CreateErrMsgFile", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var resultTask = (Task<ResultDTO>)privateMethod.Invoke(_controller, new object[] { detectionRunId, errMsg });
            var result = await resultTask;

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Directory path not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainingRunErrorLogMessage_EmptyGuid_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.Empty;

            // Act
            var result = await _controller.GetTrainingRunErrorLogMessage(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Invalid training run id", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainingRunErrorLogMessage_AppSettingsFailure_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Settings error"));

            // Act
            var result = await _controller.GetTrainingRunErrorLogMessage(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainingRunErrorLogMessage_EmptyFolderPath_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<string>("TrainingRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(string.Empty));

            // Act
            var result = await _controller.GetTrainingRunErrorLogMessage(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Directory path not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainingRun_EmptyGuid_ReturnsFailResult()
        {
            // Arrange
            var trainingRunId = Guid.Empty;

            // Act
            var result = await _controller.DeleteTrainingRun(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid training run id", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainingRun_NoMatchingJobs_ReturnsSuccess()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var enqueuedJobs = new JobList<EnqueuedJobDto>(new List<KeyValuePair<string, EnqueuedJobDto>>());
            Mock<IMonitoringApi> mockMonitoringApi = new Mock<IMonitoringApi>();
            mockMonitoringApi.Setup(api => api.EnqueuedJobs("default", 0, int.MaxValue)).Returns(enqueuedJobs);

            JobStorage.Current = Mock.Of<JobStorage>(storage => storage.GetMonitoringApi() == mockMonitoringApi.Object);

            _mockTrainingRunService
                .Setup(service => service.DeleteTrainingRun(trainingRunId, It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteTrainingRun(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteTrainingRun_DeleteTrainingRunFails_ReturnsFailureResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();

            var mockMonitoringApi = new Mock<IMonitoringApi>();
            mockMonitoringApi
                .Setup(api => api.EnqueuedJobs("default", 0, int.MaxValue))
                .Returns(new JobList<EnqueuedJobDto>(new List<KeyValuePair<string, EnqueuedJobDto>>()));

            var mockJobStorage = new Mock<JobStorage>();
            mockJobStorage.Setup(js => js.GetMonitoringApi()).Returns(mockMonitoringApi.Object);
            JobStorage.Current = mockJobStorage.Object;

            _mockTrainingRunService
                .Setup(service => service.DeleteTrainingRun(trainingRunId, It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _controller.DeleteTrainingRun(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainingRun_DeleteTrainingRunSucceeds_ReturnsSuccessResult()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();

            var mockMonitoringApi = new Mock<IMonitoringApi>();
            mockMonitoringApi
                .Setup(api => api.EnqueuedJobs("default", 0, int.MaxValue))
                .Returns(new JobList<EnqueuedJobDto>(new List<KeyValuePair<string, EnqueuedJobDto>>()));

            var mockJobStorage = new Mock<JobStorage>();
            mockJobStorage.Setup(js => js.GetMonitoringApi()).Returns(mockMonitoringApi.Object);
            JobStorage.Current = mockJobStorage.Object;

            _mockTrainingRunService
                .Setup(service => service.DeleteTrainingRun(trainingRunId, It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.DeleteTrainingRun(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ExecuteTrainingRunProcess_ExceptionThrown_ReturnsExceptionFailure()
        {
            // Arrange
            var trainingRunDTO = new TrainingRunDTO { Id = Guid.NewGuid() };
            var paramsDTO = new TrainingRunTrainParamsDTO { Id = Guid.NewGuid() };

            _mockTrainingRunService
                .Setup(service => service.UpdateTrainingRunEntity(trainingRunDTO.Id.Value, null, nameof(ScheduleRunsStatus.Processing), false,null))
                .Throws(new Exception("Unexpected error"));

            // Act
            var result = await _controller.ExecuteTrainingRunProcess(trainingRunDTO, paramsDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Unexpected error", result.ErrMsg);
        }

        [Fact]
        public async Task ExecuteTrainingRunProcess_ShouldReturnFail_WhenUpdateTrainRunFails()
        {
            // Arrange
            var paramsDTO = new TrainingRunTrainParamsDTO { Id = Guid.NewGuid() };
            var trainingRunDTO = new TrainingRunDTO { Id = Guid.NewGuid(), DatasetId = Guid.NewGuid() };
            _mockTrainingRunService.Setup(s => s.UpdateTrainingRunEntity(It.IsAny<Guid>(), null, nameof(ScheduleRunsStatus.Processing), null, null))
                .ReturnsAsync(ResultDTO.Fail("Object reference not set to an instance of an object."));

            // Act
            var result = await _controller.ExecuteTrainingRunProcess(trainingRunDTO, paramsDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Object reference not set to an instance of an object.", result.ErrMsg);
        }

        [Fact]
        public async Task ExecuteTrainingRunProcess_ShouldReturnFail_WhenGetDatasetFails()
        {
            // Arrange
            var paramsDTO = new TrainingRunTrainParamsDTO { Id = Guid.NewGuid() };
            var trainingRunDTO = new TrainingRunDTO { Id = Guid.NewGuid(), DatasetId = Guid.NewGuid() };
            _mockTrainingRunService.Setup(s => s.UpdateTrainingRunEntity(It.IsAny<Guid>(), null, nameof(ScheduleRunsStatus.Processing), null, null))
                .ReturnsAsync(ResultDTO.Ok());
            _mockDatasetService.Setup(s => s.GetDatasetDTOFullyIncluded(It.IsAny<Guid>(), false))
                .ReturnsAsync(ResultDTO<DatasetDTO>.Fail("Object reference not set to an instance of an object."));

            // Act
            var result = await _controller.ExecuteTrainingRunProcess(trainingRunDTO, paramsDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Object reference not set to an instance of an object.", result.ErrMsg);
        }

        [Fact]
        public async Task ExecuteTrainingRunProcess_ShouldReturnExceptionFail_WhenExceptionOccurs()
        {
            // Arrange
            var paramsDTO = new TrainingRunTrainParamsDTO { Id = Guid.NewGuid() };
            var trainingRunDTO = new TrainingRunDTO { Id = Guid.NewGuid(), DatasetId = Guid.NewGuid() };

            // Simulate an exception being thrown in the process.
            _mockTrainingRunService.Setup(s => s.UpdateTrainingRunEntity(It.IsAny<Guid>(), null, nameof(ScheduleRunsStatus.Processing), null, null))
                .ThrowsAsync(new NullReferenceException("Object reference not set to an instance of an object"));

            // Act
            var result = await _controller.ExecuteTrainingRunProcess(trainingRunDTO, paramsDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Object reference not set to an instance of an object", result.ErrMsg);
        }

        [Fact]
        public async Task ExecuteTrainingRunProcess_ShouldReturnSuccess_WhenAllStepsAreSuccessful()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();

            var trainingRunDTO = new TrainingRunDTO
            {
                Id = trainingRunId,
                DatasetId = datasetId,
                BaseModel = new TrainedModelDTO()
            };

            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = "TestDataset"
            };
            var paramsDTO = new TrainingRunTrainParamsDTO
            {
                Id = Guid.NewGuid(),
                NumEpochs = 5,
                NumFrozenStages = 2,
                BatchSize = 32
            };

            _mockTrainingRunService
                .Setup(s => s.UpdateTrainingRunEntity(
                    trainingRunId,
                    null,
                    nameof(ScheduleRunsStatus.Processing),
                    false,
                    It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetService
                .Setup(s => s.GetDatasetDTOFullyIncluded(datasetId, false))
                .ReturnsAsync(ResultDTO<DatasetDTO>.Ok(datasetDTO));

            _mockDatasetService
                .Setup(s => s.ExportDatasetAsCOCOFormat(
                    datasetId,
                    "AllImages",
                    It.IsAny<string>(),
                    true))
                .ReturnsAsync(ResultDTO<string>.Ok("path/to/dataset"));

            _mockTrainingRunService
                .Setup(s => s.GenerateTrainingRunConfigFile(
                    trainingRunId,
                    It.IsAny<DatasetDTO>(),
                    It.IsAny<TrainedModelDTO>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(ResultDTO<string>.Ok("config/path"));

            _mockTrainingRunService
                .Setup(s => s.StartTrainingRun(trainingRunId))
                .ReturnsAsync(ResultDTO.Ok());

            _mockTrainingRunService
                .Setup(s => s.CreateTrainedModelByTrainingRunId(trainingRunId))
                .ReturnsAsync(ResultDTO<Guid>.Ok(trainedModelId));

            _mockTrainingRunService
                .Setup(s => s.UpdateTrainingRunEntity(
                    trainingRunId,
                    trainedModelId,
                    nameof(ScheduleRunsStatus.Success),
                    true,
                    It.IsAny<string>()))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _controller.ExecuteTrainingRunProcess(trainingRunDTO, paramsDTO);

            // Assert
            Assert.True(result.IsSuccess);

            _mockTrainingRunService.Verify(s => s.UpdateTrainingRunEntity(
                trainingRunId,
                null,
                nameof(ScheduleRunsStatus.Processing),
                false,
                It.IsAny<string>()), Times.Once);

            _mockDatasetService.Verify(s => s.GetDatasetDTOFullyIncluded(datasetId, false), Times.Once);

            _mockDatasetService.Verify(s => s.ExportDatasetAsCOCOFormat(
                datasetId,
                "AllImages",
                It.IsAny<string>(),
                true), Times.Once);

            _mockTrainingRunService.Verify(s => s.StartTrainingRun(trainingRunId), Times.Once);

            _mockTrainingRunService.Verify(s => s.CreateTrainedModelByTrainingRunId(trainingRunId), Times.Once);

            _mockTrainingRunService.Verify(s => s.UpdateTrainingRunEntity(
                trainingRunId,
                trainedModelId,
                nameof(ScheduleRunsStatus.Success),
                true,
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetTrainingRunStatistics_ReturnsOkResult_WithData_WhenServiceReturnsDataSuccessfully()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var expectedData = new TrainingRunResultsDTO();
            var successResult = ResultDTO<TrainingRunResultsDTO>.Ok(expectedData);

            _mockTrainingRunService
                .Setup(service => service.GetBestEpochForTrainingRun(trainingRunId))
                .Returns(successResult);

            // Act
            var result = await _controller.GetTrainingRunStatistics(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedData, result.Data);
        }

        [Fact]
        public async Task GetTrainingRunStatistics_ReturnsFailResult_WhenServiceReturnsFailure()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var errorMessage = "Error retrieving data";
            var failureResult = ResultDTO<TrainingRunResultsDTO>.Fail(errorMessage);

            _mockTrainingRunService
                .Setup(service => service.GetBestEpochForTrainingRun(trainingRunId))
                .Returns(failureResult);

            // Act
            var result = await _controller.GetTrainingRunStatistics(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetTrainingRunStatistics_ReturnsFailResult_WhenServiceReturnsNullData()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var successWithNullDataResult = ResultDTO<TrainingRunResultsDTO>.Ok(null);

            _mockTrainingRunService
                .Setup(service => service.GetBestEpochForTrainingRun(trainingRunId))
                .Returns(successWithNullDataResult);

            // Act
            var result = await _controller.GetTrainingRunStatistics(trainingRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get training run statistics", result.ErrMsg);
        }

    }
}
