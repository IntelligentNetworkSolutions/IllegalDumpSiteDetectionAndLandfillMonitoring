using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.ScheduleRuns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class ScheduleRunsControllerTests
    {
        private readonly Mock<IDetectionRunService> _mockDetectionRunService;
        private readonly Mock<ITrainingRunService> _mockTrainingRunService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ScheduleRunsController _controller;

        public ScheduleRunsControllerTests()
        {
            _mockDetectionRunService = new Mock<IDetectionRunService>();
            _mockTrainingRunService = new Mock<ITrainingRunService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new ScheduleRunsController(
                _mockDetectionRunService.Object,
                _mockTrainingRunService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public async Task ViewScheduledRuns_ReturnsViewResult_WhenRunsAreSuccessful()
        {
            // Arrange
            var detectionRuns = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Status = "Success" }
            };
            var trainingRuns = new List<TrainingRunDTO>
            {
                new TrainingRunDTO { Id = Guid.NewGuid(), Status = "Success" }
            };

            _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));

            _mockTrainingRunService.Setup(s => s.GetAllTrainingRuns())
                .ReturnsAsync(ResultDTO<List<TrainingRunDTO>>.Ok(trainingRuns));

            // Act
            var result = await _controller.ViewScheduledRuns();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ViewScheduledRunsViewModel>(viewResult.Model);
            Assert.Equal(detectionRuns, model.DetectionRuns);
            Assert.Equal(trainingRuns, model.TrainingRuns);
        }

        [Fact]
        public async Task ViewScheduledRuns_RedirectsToBadRequest_WhenDetectionRunServiceFails()
        {
            // Arrange
            _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Fail("Error"));

            // Act
            var result = await _controller.ViewScheduledRuns();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ViewScheduledRuns_RedirectsToError404_WhenNoDetectionRunsFound()
        {
            // Arrange
            _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(null));

            // Act
            var result = await _controller.ViewScheduledRuns();

            // Assert
            var redirectResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ViewScheduledRuns_RedirectsToBadRequest_WhenTrainingRunServiceFails()
        {
            // Arrange
            var detectionRuns = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Status = "Success" }
            };
            _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));

            _mockTrainingRunService.Setup(s => s.GetAllTrainingRuns())
                .ReturnsAsync(ResultDTO<List<TrainingRunDTO>>.Fail("Error"));

            // Act
            var result = await _controller.ViewScheduledRuns();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ViewScheduledRuns_RedirectsToError404_WhenNoTrainingRunsFound()
        {
            // Arrange
            var detectionRuns = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = Guid.NewGuid(), Status = "Success" }
            };
            _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));

            _mockTrainingRunService.Setup(s => s.GetAllTrainingRuns())
                .ReturnsAsync(ResultDTO<List<TrainingRunDTO>>.Ok(null));

            // Act
            var result = await _controller.ViewScheduledRuns();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        //[Fact]
        //public async Task ViewScheduledRuns_HandlesProcessingDetectionRuns_Successfully()
        //{
        //    // Arrange
        //    var detectionRuns = new List<DetectionRunDTO>
        //    {
        //        new DetectionRunDTO { Id = Guid.NewGuid(), Status = nameof(ScheduleRunsStatus.Processing) }
        //    };
        //    var trainingRuns = new List<TrainingRunDTO>
        //    {
        //        new TrainingRunDTO { Id = Guid.NewGuid(), Status = "Success" }
        //    };

        //    _mockDetectionRunService.Setup(s => s.GetAllDetectionRuns())
        //        .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(detectionRuns));
        //    _mockTrainingRunService.Setup(s => s.GetAllTrainingRuns())
        //        .ReturnsAsync(ResultDTO<List<TrainingRunDTO>>.Ok(trainingRuns));

        //    // Mock Hangfire job storage
        //    var mockMonitoringApi = new Mock<IMonitoringApi>();
        //    mockMonitoringApi.Setup(m => m.ProcessingJobs(0, int.MaxValue))
        //        .Returns(new JobList<ProcessingJobDto>(new Dictionary<string, ProcessingJobDto>()));
        //    mockMonitoringApi.Setup(m => m.SucceededJobs(0, int.MaxValue))
        //        .Returns(new JobList<SucceededJobDto>(new Dictionary<string, SucceededJobDto>()));
        //    mockMonitoringApi.Setup(m => m.FailedJobs(0, int.MaxValue))
        //        .Returns(new JobList<FailedJobDto>(new Dictionary<string, FailedJobDto>()));

        //    var storage = new Mock<JobStorage>();
        //    storage.Setup(x => x.GetMonitoringApi())
        //        .Returns(mockMonitoringApi.Object);
        //    JobStorage.Current = storage.Object;

        //    // Act
        //    var result = await _controller.ViewScheduledRuns();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<ViewScheduledRunsViewModel>(viewResult.Model);
        //    Assert.NotNull(model.DetectionRuns);
        //    Assert.NotNull(model.TrainingRuns);

        //    // Cleanup
        //    JobStorage.Current = null;
        //}


    }
}
