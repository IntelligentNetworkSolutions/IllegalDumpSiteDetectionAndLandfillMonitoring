using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.TraninigDTOsTests
{
    public class TrainingRunDTOTests
    {
        [Fact]
        public void TrainingRunDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Training Run 1";
            var isCompleted = true;
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModelDTO();
            var baseModelId = Guid.NewGuid();
            var baseModel = new TrainedModelDTO();
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();

            // Act
            var dto = new TrainingRunDTO
            {
                Id = id,
                Name = name,
                IsCompleted = isCompleted,
                DatasetId = datasetId,
                Dataset = dataset,
                TrainedModelId = trainedModelId,
                TrainedModel = trainedModel,
                BaseModelId = baseModelId,
                BaseModel = baseModel,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(isCompleted, dto.IsCompleted);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(trainedModelId, dto.TrainedModelId);
            Assert.Equal(trainedModel, dto.TrainedModel);
            Assert.Equal(baseModelId, dto.BaseModelId);
            Assert.Equal(baseModel, dto.BaseModel);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
        }

        [Fact]
        public void TrainingRunDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new TrainingRunDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.Name);
            Assert.False(dto.IsCompleted);
            Assert.Null(dto.DatasetId);
            Assert.Null(dto.Dataset);
            Assert.Null(dto.TrainedModelId);
            Assert.Null(dto.TrainedModel);
            Assert.Null(dto.BaseModelId);
            Assert.Null(dto.BaseModel);
            Assert.Null(dto.CreatedById);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
        }

        [Fact]
        public void TrainingRunDTO_Properties_ShouldBeInitializedWithDefaultValues()
        {
            // Act
            var dto = new TrainingRunDTO();

            // Assert
            Assert.False(dto.IsCompleted);
        }
    }
}
