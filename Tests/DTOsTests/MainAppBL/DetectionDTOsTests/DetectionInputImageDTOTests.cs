using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectionInputImageDTOTests
    {
        [Fact]
        public void DetectionInputImageDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Image";
            var description = "Test Image Description";
            var dateTaken = DateTime.UtcNow;
            var imagePath = "/images/test-image.png";
            var imageFileName = "test-image.png";
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();
            var updatedById = "user456";
            var updatedOn = DateTime.UtcNow;
            var updatedBy = new UserDTO();

            // Act
            var dto = new DetectionInputImageDTO
            {
                Id = id,
                Name = name,
                Description = description,
                DateTaken = dateTaken,
                ImagePath = imagePath,
                ImageFileName = imageFileName,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(dateTaken, dto.DateTaken);
            Assert.Equal(imagePath, dto.ImagePath);
            Assert.Equal(imageFileName, dto.ImageFileName);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
        }

        [Fact]
        public void DetectionInputImageDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DetectionInputImageDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.Null(dto.DateTaken);
            Assert.Null(dto.ImagePath);
            Assert.Null(dto.ImageFileName);
            Assert.Null(dto.CreatedById);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.UpdatedBy);
        }

        [Fact]
        public void DetectionInputImageDTO_ShouldBeClassType()
        {
            // Arrange & Act
            var dto = new DetectionInputImageDTO();

            // Assert
            Assert.IsType<DetectionInputImageDTO>(dto);
        }

    }
}
