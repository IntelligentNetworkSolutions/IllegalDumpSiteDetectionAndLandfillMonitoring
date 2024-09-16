using DTOs.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.DetectionTests
{
    public class DetectedDumpSiteViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new DetectedDumpSiteViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void ConfidenceRateProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var confidenceRate = 0.85;
            var viewModel = new DetectedDumpSiteViewModel
            {
                ConfidenceRate = confidenceRate
            };

            // Act & Assert
            Assert.Equal(confidenceRate, viewModel.ConfidenceRate);
        }

        [Fact]
        public void DetectionRunIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var viewModel = new DetectedDumpSiteViewModel
            {
                DetectionRunId = detectionRunId
            };

            // Act & Assert
            Assert.Equal(detectionRunId, viewModel.DetectionRunId);
        }

        [Fact]
        public void DetectionRunProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var detectionRun = new DetectionRunViewModel();
            var viewModel = new DetectedDumpSiteViewModel
            {
                DetectionRun = detectionRun
            };

            // Act & Assert
            Assert.Equal(detectionRun, viewModel.DetectionRun);
        }

        [Fact]
        public void DatasetClassIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var datasetClassId = Guid.NewGuid();
            var viewModel = new DetectedDumpSiteViewModel
            {
                DatasetClassId = datasetClassId
            };

            // Act & Assert
            Assert.Equal(datasetClassId, viewModel.DatasetClassId);
        }

        [Fact]
        public void DatasetClassProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var datasetClass = new DatasetClassDTO();
            var viewModel = new DetectedDumpSiteViewModel
            {
                DatasetClass = datasetClass
            };

            // Act & Assert
            Assert.Equal(datasetClass, viewModel.DatasetClass);
        }

        [Fact]
        public void GeomProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var coordinates = new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            };

            var linearRing = new LinearRing(coordinates);
            var geom = new Polygon(linearRing);  // Assuming Polygon is a class; adjust as needed
            var viewModel = new DetectedDumpSiteViewModel
            {
                Geom = geom
            };

            // Act & Assert
            Assert.Equal(geom, viewModel.Geom);
        }

        [Fact]
        public void GeoJsonProperty_ShouldReturnCorrectGeoJson()
        {
            // Arrange
            var coordinates = new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            };

            var linearRing = new LinearRing(coordinates);
            var geom = new Polygon(linearRing);
            var expectedGeoJson = GeoJsonHelpers.GeometryToGeoJson(geom);
            var viewModel = new DetectedDumpSiteViewModel
            {
                Geom = geom
            };

            // Act & Assert
            Assert.Equal(expectedGeoJson, viewModel.GeoJson);
        }

       
    }
}
