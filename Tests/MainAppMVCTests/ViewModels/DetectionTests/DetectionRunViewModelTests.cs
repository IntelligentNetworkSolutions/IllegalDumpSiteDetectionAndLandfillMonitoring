using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.DetectionTests
{
    public class DetectionRunViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new DetectionRunViewModel
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
            var name = "Test Detection Run";
            var viewModel = new DetectionRunViewModel
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
            var viewModel = new DetectionRunViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void IsCompletedProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var isCompleted = true;
            var viewModel = new DetectionRunViewModel
            {
                IsCompleted = isCompleted
            };

            // Act & Assert
            Assert.Equal(isCompleted, viewModel.IsCompleted);
        }

        [Fact]
        public void ImagePathProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var imagePath = "path/to/image.png";
            var viewModel = new DetectionRunViewModel
            {
                ImagePath = imagePath
            };

            // Act & Assert
            Assert.Equal(imagePath, viewModel.ImagePath);
        }

        [Fact]
        public void ImageFileNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var imageFileName = "image.png";
            var viewModel = new DetectionRunViewModel
            {
                ImageFileName = imageFileName
            };

            // Act & Assert
            Assert.Equal(imageFileName, viewModel.ImageFileName);
        }

        [Fact]
        public void CreatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdById = "user-id";
            var viewModel = new DetectionRunViewModel
            {
                CreatedById = createdById
            };

            // Act & Assert
            Assert.Equal(createdById, viewModel.CreatedById);
        }

        [Fact]
        public void CreatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var viewModel = new DetectionRunViewModel
            {
                CreatedOn = createdOn
            };

            // Act & Assert
            Assert.Equal(createdOn, viewModel.CreatedOn);
        }

        [Fact]
        public void CreatedByProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdBy = new UserDTO();
            var viewModel = new DetectionRunViewModel
            {
                CreatedBy = createdBy
            };

            // Act & Assert
            Assert.Equal(createdBy, viewModel.CreatedBy);
        }

        [Fact]
        public void DetectedDumpSitesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new DetectionRunViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.DetectedDumpSites);
            Assert.Empty(viewModel.DetectedDumpSites);
        }

        [Fact]
        public void DetectedDumpSitesProperty_ShouldAllowAddingItems()
        {
            // Arrange
            var viewModel = new DetectionRunViewModel();
            var dumpSite = new DetectedDumpSiteViewModel();

            // Act
            viewModel.DetectedDumpSites.Add(dumpSite);

            // Assert
            Assert.Single(viewModel.DetectedDumpSites);
            Assert.Contains(dumpSite, viewModel.DetectedDumpSites);
        }
    }
}
