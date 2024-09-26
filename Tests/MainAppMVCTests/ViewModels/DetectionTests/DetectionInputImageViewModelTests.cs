using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;

namespace Tests.MainAppMVCTests.ViewModels.DetectionTests
{
    public class DetectionInputImageViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new DetectionInputImageViewModel
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
            var name = "Test Image";
            var viewModel = new DetectionInputImageViewModel
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
            var description = "Test image description.";
            var viewModel = new DetectionInputImageViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void DateTakenProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var dateTaken = DateTime.UtcNow;
            var viewModel = new DetectionInputImageViewModel
            {
                DateTaken = dateTaken
            };

            // Act & Assert
            Assert.Equal(dateTaken, viewModel.DateTaken);
        }

        [Fact]
        public void ImagePathProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var imagePath = "/images/test.png";
            var viewModel = new DetectionInputImageViewModel
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
            var imageFileName = "test.png";
            var viewModel = new DetectionInputImageViewModel
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
            var createdById = "user123";
            var viewModel = new DetectionInputImageViewModel
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
            var viewModel = new DetectionInputImageViewModel
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
            var viewModel = new DetectionInputImageViewModel
            {
                CreatedBy = createdBy
            };

            // Act & Assert
            Assert.Equal(createdBy, viewModel.CreatedBy);
        }

        [Fact]
        public void UpdatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedById = "user456";
            var viewModel = new DetectionInputImageViewModel
            {
                UpdatedById = updatedById
            };

            // Act & Assert
            Assert.Equal(updatedById, viewModel.UpdatedById);
        }

        [Fact]
        public void UpdatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedOn = DateTime.UtcNow;
            var viewModel = new DetectionInputImageViewModel
            {
                UpdatedOn = updatedOn
            };

            // Act & Assert
            Assert.Equal(updatedOn, viewModel.UpdatedOn);
        }

        [Fact]
        public void UpdatedByProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedBy = new UserDTO();
            var viewModel = new DetectionInputImageViewModel
            {
                UpdatedBy = updatedBy
            };

            // Act & Assert
            Assert.Equal(updatedBy, viewModel.UpdatedBy);
        }
    }

}
