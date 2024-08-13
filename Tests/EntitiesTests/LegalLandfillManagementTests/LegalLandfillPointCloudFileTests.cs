using Entities.LegalLandfillsManagementEntites;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.LegalLandfillManagementTests
{
    public class LegalLandfillPointCloudFileTests
    {
        [Fact]
        public void LegalLandfillPointCloudFile_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var pointCloudFile = new LegalLandfillPointCloudFile();

            // Assert
            Assert.True(pointCloudFile is BaseEntity<Guid>);
        }

        [Fact]
        public void FileName_ShouldBeSettableAndGettable()
        {
            // Arrange
            var pointCloudFile = new LegalLandfillPointCloudFile();
            var expectedFileName = "test_file.xyz";

            // Act
            pointCloudFile.FileName = expectedFileName;

            // Assert
            Assert.Equal(expectedFileName, pointCloudFile.FileName);
        }

        [Fact]
        public void FilePath_ShouldBeSettableAndGettable()
        {
            // Arrange
            var pointCloudFile = new LegalLandfillPointCloudFile();
            var expectedFilePath = "/path/to/test_file.xyz";

            // Act
            pointCloudFile.FilePath = expectedFilePath;

            // Assert
            Assert.Equal(expectedFilePath, pointCloudFile.FilePath);
        }

        [Fact]
        public void ScanDateTime_ShouldBeSettableAndGettable()
        {
            // Arrange
            var pointCloudFile = new LegalLandfillPointCloudFile();
            var expectedDateTime = new DateTime(2024, 8, 5, 12, 0, 0);

            // Act
            pointCloudFile.ScanDateTime = expectedDateTime;

            // Assert
            Assert.Equal(expectedDateTime, pointCloudFile.ScanDateTime);
        }

        [Fact]
        public void LegalLandfillId_ShouldBeSettableAndGettable()
        {
            // Arrange
            var pointCloudFile = new LegalLandfillPointCloudFile();
            var expectedId = Guid.NewGuid();

            // Act
            pointCloudFile.LegalLandfillId = expectedId;

            // Assert
            Assert.Equal(expectedId, pointCloudFile.LegalLandfillId);
        }

        [Fact]
        public void LegalLandfill_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var pointCloudFile = new LegalLandfillPointCloudFile();

            // Assert
            Assert.Null(pointCloudFile.LegalLandfill);
        }

        [Fact]
        public void LegalLandfill_ShouldBeSettableAndGettable()
        {
            // Arrange
            var pointCloudFile = new LegalLandfillPointCloudFile();
            var legalLandfill = new LegalLandfill();

            // Act
            pointCloudFile.LegalLandfill = legalLandfill;

            // Assert
            Assert.Equal(legalLandfill, pointCloudFile.LegalLandfill);
        }
    }
}
