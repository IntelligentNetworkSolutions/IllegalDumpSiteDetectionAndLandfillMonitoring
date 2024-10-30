using DTOs.MainApp.BL.DetectionDTOs;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectionRunJsonPayloadDTOTests
    {
        [Fact]
        public void DetectionRunJsonPayloadDTO_DefaultConstructor_ShouldInitializeId()
        {
            // Arrange
            var detectionImagePath = "/images/detection.png";
            var detectionImageFileName = "detection.png";
            var trainedModelName = "Model 1";
            var trainedModelPath = "/models/model1";
            var detectionRunId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();

            // Act
            var dto = new DetectionRunJsonPayloadDTO
            {
                DetectionImagePath = detectionImagePath,
                DetectionImageFileName = detectionImageFileName,
                TrainedModelName = trainedModelName,
                TrainedModelPath = trainedModelPath,
                DetectionRunId = detectionRunId,
                TrainedModelId = trainedModelId
            };

            // Assert
            Assert.NotEqual(Guid.Empty, dto.Id);
            Assert.Equal(detectionRunId, dto.DetectionRunId);
            Assert.Equal(trainedModelId, dto.TrainedModelId);
        }

        [Fact]
        public void DetectionRunJsonPayloadDTO_ShouldBeRecordType()
        {
            // Arrange
            var detectionImagePath = "/images/detection.png";
            var detectionImageFileName = "detection.png";
            var trainedModelName = "Model 1";
            var trainedModelPath = "/models/model1";
            var detectionRunId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();

            // Act
            var dto = new DetectionRunJsonPayloadDTO
            {
                DetectionImagePath = detectionImagePath,
                DetectionImageFileName = detectionImageFileName,
                TrainedModelName = trainedModelName,
                TrainedModelPath = trainedModelPath,
                DetectionRunId = detectionRunId,
                TrainedModelId = trainedModelId
            };

            // Assert
            Assert.IsType<DetectionRunJsonPayloadDTO>(dto);
        }

        [Fact]
        public void DetectionRunJsonPayloadDTO_ShouldInitializeWithProvidedValues()
        {
            // Arrange
            var detectionImagePath = "/images/detection.png";
            var detectionImageFileName = "detection.png";
            var trainedModelName = "Model 1";
            var trainedModelPath = "/models/model1";
            var detectionRunId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();

            // Act
            var dto = new DetectionRunJsonPayloadDTO
            {
                DetectionImagePath = detectionImagePath,
                DetectionImageFileName = detectionImageFileName,
                TrainedModelName = trainedModelName,
                TrainedModelPath = trainedModelPath,
                DetectionRunId = detectionRunId,
                TrainedModelId = trainedModelId
            };

            // Assert
            Assert.Equal(detectionImagePath, dto.DetectionImagePath);
            Assert.Equal(detectionImageFileName, dto.DetectionImageFileName);
            Assert.Equal(trainedModelName, dto.TrainedModelName);
            Assert.Equal(trainedModelPath, dto.TrainedModelPath);
            Assert.Equal(detectionRunId, dto.DetectionRunId);
            Assert.Equal(trainedModelId, dto.TrainedModelId);
        }
    }
}
