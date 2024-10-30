using DTOs.MainApp.BL.TrainingDTOs;

namespace Tests.DTOsTests.MainAppBL.TraninigDTOsTests
{
    public class TrainingEpochMetricsDTOTests
    {
        [Fact]
        public void EpochProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var epoch = 5;
            var dto = new TrainingEpochMetricsDTO { Epoch = epoch };

            // Act & Assert
            Assert.Equal(epoch, dto.Epoch);
        }

        [Fact]
        public void StepsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var steps = new List<TrainingEpochStepMetricsDTO>
    {
        new TrainingEpochStepMetricsDTO { Step = 1 },
        new TrainingEpochStepMetricsDTO { Step = 2 }
    };
            var dto = new TrainingEpochMetricsDTO { Steps = steps };

            // Act & Assert
            Assert.Equal(steps, dto.Steps);
        }
    }
}
