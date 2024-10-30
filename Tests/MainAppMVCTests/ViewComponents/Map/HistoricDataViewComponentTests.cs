using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewComponents.Map;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewComponents.Map
{
    public class HistoricDataViewComponentTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly Mock<IDetectionRunService> _mockDetectionRunService;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly HistoricDataViewComponent _viewComponent;

        public HistoricDataViewComponentTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigurationSection = new Mock<IConfigurationSection>();
            _mockDetectionRunService = new Mock<IDetectionRunService>();

            _mockConfigurationSection.SetupGet(x => x.Value).Returns("HistoricData");
            _mockConfiguration.Setup(c => c.GetSection("AppSettings:Modules")).Returns(_mockConfigurationSection.Object);

            _modulesAndAuthClaimsHelper = new ModulesAndAuthClaimsHelper(_mockConfiguration.Object);
            _viewComponent = new HistoricDataViewComponent(_modulesAndAuthClaimsHelper, _mockDetectionRunService.Object);
        }

        [Fact]
        public async Task InvokeAsync_UserHasModuleAndAuthClaim_ReturnsView()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = await _viewComponent.InvokeAsync(detectionRunId);

            // Assert
            var viewResult = Assert.IsType<ContentViewComponentResult>(result);
            Assert.NotNull(viewResult);
        }
    }
}
