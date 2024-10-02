using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class MapControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult_WithNullGuid()
        {
            // Arrange
            var controller = new MapController();

            // Act
            var result = controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult?.ViewData["detectionRunId"]);
        }

        [Fact]
        public void Index_ReturnsViewResult_WhenCalled()
        {
            // Arrange
            var controller = new MapController();

            // Act
            var result = controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("")]
        [InlineData(null)]
        public void Index_HandlesInvalidGuidFormat(string invalidGuid)
        {
            // Arrange
            var controller = new MapController();
            Guid? guid = null;
            if (Guid.TryParse(invalidGuid, out var parsedGuid))
            {
                guid = parsedGuid;
            }

            // Act
            var result = controller.Index(guid) as ViewResult;

            // Assert
            Assert.Null(result.ViewData["detectionRunId"]);
        }

    }
}
