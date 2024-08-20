using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.LegalLandfillManagementTests
{
    public class LegalLandfillViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new LegalLandfillViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void NameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var name = "Test Legal Landfill";
            var viewModel = new LegalLandfillViewModel
            {
                Name = name
            };

            // Act & Assert
            Assert.Equal(name, viewModel.Name);
        }

        [Fact]
        public void DescriptionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var description = "This is a test description.";
            var viewModel = new LegalLandfillViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void LegalLandfillPointCloudFilesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new LegalLandfillViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.LegalLandfillPointCloudFiles);
            Assert.Empty(viewModel.LegalLandfillPointCloudFiles);
        }

        [Fact]
        public void LegalLandfillPointCloudFilesProperty_ShouldAllowAddingItems()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel();
            var pointCloudFile = new LegalLandfillPointCloudFileViewModel();

            // Act
            viewModel.LegalLandfillPointCloudFiles.Add(pointCloudFile);

            // Assert
            Assert.Single(viewModel.LegalLandfillPointCloudFiles);
            Assert.Contains(pointCloudFile, viewModel.LegalLandfillPointCloudFiles);
        }
    }
}
