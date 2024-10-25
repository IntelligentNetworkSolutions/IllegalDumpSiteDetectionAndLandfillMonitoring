using DAL.Interfaces.Helpers;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class SpecialActionsControllerTests
    {
        private readonly Mock<ModulesAndAuthClaimsHelper> _mockModulesAndAuthClaims;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly SpecialActionsController _controller;

        public SpecialActionsControllerTests()
        {
            _mockModulesAndAuthClaims = new Mock<ModulesAndAuthClaimsHelper>(MockBehavior.Strict, new object[] { Mock.Of<IConfiguration>() });
            _mockConfiguration = new Mock<IConfiguration>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _controller = new SpecialActionsController(_mockModulesAndAuthClaims.Object, _mockConfiguration.Object, _mockAppSettingsAccessor.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

    }
}
