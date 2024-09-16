using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class WasteVolumeComparisonDTOTests
    {
        [Fact]
        public void WasteVolumeComparisonDTO_Properties_ShouldBeInitializedCorrectly()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = "FileA.las",
                FileBName = "FileB.las",
                Difference = 5.75,
                ScanDateFileA = "2024-08-01",
                ScanDateFileB = "2024-08-02"
            };

            // Act & Assert
            Assert.Equal("FileA.las", dto.FileAName);
            Assert.Equal("FileB.las", dto.FileBName);
            Assert.Equal(5.75, dto.Difference);
            Assert.Equal("2024-08-01", dto.ScanDateFileA);
            Assert.Equal("2024-08-02", dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_FileAName_ShouldAllowNullOrEmpty()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = null,
                FileBName = "FileB.las",
                Difference = 10.5,
                ScanDateFileA = "2024-08-01",
                ScanDateFileB = "2024-08-02"
            };

            // Act & Assert
            Assert.Null(dto.FileAName);
            Assert.Equal("FileB.las", dto.FileBName);
            Assert.Equal(10.5, dto.Difference);
            Assert.Equal("2024-08-01", dto.ScanDateFileA);
            Assert.Equal("2024-08-02", dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_FileBName_ShouldAllowNullOrEmpty()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = "FileA.las",
                FileBName = null,
                Difference = 10.5,
                ScanDateFileA = "2024-08-01",
                ScanDateFileB = "2024-08-02"
            };

            // Act & Assert
            Assert.Equal("FileA.las", dto.FileAName);
            Assert.Null(dto.FileBName);
            Assert.Equal(10.5, dto.Difference);
            Assert.Equal("2024-08-01", dto.ScanDateFileA);
            Assert.Equal("2024-08-02", dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_Difference_ShouldAllowNull()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = "FileA.las",
                FileBName = "FileB.las",
                Difference = null,
                ScanDateFileA = "2024-08-01",
                ScanDateFileB = "2024-08-02"
            };

            // Act & Assert
            Assert.Equal("FileA.las", dto.FileAName);
            Assert.Equal("FileB.las", dto.FileBName);
            Assert.Null(dto.Difference);
            Assert.Equal("2024-08-01", dto.ScanDateFileA);
            Assert.Equal("2024-08-02", dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_ScanDateFileA_ShouldAllowNullOrEmpty()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = "FileA.las",
                FileBName = "FileB.las",
                Difference = 5.0,
                ScanDateFileA = null,
                ScanDateFileB = "2024-08-02"
            };

            // Act & Assert
            Assert.Equal("FileA.las", dto.FileAName);
            Assert.Equal("FileB.las", dto.FileBName);
            Assert.Equal(5.0, dto.Difference);
            Assert.Null(dto.ScanDateFileA);
            Assert.Equal("2024-08-02", dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_ScanDateFileB_ShouldAllowNullOrEmpty()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO
            {
                FileAName = "FileA.las",
                FileBName = "FileB.las",
                Difference = 7.25,
                ScanDateFileA = "2024-08-01",
                ScanDateFileB = null
            };

            // Act & Assert
            Assert.Equal("FileA.las", dto.FileAName);
            Assert.Equal("FileB.las", dto.FileBName);
            Assert.Equal(7.25, dto.Difference);
            Assert.Equal("2024-08-01", dto.ScanDateFileA);
            Assert.Null(dto.ScanDateFileB);
        }

        [Fact]
        public void WasteVolumeComparisonDTO_AllProperties_ShouldBeNullOrEmptyByDefault()
        {
            // Arrange
            var dto = new WasteVolumeComparisonDTO();

            // Act & Assert
            Assert.Null(dto.FileAName);
            Assert.Null(dto.FileBName);
            Assert.Null(dto.Difference);
            Assert.Null(dto.ScanDateFileA);
            Assert.Null(dto.ScanDateFileB);
        }
    }
}
