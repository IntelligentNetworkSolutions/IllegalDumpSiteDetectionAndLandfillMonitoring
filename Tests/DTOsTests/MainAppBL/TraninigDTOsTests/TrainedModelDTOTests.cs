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
    public class TrainedModelDTOTests
    {
        [Fact]
        public void TrainedModelDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Model";
            var modelFilePath = "/models/test.model";
            var isPublished = true;
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var trainingRunId = Guid.NewGuid();
            var trainingRun = new TrainingRunDTO();
            var baseModelId = Guid.NewGuid();
            var baseModel = new TrainedModelDTO();
            var trainedModelStatisticsId = Guid.NewGuid();
            var trainedModelStatistics = new TrainedModelStatisticsDTO();
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();

            // Act
            var dto = new TrainedModelDTO
            {
                Id = id,
                Name = name,
                ModelFilePath = modelFilePath,
                IsPublished = isPublished,
                DatasetId = datasetId,
                Dataset = dataset,
                TrainingRunId = trainingRunId,
                TrainingRun = trainingRun,
                BaseModelId = baseModelId,
                BaseModel = baseModel,
                TrainedModelStatisticsId = trainedModelStatisticsId,
                TrainedModelStatistics = trainedModelStatistics,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(modelFilePath, dto.ModelFilePath);
            Assert.Equal(isPublished, dto.IsPublished);
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(trainingRunId, dto.TrainingRunId);
            Assert.Equal(trainingRun, dto.TrainingRun);
            Assert.Equal(baseModelId, dto.BaseModelId);
            Assert.Equal(baseModel, dto.BaseModel);
            Assert.Equal(trainedModelStatisticsId, dto.TrainedModelStatisticsId);
            Assert.Equal(trainedModelStatistics, dto.TrainedModelStatistics);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
        }

        [Fact]
        public void TrainedModelDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new TrainedModelDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.Name);
            Assert.Null(dto.ModelFilePath);
            Assert.False(dto.IsPublished);
            Assert.Null(dto.DatasetId);
            Assert.Null(dto.Dataset);
            Assert.Null(dto.TrainingRunId);
            Assert.Null(dto.TrainingRun);
            Assert.Null(dto.BaseModelId);
            Assert.Null(dto.BaseModel);
            Assert.Null(dto.TrainedModelStatisticsId);
            Assert.Null(dto.TrainedModelStatistics);
            Assert.Null(dto.CreatedById);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
        }

        [Fact]
        public void TrainedModelDTO_ShouldBeClassType()
        {
            // Arrange & Act
            var dto = new TrainedModelDTO();

            // Assert
            Assert.IsType<TrainedModelDTO>(dto);
        }
    }
}
