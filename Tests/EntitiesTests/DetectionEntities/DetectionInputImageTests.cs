using Entities;
using Entities.DetectionEntities;

namespace Tests.EntitiesTests.DetectionEntities
{
    public class DetectionInputImageTests
    {
        [Fact]
        public void DetectionInputImage_ShouldInitialize_WithDefaultValues()
        {
            var detectionInputImage = new DetectionInputImage();

            Assert.Null(detectionInputImage.Name);
            Assert.Null(detectionInputImage.Description);
            Assert.Equal(default, detectionInputImage.DateTaken);
            Assert.Null(detectionInputImage.ImagePath);
            Assert.Null(detectionInputImage.ImageFileName);

            Assert.Null(detectionInputImage.CreatedById);
            Assert.Equal(default, detectionInputImage.CreatedOn);
            Assert.Null(detectionInputImage.CreatedBy);

            Assert.Null(detectionInputImage.UpdatedById);
            Assert.Null(detectionInputImage.UpdatedOn);
            Assert.Null(detectionInputImage.UpdatedBy);
        }


        [Fact]
        public void DetectionInputImage_ShouldSetAndGetName()
        {
            var inputImage = new DetectionInputImage();
            var name = "Test Input Image";

            inputImage.Name = name;

            Assert.Equal(name, inputImage.Name);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetDescription()
        {
            var inputImage = new DetectionInputImage();
            var description = "Test Input Image Description";

            inputImage.Description = description;

            Assert.Equal(description, inputImage.Description);
        }


        [Fact]
        public void DetectionInputImage_ShouldSetAndGetImagePath()
        {
            var inputImage = new DetectionInputImage();
            var imagePath = "/images/test-image.png";

            inputImage.ImagePath = imagePath;

            Assert.Equal(imagePath, inputImage.ImagePath);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetImageFileName()
        {
            var inputImage = new DetectionInputImage();
            var imageFileName = "test-image.png";

            inputImage.ImageFileName = imageFileName;

            Assert.Equal(imageFileName, inputImage.ImageFileName);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetDateTaken()
        {
            var inputImage = new DetectionInputImage();
            var dateTaken = DateTime.UtcNow;

            inputImage.DateTaken = dateTaken;

            Assert.Equal(dateTaken, inputImage.DateTaken);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetCreatedById()
        {
            var inputImage = new DetectionInputImage();
            var createdById = "test-user-123";

            inputImage.CreatedById = createdById;

            Assert.Equal(createdById, inputImage.CreatedById);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetCreatedOn()
        {
            var inputImage = new DetectionInputImage();
            var createdOn = DateTime.UtcNow;

            inputImage.CreatedOn = createdOn;

            Assert.Equal(createdOn, inputImage.CreatedOn);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetCreatedBy()
        {
            var inputImage = new DetectionInputImage();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            inputImage.CreatedBy = createdBy;

            Assert.Equal(createdBy, inputImage.CreatedBy);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetUpdatedById()
        {
            var inputImage = new DetectionInputImage();
            var updatedById = "test-user-123";

            inputImage.UpdatedById = updatedById;

            Assert.Equal(updatedById, inputImage.UpdatedById);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetUpdatedOn()
        {
            var inputImage = new DetectionInputImage();
            var updatedOn = DateTime.UtcNow;

            inputImage.UpdatedOn = updatedOn;

            Assert.Equal(updatedOn, inputImage.UpdatedOn);
        }

        [Fact]
        public void DetectionInputImage_ShouldSetAndGetUpdatedBy()
        {
            var inputImage = new DetectionInputImage();
            var updatedBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            inputImage.UpdatedBy = updatedBy;

            Assert.Equal(updatedBy, inputImage.UpdatedBy);
        }

    }
}
