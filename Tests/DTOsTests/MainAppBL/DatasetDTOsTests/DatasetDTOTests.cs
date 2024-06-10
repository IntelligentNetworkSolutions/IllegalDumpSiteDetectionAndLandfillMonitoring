using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class DatasetDTOTests
    {
        [Fact]
        public void DatasetDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Dataset";
            var description = "Test Description";
            var isPublished = true;
            var parentDatasetId = Guid.NewGuid();
            var parentDataset = new DatasetDTO();
            var createdById = "Creator";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var updatedById = "Updater";
            var updatedOn = new DateTime(2023, 2, 1);
            var updatedBy = new UserDTO();
            var annotationsPerSubclass = true;

            // Act
            var dto = new DatasetDTO
            {
                Id = id,
                Name = name,
                Description = description,
                IsPublished = isPublished,
                ParentDatasetId = parentDatasetId,
                ParentDataset = parentDataset,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy,
                AnnotationsPerSubclass = annotationsPerSubclass
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.True(dto.IsPublished);
            Assert.Equal(parentDatasetId, dto.ParentDatasetId);
            Assert.Equal(parentDataset, dto.ParentDataset);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
            Assert.True(dto.AnnotationsPerSubclass.Value);
        }

        [Fact]
        public void DatasetDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DatasetDTO();

            // Assert
            Assert.Equal(default(Guid), dto.Id);
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.False(dto.IsPublished);
            Assert.Null(dto.ParentDatasetId);
            Assert.Null(dto.ParentDataset);
            Assert.Null(dto.CreatedById);
            Assert.Equal(DateTime.UtcNow.Date, dto.CreatedOn.Date);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.UpdatedBy);
            Assert.Null(dto.AnnotationsPerSubclass);
        }

        [Fact]
        public void DatasetDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new DatasetDTO();
            var id = Guid.NewGuid();
            var name = "Test Dataset";
            var description = "Test Description";
            var isPublished = true;
            var parentDatasetId = Guid.NewGuid();
            var parentDataset = new DatasetDTO();
            var createdById = "Creator";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var updatedById = "Updater";
            var updatedOn = new DateTime(2023, 2, 1);
            var updatedBy = new UserDTO();
            var annotationsPerSubclass = true;

            // Act
            dto = dto with
            {
                Id = id,
                Name = name,
                Description = description,
                IsPublished = isPublished,
                ParentDatasetId = parentDatasetId,
                ParentDataset = parentDataset,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy,
                AnnotationsPerSubclass = annotationsPerSubclass
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.True(dto.IsPublished);
            Assert.Equal(parentDatasetId, dto.ParentDatasetId);
            Assert.Equal(parentDataset, dto.ParentDataset);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
            Assert.True(dto.AnnotationsPerSubclass.Value);
        }
    }
}
