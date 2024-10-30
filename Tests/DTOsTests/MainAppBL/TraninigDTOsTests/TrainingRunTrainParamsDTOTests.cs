using DTOs.MainApp.BL.TrainingDTOs;

namespace Tests.DTOsTests.MainAppBL.TraninigDTOsTests
{
    public class TrainingRunTrainParamsDTOTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new TrainingRunTrainParamsDTO { Id = id };

            // Act & Assert
            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public void TrainingRunIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var dto = new TrainingRunTrainParamsDTO { TrainingRunId = trainingRunId };

            // Act & Assert
            Assert.Equal(trainingRunId, dto.TrainingRunId);
        }

        [Fact]
        public void TrainingRunProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var trainingRun = new TrainingRunDTO();
            var dto = new TrainingRunTrainParamsDTO { TrainingRun = trainingRun };

            // Act & Assert
            Assert.Equal(trainingRun, dto.TrainingRun);
        }

        [Fact]
        public void NumEpochsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var numEpochs = 50;
            var dto = new TrainingRunTrainParamsDTO { NumEpochs = numEpochs };

            // Act & Assert
            Assert.Equal(numEpochs, dto.NumEpochs);
        }

        [Fact]
        public void BatchSizeProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var batchSize = 32;
            var dto = new TrainingRunTrainParamsDTO { BatchSize = batchSize };

            // Act & Assert
            Assert.Equal(batchSize, dto.BatchSize);
        }

        [Fact]
        public void NumFrozenStagesProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var numFrozenStages = 2;
            var dto = new TrainingRunTrainParamsDTO { NumFrozenStages = numFrozenStages };

            // Act & Assert
            Assert.Equal(numFrozenStages, dto.NumFrozenStages);
        }

    }
}
