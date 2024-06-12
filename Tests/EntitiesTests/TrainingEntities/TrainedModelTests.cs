using Entities.TrainingEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace Tests.EntitiesTests.TrainingEntities
{
    public class TrainedModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var createdById = "user123";

            // Act
            var trainedModel = new TrainedModel
            {
                Name = "Model A",
                ModelFilePath = "/path/to/model",
                IsPublished = true,
                DatasetId = Guid.NewGuid(),
                CreatedById = createdById,
                CreatedOn = createdOn
            };

            // Assert
            Assert.Equal("Model A", trainedModel.Name);
            Assert.Equal("/path/to/model", trainedModel.ModelFilePath);
            Assert.True(trainedModel.IsPublished);
            Assert.NotEqual(Guid.Empty, trainedModel.DatasetId);
            Assert.Equal(createdById, trainedModel.CreatedById);
            Assert.Equal(createdOn, trainedModel.CreatedOn);
        }

        [Fact]
        public void Constructor_DefaultValuesShouldBeSet()
        {
            // Act
            var trainedModel = new TrainedModel();

            // Assert
            Assert.False(trainedModel.IsPublished);
            Assert.Null(trainedModel.BaseModel);
            Assert.Null(trainedModel.TrainedModelStatistics);
        }

        [Fact]
        public void CanAssignRelationships()
        {
            // Arrange
            var dataset = new Dataset();
            var trainingRun = new TrainingRun();
            var user = new ApplicationUser();

            // Act
            var trainedModel = new TrainedModel
            {
                Dataset = dataset,
                TrainingRun = trainingRun,
                CreatedBy = user
            };

            // Assert
            Assert.Same(dataset, trainedModel.Dataset);
            Assert.Same(trainingRun, trainedModel.TrainingRun);
            Assert.Same(user, trainedModel.CreatedBy);
        }

        [Fact]
        public void BaseModel_OptionalRelationship()
        {
            // Arrange
            var baseModel = new TrainedModel();

            // Act
            var trainedModel = new TrainedModel { BaseModel = baseModel };

            // Assert
            Assert.Same(baseModel, trainedModel.BaseModel);
        }

        [Fact]
        public void SettingNullBaseModel_ShouldNotThrow()
        {
            // Arrange
            var trainedModel = new TrainedModel();

            // Act & Assert
            Exception ex = Record.Exception(() => trainedModel.BaseModel = null);
            Assert.Null(ex);
        }
    }
}
