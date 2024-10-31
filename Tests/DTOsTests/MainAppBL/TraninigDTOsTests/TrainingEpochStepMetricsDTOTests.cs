using DTOs.MainApp.BL.TrainingDTOs;

namespace Tests.DTOsTests.MainAppBL.TraninigDTOsTests
{
    public class TrainingEpochStepMetricsDTOTests
    {
        [Fact]
        public void StepProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var step = 3;
            var dto = new TrainingEpochStepMetricsDTO { Step = step };

            // Act & Assert
            Assert.Equal(step, dto.Step);
        }

        [Fact]
        public void MetricsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var metrics = new Dictionary<string, double> { { "accuracy", 0.95 }, { "loss", 0.05 } };
            var dto = new TrainingEpochStepMetricsDTO { Metrics = metrics };

            // Act & Assert
            Assert.Equal(metrics, dto.Metrics);
        }
    }
}
