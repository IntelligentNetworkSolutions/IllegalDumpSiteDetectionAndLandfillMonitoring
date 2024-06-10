using DTOs.MainApp.BL.TrainingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.TraninigDTOsTests
{
    public class TrainedModelStatisticsDTOTests
    {
        [Fact]
        public void TrainedModelStatisticsDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();
            var trainedModel = new TrainedModelDTO();
            var trainingDuration = TimeSpan.FromHours(5);
            var totalParameters = 50000;
            var numEpochs = 50.5;
            var learningRate = 0.01;
            var avgBoxLoss = 0.05;
            var mApp = 0.75;

            // Act
            var dto = new TrainedModelStatisticsDTO
            {
                Id = id,
                TrainedModelId = trainedModelId,
                TrainedModel = trainedModel,
                TrainingDuration = trainingDuration,
                TotalParameters = totalParameters,
                NumEpochs = numEpochs,
                LearningRate = learningRate,
                AvgBoxLoss = avgBoxLoss,
                mApp = mApp
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(trainedModelId, dto.TrainedModelId);
            Assert.Equal(trainedModel, dto.TrainedModel);
            Assert.Equal(trainingDuration, dto.TrainingDuration);
            Assert.Equal(totalParameters, dto.TotalParameters);
            Assert.Equal(numEpochs, dto.NumEpochs);
            Assert.Equal(learningRate, dto.LearningRate);
            Assert.Equal(avgBoxLoss, dto.AvgBoxLoss);
            Assert.Equal(mApp, dto.mApp);
        }

        [Fact]
        public void TrainedModelStatisticsDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new TrainedModelStatisticsDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.TrainedModelId);
            Assert.Null(dto.TrainedModel);
            Assert.Null(dto.TrainingDuration);
            Assert.Null(dto.TotalParameters);
            Assert.Null(dto.NumEpochs);
            Assert.Null(dto.LearningRate);
            Assert.Null(dto.AvgBoxLoss);
            Assert.Null(dto.mApp);
        }

        [Fact]
        public void TrainedModelStatisticsDTO_ShouldBeClassType()
        {
            // Arrange & Act
            var dto = new TrainedModelStatisticsDTO();

            // Assert
            Assert.IsType<TrainedModelStatisticsDTO>(dto);
        }
    }
}
