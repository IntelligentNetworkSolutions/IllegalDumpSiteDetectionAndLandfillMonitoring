using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DetectionDTOs;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectionRunDTOTests
    {
        [Fact]
        public void DetectionRunDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Run";
            var description = "Test Run Description";
            var isCompleted = true;
            //var imagePath = "/images/test.png";
            //var imageFileName = "test.png";
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();
            var detectedDumpSites = new List<DetectedDumpSiteDTO>();

            // Act
            var dto = new DetectionRunDTO
            {
                Id = id,
                Name = name,
                Description = description,
                IsCompleted = isCompleted,
                //ImagePath = imagePath,
                //ImageFileName = imageFileName,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                DetectedDumpSites = detectedDumpSites
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(isCompleted, dto.IsCompleted);
            //Assert.Equal(imagePath, dto.ImagePath);
            //Assert.Equal(imageFileName, dto.ImageFileName);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(detectedDumpSites, dto.DetectedDumpSites);
        }

        [Fact]
        public void DetectionRunDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DetectionRunDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.False(dto.IsCompleted);
            //Assert.Null(dto.ImagePath);
            //Assert.Null(dto.ImageFileName);
            Assert.Null(dto.CreatedById);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
            Assert.NotNull(dto.DetectedDumpSites);
            Assert.Empty(dto.DetectedDumpSites);
        }

        [Fact]
        public void DetectionRunDTO_ShouldBeClassType()
        {
            // Arrange & Act
            var dto = new DetectionRunDTO();

            // Assert
            Assert.IsType<DetectionRunDTO>(dto);
        }
    }
}
