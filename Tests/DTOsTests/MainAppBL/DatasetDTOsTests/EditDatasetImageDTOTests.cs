using DTOs.MainApp.BL.DatasetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class EditDatasetImageDTOTests
    {
        [Fact]
        public void EditDatasetImageDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var isEnabled = true;
            var name = "Test Image";
            var fileName = "test_image.png";
            var updatedById = "user123";

            // Act
            var dto = new EditDatasetImageDTO
            {
                Id = id,
                DatasetId = datasetId,
                IsEnabled = isEnabled,
                Name = name,
                FileName = fileName,
                UpdatedById = updatedById
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(isEnabled, dto.IsEnabled);
            Assert.Equal(name, dto.Name);
            Assert.Equal(fileName, dto.FileName);
            Assert.Equal(updatedById, dto.UpdatedById);
        }

        [Fact]
        public void EditDatasetImageDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new EditDatasetImageDTO();

            // Assert
            Assert.Equal(default(Guid), dto.Id);
            Assert.Equal(default(Guid), dto.DatasetId);
            Assert.False(dto.IsEnabled);
            Assert.Null(dto.Name);
            Assert.Null(dto.FileName);
            Assert.Null(dto.UpdatedById);
        }

        [Fact]
        public void EditDatasetImageDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new EditDatasetImageDTO();
            var id = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var isEnabled = true;
            var name = "Test Image";
            var fileName = "test_image.png";
            var updatedById = "user123";

            // Act
            dto.Id = id;
            dto.DatasetId = datasetId;
            dto.IsEnabled = isEnabled;
            dto.Name = name;
            dto.FileName = fileName;
            dto.UpdatedById = updatedById;

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(isEnabled, dto.IsEnabled);
            Assert.Equal(name, dto.Name);
            Assert.Equal(fileName, dto.FileName);
            Assert.Equal(updatedById, dto.UpdatedById);
        }
    }
}
