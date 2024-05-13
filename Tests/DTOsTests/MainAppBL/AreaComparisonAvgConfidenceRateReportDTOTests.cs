using DTOs.MainApp.BL.DetectionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class AreaComparisonAvgConfidenceRateReportDTOTests
    {
        [Fact]
        public void DTO_Initialization_AllPropertiesShouldBeNull()
        {
            // Arrange
            var dto = new AreaComparisonAvgConfidenceRateReportDTO();

            // Act

            // Assert
            Assert.Null(dto.DetectionRunId);
            Assert.Null(dto.DetectionRunName);
            Assert.Null(dto.DetectionRunDescription);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.IsCompleted);
            Assert.Null(dto.TotalAreaOfDetectionRun);
            Assert.Null(dto.AvgConfidenceRate);
            Assert.Null(dto.AllConfidenceRates);
            Assert.Null(dto.GroupedDumpSitesList);
        }

        [Fact]
        public void DTO_SetProperties_AllPropertiesShouldBeSetCorrectly()
        {
            // Arrange
            var dto = new AreaComparisonAvgConfidenceRateReportDTO
            {
                DetectionRunId = Guid.NewGuid(),
                DetectionRunName = "Test Detection Run",
                DetectionRunDescription = "Test Description",
                CreatedBy = "TestUser",
                CreatedOn = DateTime.Now,
                IsCompleted = true,
                TotalAreaOfDetectionRun = 100.0,
                AvgConfidenceRate = 0.75,
                AllConfidenceRates = new List<double?> { 0.7, 0.8, 0.9 },
                GroupedDumpSitesList = new List<GroupedDumpSitesListHistoricDataDTO>()
            };

            // Act

            // Assert
            Assert.NotNull(dto.DetectionRunId);
            Assert.Equal("Test Detection Run", dto.DetectionRunName);
            Assert.Equal("Test Description", dto.DetectionRunDescription);
            Assert.Equal("TestUser", dto.CreatedBy);
            Assert.NotNull(dto.CreatedOn);
            Assert.True(dto.IsCompleted);
            Assert.Equal(100.0, dto.TotalAreaOfDetectionRun);
            Assert.Equal(0.75, dto.AvgConfidenceRate);
            Assert.Collection(dto.AllConfidenceRates,
                item => Assert.Equal(0.7, item),
                item => Assert.Equal(0.8, item),
                item => Assert.Equal(0.9, item));
            Assert.NotNull(dto.GroupedDumpSitesList);
            Assert.Empty(dto.GroupedDumpSitesList);
        }
    }
}
