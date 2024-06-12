using Entities.DatasetEntities;
using Entities.DetectionEntities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.DetectionEntities
{
    public class DetectedDumpSiteTests
    {
        [Fact]
        public void ConfidenceRate_CanBeSetAndRetrieved()
        {
            double? confidenceRate = 0.85;
            var detectedDumpSite = new DetectedDumpSite
            {
                ConfidenceRate = confidenceRate
            };
            Assert.Equal(confidenceRate, detectedDumpSite.ConfidenceRate);
        }

        [Fact]
        public void DatasetClassId_CanBeSetAndRetrieved()
        {
            var datasetClassId = Guid.NewGuid();
            var detectedDumpSite = new DetectedDumpSite
            {
                DatasetClassId = datasetClassId
            };
            Assert.Equal(datasetClassId, detectedDumpSite.DatasetClassId);
        }

        [Fact]
        public void DetectionRunId_CanBeSetAndRetrieved()
        {
            var detectionRunId = Guid.NewGuid();
            var detectedDumpSite = new DetectedDumpSite
            {
                DetectionRunId = detectionRunId
            };
            Assert.Equal(detectionRunId, detectedDumpSite.DetectionRunId);
        }

        [Fact]
        public void Geom_CanBeSetAndRetrieved()
        {
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            }));
            var detectedDumpSite = new DetectedDumpSite
            {
                Geom = polygon
            };
            Assert.Equal(polygon, detectedDumpSite.Geom);
        }

        [Fact]
        public void GeoJson_CanBeSetAndRetrieved()
        {
            var geoJson = "{\"type\": \"Polygon\", \"coordinates\": [[[0, 0], [0, 1], [1, 1], [1, 0], [0, 0]]]}";
            var detectedDumpSite = new DetectedDumpSite
            {
                GeoJson = geoJson
            };
            Assert.Equal(geoJson, detectedDumpSite.GeoJson);
        }

        [Fact]
        public void DatasetClass_CanBeSetAndRetrieved()
        {
            var datasetClass = new DatasetClass();
            var detectedDumpSite = new DetectedDumpSite
            {
                DatasetClass = datasetClass
            };
            Assert.Equal(datasetClass, detectedDumpSite.DatasetClass);
        }

        [Fact]
        public void DetectionRun_CanBeSetAndRetrieved()
        {
            var detectionRun = new DetectionRun();
            var detectedDumpSite = new DetectedDumpSite
            {
                DetectionRun = detectionRun
            };
            Assert.Equal(detectionRun, detectedDumpSite.DetectionRun);
        }
    }
}
