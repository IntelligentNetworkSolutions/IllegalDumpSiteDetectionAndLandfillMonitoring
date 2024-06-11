using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class DatasetImageDTOTests
    {
        [Fact]
        public void DatasetImageDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fileName = "test.png";
            var name = "Test Image";
            var imagePath = "/images/test.png";
            var thumbnailPath = "/images/thumbnails/test.png";
            var isEnabled = true;
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var createdById = "Creator";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var updatedById = "Updater";
            var updatedOn = new DateTime(2023, 2, 1);
            var updatedBy = new UserDTO();

            // Act
            var dto = new DatasetImageDTO
            {
                Id = id,
                FileName = fileName,
                Name = name,
                ImagePath = imagePath,
                ThumbnailPath = thumbnailPath,
                IsEnabled = isEnabled,
                DatasetId = datasetId,
                Dataset = dataset,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(fileName, dto.FileName);
            Assert.Equal(name, dto.Name);
            Assert.Equal(imagePath, dto.ImagePath);
            Assert.Equal(thumbnailPath, dto.ThumbnailPath);
            Assert.True(dto.IsEnabled);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
        }

        [Fact]
        public void DatasetImageDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DatasetImageDTO();

            // Assert
            Assert.Equal(default(Guid), dto.Id);
            Assert.Null(dto.FileName);
            Assert.Null(dto.Name);
            Assert.Null(dto.ImagePath);
            Assert.Null(dto.ThumbnailPath);
            Assert.False(dto.IsEnabled);
            Assert.Null(dto.DatasetId);
            Assert.Null(dto.Dataset);
            Assert.Null(dto.CreatedById);
            Assert.Equal(DateTime.UtcNow.Date, dto.CreatedOn.Date);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.UpdatedBy);
        }

        [Fact]
        public void DatasetImageDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new DatasetImageDTO();
            var id = Guid.NewGuid();
            var fileName = "test.png";
            var name = "Test Image";
            var imagePath = "/images/test.png";
            var thumbnailPath = "/images/thumbnails/test.png";
            var isEnabled = true;
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var createdById = "Creator";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var updatedById = "Updater";
            var updatedOn = new DateTime(2023, 2, 1);
            var updatedBy = new UserDTO();

            // Act
            dto = dto with
            {
                Id = id,
                FileName = fileName,
                Name = name,
                ImagePath = imagePath,
                ThumbnailPath = thumbnailPath,
                IsEnabled = isEnabled,
                DatasetId = datasetId,
                Dataset = dataset,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(fileName, dto.FileName);
            Assert.Equal(name, dto.Name);
            Assert.Equal(imagePath, dto.ImagePath);
            Assert.Equal(thumbnailPath, dto.ThumbnailPath);
            Assert.True(dto.IsEnabled);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
        }
    }
}
