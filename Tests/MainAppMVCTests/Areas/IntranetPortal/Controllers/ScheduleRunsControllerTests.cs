using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.ScheduleRuns;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SD.Enums;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

    }
}
