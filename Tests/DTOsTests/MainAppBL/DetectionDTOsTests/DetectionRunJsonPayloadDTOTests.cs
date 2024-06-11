using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectionRunJsonPayloadDTOTests
    {
        [Fact]
        public void DetectionRunJsonPayloadDTO_DefaultConstructor_ShouldInitializeId()
        {
            var detectionImagePath = "/images/detection.png";
            var detectionImageFileName = "detection.png";
            var trainedModelName = "Model 1";
            var trainedModelPath = "/models/model1";
            // Act
            var dto = new DetectionRunJsonPayloadDTO()
            {
                DetectionImagePath = detectionImagePath,
                DetectionImageFileName = detectionImageFileName,
                TrainedModelName = trainedModelName,
                TrainedModelPath = trainedModelPath
            };

            // Assert
            Assert.NotEqual(Guid.Empty, dto.Id);
        }

        [Fact]
        public void DetectionRunJsonPayloadDTO_ShouldBeRecordType()
        {
            // Arrange & Act
            var detectionImagePath = "/images/detection.png";
            var detectionImageFileName = "detection.png";
            var trainedModelName = "Model 1";
            var trainedModelPath = "/models/model1";
            // Act
            var dto = new DetectionRunJsonPayloadDTO()
            {
                DetectionImagePath = detectionImagePath,
                DetectionImageFileName = detectionImageFileName,
                TrainedModelName = trainedModelName,
                TrainedModelPath = trainedModelPath
            };

            // Assert
            Assert.IsType<DetectionRunJsonPayloadDTO>(dto);
        }
    }
}
