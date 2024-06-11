using MainApp.MVC.Areas.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.Common
{
    public class ErrorControllerTests
    {
        [Theory]
        [InlineData(401, "Error_401")]
        [InlineData(403, "Error_403")]
        [InlineData(404, "Error_404")]
        [InlineData(500, "Error_500")]
        public void Error_ReturnsCorrectView_ForStatusCode(int statusCode, string expectedView)
        {
            // Arrange
            var controller = new ErrorController();

            // Act
            var result = controller.Error(statusCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedView, viewResult.ViewName);
        }

        [Fact]
        public void Error_ReturnsDefaultErrorView_WhenStatusCodeIsUnknown()
        {
            // Arrange
            var controller = new ErrorController();
            int unknownStatusCode = 999;

            // Act
            var result = controller.Error(unknownStatusCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Error_ReturnsDefaultErrorView_WhenStatusCodeIsZero()
        {
            // Arrange
            var controller = new ErrorController();
            int zeroStatusCode = 0;

            // Act
            var result = controller.Error(zeroStatusCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Error_ReturnsDefaultErrorView_WhenStatusCodeIsNegative()
        {
            // Arrange
            var controller = new ErrorController();
            int negativeStatusCode = -1;

            // Act
            var result = controller.Error(negativeStatusCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Error_HandleExceptionGracefully()
        {
            // Arrange
            var controller = new ErrorController();

            var result = controller.Error(-999);

            // Act & Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
