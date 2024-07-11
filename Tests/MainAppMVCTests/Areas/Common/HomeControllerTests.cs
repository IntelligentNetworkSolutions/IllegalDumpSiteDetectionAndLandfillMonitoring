using MainApp.MVC.Areas.Common.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.Common
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsRedirectToIntranetPortalMapIndex_WhenAuthenticatedAndIntranetMode()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HomeController>>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c["ApplicationStartupMode"]).Returns(SD.ApplicationStartModes.IntranetPortal);

            var controller = new HomeController(loggerMock.Object, configurationMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };

            // Act
            var result = controller.Index() as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("~/IntranetPortal/LandingPage", result.Url);
        }

        [Fact]
        public void Index_ReturnsRedirectToIntranetPortalLandingPage_WhenNotAuthenticatedAndIntranetMode()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HomeController>>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c["ApplicationStartupMode"]).Returns(SD.ApplicationStartModes.IntranetPortal);

            var controller = new HomeController(loggerMock.Object, configurationMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = controller.Index() as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("~/IntranetPortal/LandingPage", result.Url);
        }

        [Fact]
        public void Index_ReturnsRedirectToPublicPortalMapIndex_WhenNotInIntranetMode()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HomeController>>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c["ApplicationStartupMode"]).Returns(SD.ApplicationStartModes.PublicPortal);

            var controller = new HomeController(loggerMock.Object, configurationMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = controller.Index() as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("~/PublicPortal/Map/Index", result.Url);
        }

        [Fact]
        public async Task ResetTranslationCache_ReturnsOkResult()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HomeController>>();
            var configurationMock = new Mock<IConfiguration>();

            var controller = new HomeController(loggerMock.Object, configurationMock.Object);

            // Act
            var result = await controller.ResetTranslationCache();

            // Assert
            Assert.IsType<OkResult>(result);
        }

       
    }
}
