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
    public class TrainingRunTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var createdById = "user123";

            // Act
            var trainingRun = new TrainingRun
            {
                Name = "Training A",
                IsCompleted = true,
                DatasetId = Guid.NewGuid(),
                CreatedById = createdById,
                CreatedOn = createdOn
            };

            // Assert
            Assert.Equal("Training A", trainingRun.Name);
            Assert.True(trainingRun.IsCompleted);
            Assert.NotEqual(Guid.Empty, trainingRun.DatasetId);
            Assert.Equal(createdById, trainingRun.CreatedById);
            Assert.Equal(createdOn, trainingRun.CreatedOn);
        }

        [Fact]
        public void Constructor_DefaultValuesShouldBeSet()
        {
            // Act
            var trainingRun = new TrainingRun();

            // Assert
            Assert.False(trainingRun.IsCompleted);
            Assert.Null(trainingRun.TrainedModel);
            Assert.Null(trainingRun.BaseModel);
        }

        [Fact]
        public void CanAssignRelationships()
        {
            // Arrange
            var dataset = new Dataset();
            var trainedModel = new TrainedModel();
            var baseModel = new TrainedModel();
            var user = new ApplicationUser();

            // Act
            var trainingRun = new TrainingRun
            {
                Dataset = dataset,
                TrainedModel = trainedModel,
                BaseModel = baseModel,
                CreatedBy = user
            };

            // Assert
            Assert.Same(dataset, trainingRun.Dataset);
            Assert.Same(trainedModel, trainingRun.TrainedModel);
            Assert.Same(baseModel, trainingRun.BaseModel);
            Assert.Same(user, trainingRun.CreatedBy);
        }

        [Fact]
        public void SettingNullRelationships_ShouldNotThrow()
        {
            // Arrange
            var trainingRun = new TrainingRun();

            // Act & Assert
            Exception ex = Record.Exception(() => trainingRun.TrainedModel = null);
            Assert.Null(ex);

            ex = Record.Exception(() => trainingRun.BaseModel = null);
            Assert.Null(ex);
        }
    }
}
