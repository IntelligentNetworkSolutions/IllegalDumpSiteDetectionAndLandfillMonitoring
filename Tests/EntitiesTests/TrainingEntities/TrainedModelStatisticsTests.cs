using Entities.TrainingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.TrainingEntities
{
    public class TrainedModelStatisticsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var trainedModelId = Guid.NewGuid();
            var trainingDuration = TimeSpan.FromMinutes(30);
            var totalParameters = 1000000;
            var numEpochs = 50.5;
            var learningRate = 0.01;
            var avgBoxLoss = 0.5;
            var mApp = 0.75;

            // Act
            var statistics = new TrainedModelStatistics
            {
                TrainedModelId = trainedModelId,
                TrainingDuration = trainingDuration,
                TotalParameters = totalParameters,
                NumEpochs = numEpochs,
                LearningRate = learningRate,
                AvgBoxLoss = avgBoxLoss,
                mApp = mApp
            };

            // Assert
            Assert.Equal(trainedModelId, statistics.TrainedModelId);
            Assert.Equal(trainingDuration, statistics.TrainingDuration);
            Assert.Equal(totalParameters, statistics.TotalParameters);
            Assert.Equal(numEpochs, statistics.NumEpochs);
            Assert.Equal(learningRate, statistics.LearningRate);
            Assert.Equal(avgBoxLoss, statistics.AvgBoxLoss);
            Assert.Equal(mApp, statistics.mApp);
        }

        [Fact]
        public void Constructor_DefaultValuesShouldBeSet()
        {
            // Act
            var statistics = new TrainedModelStatistics();

            // Assert
            Assert.Null(statistics.TrainingDuration);
            Assert.Null(statistics.TotalParameters);
            Assert.Null(statistics.NumEpochs);
            Assert.Null(statistics.LearningRate);
            Assert.Null(statistics.AvgBoxLoss);
            Assert.Null(statistics.mApp);
        }
               
        [Fact]
        public void CanSetNullableProperties()
        {
            // Arrange
            var statistics = new TrainedModelStatistics();

            // Act
            statistics.TrainingDuration = TimeSpan.FromHours(1);
            statistics.TotalParameters = 2000000;
            statistics.NumEpochs = 100.0;
            statistics.LearningRate = 0.001;
            statistics.AvgBoxLoss = 0.25;
            statistics.mApp = 0.85;

            // Assert
            Assert.Equal(TimeSpan.FromHours(1), statistics.TrainingDuration);
            Assert.Equal(2000000, statistics.TotalParameters);
            Assert.Equal(100.0, statistics.NumEpochs);
            Assert.Equal(0.001, statistics.LearningRate);
            Assert.Equal(0.25, statistics.AvgBoxLoss);
            Assert.Equal(0.85, statistics.mApp);
        }
    }
}
