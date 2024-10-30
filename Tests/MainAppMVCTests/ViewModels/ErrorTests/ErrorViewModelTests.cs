using MainApp.MVC.ViewModels.IntranetPortal.Errors;

namespace Tests.MainAppMVCTests.ViewModels.ErrorTests
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void RequestIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var requestId = "1234";
            var viewModel = new ErrorViewModel { RequestId = requestId };

            // Act & Assert
            Assert.Equal(requestId, viewModel.RequestId);
        }

        [Fact]
        public void ShowRequestId_ShouldReturnTrue_WhenRequestIdIsNotEmpty()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = "1234" };

            // Act & Assert
            Assert.True(viewModel.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_ShouldReturnFalse_WhenRequestIdIsNullOrEmpty()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = null };

            // Act & Assert
            Assert.False(viewModel.ShowRequestId);
        }

    }
}
