using MainApp.MVC.Helpers;
using MainApp.MVC.ViewComponents.Map;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SD.Helpers;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Tests.MainAppMVCTests.ViewComponents.Map
{
    public class DetectionIgnoreZonesToolViewComponentTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly DetectionIgnoreZonesToolViewComponent _viewComponent;

        public DetectionIgnoreZonesToolViewComponentTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigurationSection = new Mock<IConfigurationSection>();

            _mockConfigurationSection.SetupGet(x => x.Value).Returns("MapToolDetectionIgnoreZones");
            _mockConfiguration.Setup(c => c.GetSection("AppSettings:Modules")).Returns(_mockConfigurationSection.Object);

            _modulesAndAuthClaimsHelper = new ModulesAndAuthClaimsHelper(_mockConfiguration.Object);
            _viewComponent = new DetectionIgnoreZonesToolViewComponent(_modulesAndAuthClaimsHelper);
        }

        [Fact]
        public async Task InvokeAsync_UserHasModuleAndAuthClaim_ReturnsView()
        {
            // Arrange
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ContentViewComponentResult>(result);
            Assert.NotNull(viewResult);
        }



    }


}
