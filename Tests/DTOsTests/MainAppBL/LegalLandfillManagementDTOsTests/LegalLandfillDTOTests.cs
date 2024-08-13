using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class LegalLandfillDTOTests
    {
        [Fact]
        public void DefaultConstructor_InitializesLegalLandfillPointCloudFiles()
        {
            // Arrange
            var landfillDTO = new LegalLandfillDTO();

            // Act
            var pointCloudFiles = landfillDTO.LegalLandfillPointCloudFiles;

            // Assert
            Assert.NotNull(pointCloudFiles);
            Assert.Empty(pointCloudFiles);
        }

        [Fact]
        public void Properties_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var landfillDTO = new LegalLandfillDTO();
            var id = Guid.NewGuid();
            var name = "Test Landfill";
            var description = "Test Description";
            var pointCloudFile = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FileName = "testfile.las",
                FilePath = "/files/testfile.las"
            };

            // Act
            landfillDTO.Id = id;
            landfillDTO.Name = name;
            landfillDTO.Description = description;
            landfillDTO.LegalLandfillPointCloudFiles?.Add(pointCloudFile);

            // Assert
            Assert.Equal(id, landfillDTO.Id);
            Assert.Equal(name, landfillDTO.Name);
            Assert.Equal(description, landfillDTO.Description);
            Assert.Single(landfillDTO.LegalLandfillPointCloudFiles);
            Assert.Contains(pointCloudFile, landfillDTO.LegalLandfillPointCloudFiles);
        }

        [Fact]
        public void LegalLandfillPointCloudFiles_CanBeModified()
        {
            // Arrange
            var landfillDTO = new LegalLandfillDTO();
            var pointCloudFile1 = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FileName = "file1.las",
                FilePath = "/files/file1.las"
            };
            var pointCloudFile2 = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FileName = "file2.las",
                FilePath = "/files/file2.las"
            };

            // Act
            landfillDTO.LegalLandfillPointCloudFiles?.Add(pointCloudFile1);
            landfillDTO.LegalLandfillPointCloudFiles?.Add(pointCloudFile2);

            // Assert
            Assert.Equal(2, landfillDTO.LegalLandfillPointCloudFiles?.Count);
            Assert.Contains(pointCloudFile1, landfillDTO.LegalLandfillPointCloudFiles);
            Assert.Contains(pointCloudFile2, landfillDTO.LegalLandfillPointCloudFiles);
        }

    }
}
