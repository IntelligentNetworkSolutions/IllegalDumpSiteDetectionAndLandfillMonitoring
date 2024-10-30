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
    public class LastExtentToolViewComponentTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly LastExtentToolViewComponent _viewComponent;

        public LastExtentToolViewComponentTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigurationSection = new Mock<IConfigurationSection>();

            _mockConfigurationSection.SetupGet(x => x.Value).Returns("MapToolLastExtent");
            _mockConfiguration.Setup(c => c.GetSection("AppSettings:Modules")).Returns(_mockConfigurationSection.Object);

            _modulesAndAuthClaimsHelper = new ModulesAndAuthClaimsHelper(_mockConfiguration.Object);
            _viewComponent = new LastExtentToolViewComponent(_modulesAndAuthClaimsHelper);
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
