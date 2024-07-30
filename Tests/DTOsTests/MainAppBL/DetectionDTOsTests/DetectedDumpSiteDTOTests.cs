using DTOs.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.DetectionDTOs;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectedDumpSiteDTOTests
    {
        [Fact]
        public void DetectedDumpSiteDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var confidenceRate = 0.75;
            var detectionRunId = Guid.NewGuid();
            var detectionRun = new DetectionRunDTO();
            var datasetClassId = Guid.NewGuid();
            var datasetClass = new DatasetClassDTO();
            var geom = new Polygon(new LinearRing(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            }));

            // Act
            var dto = new DetectedDumpSiteDTO
            {
                Id = id,
                ConfidenceRate = confidenceRate,
                DetectionRunId = detectionRunId,
                DetectionRun = detectionRun,
                DatasetClassId = datasetClassId,
                DatasetClass = datasetClass,
                Geom = geom
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(confidenceRate, dto.ConfidenceRate);
            Assert.Equal(detectionRunId, dto.DetectionRunId);
            Assert.Equal(detectionRun, dto.DetectionRun);
            Assert.Equal(datasetClassId, dto.DatasetClassId);
            Assert.Equal(datasetClass, dto.DatasetClass);
            Assert.Equal(geom, dto.Geom);
        }

        [Fact]
        public void DetectedDumpSiteDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DetectedDumpSiteDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.ConfidenceRate);
            Assert.Null(dto.DetectionRunId);
            Assert.Null(dto.DetectionRun);
            Assert.Null(dto.DatasetClassId);
            Assert.Null(dto.DatasetClass);
            Assert.Null(dto.Geom);
        }

        [Fact]
        public void DetectedDumpSiteDTO_GeoJson_ShouldReturnExpectedGeoJson()
        {
            // Arrange
            var geom = new Polygon(new LinearRing(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            }));
            var expectedGeoJson = GeoJsonHelpers.GeometryToGeoJson(geom);

            // Act
            var dto = new DetectedDumpSiteDTO { Geom = geom };
            var actualGeoJson = dto.GeoJson;

            // Assert
            Assert.Equal(expectedGeoJson, actualGeoJson);
        }
    }
}
