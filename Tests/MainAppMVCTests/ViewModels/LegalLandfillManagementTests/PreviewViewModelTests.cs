using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;

namespace Tests.MainAppMVCTests.ViewModels.LegalLandfillManagementTests
{
    public class PreviewViewModelTests
    {
        [Fact]
        public void FileIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var viewModel = new PreviewViewModel { FileId = fileId };

            // Act & Assert
            Assert.Equal(fileId, viewModel.FileId);
        }

        [Fact]
        public void LandfillIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var viewModel = new PreviewViewModel { LandfillId = landfillId };

            // Act & Assert
            Assert.Equal(landfillId, viewModel.LandfillId);
        }

        [Fact]
        public void FileNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var fileName = "example.txt";
            var viewModel = new PreviewViewModel { FileName = fileName };

            // Act & Assert
            Assert.Equal(fileName, viewModel.FileName);
        }

    }
}
