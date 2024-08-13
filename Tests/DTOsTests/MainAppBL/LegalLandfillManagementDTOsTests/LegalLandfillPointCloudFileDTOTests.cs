using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class LegalLandfillPointCloudFileDTOTests
    {
        [Fact]
        public void Properties_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var pointCloudFileDTO = new LegalLandfillPointCloudFileDTO();
            var id = Guid.NewGuid();
            var fileName = "testfile.las";
            var filePath = "/files/testfile.las";
            var scanDateTime = DateTime.UtcNow;
            var legalLandfillId = Guid.NewGuid();

            // Act
            pointCloudFileDTO.Id = id;
            pointCloudFileDTO.FileName = fileName;
            pointCloudFileDTO.FilePath = filePath;
            pointCloudFileDTO.ScanDateTime = scanDateTime;
            pointCloudFileDTO.LegalLandfillId = legalLandfillId;

            // Assert
            Assert.Equal(id, pointCloudFileDTO.Id);
            Assert.Equal(fileName, pointCloudFileDTO.FileName);
            Assert.Equal(filePath, pointCloudFileDTO.FilePath);
            Assert.Equal(scanDateTime, pointCloudFileDTO.ScanDateTime);
            Assert.Equal(legalLandfillId, pointCloudFileDTO.LegalLandfillId);
        }

        [Fact]
        public void LegalLandfill_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var pointCloudFileDTO = new LegalLandfillPointCloudFileDTO();
            var legalLandfillDTO = new LegalLandfillDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Landfill",
                Description = "Test Description"
            };

            // Act
            pointCloudFileDTO.LegalLandfill = legalLandfillDTO;

            // Assert
            Assert.Equal(legalLandfillDTO, pointCloudFileDTO.LegalLandfill);
        }
    }
}
