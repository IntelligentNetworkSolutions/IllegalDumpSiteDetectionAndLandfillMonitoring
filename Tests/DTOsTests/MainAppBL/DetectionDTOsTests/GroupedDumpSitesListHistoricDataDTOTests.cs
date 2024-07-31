using DTOs.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class GroupedDumpSitesListHistoricDataDTOTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            }));

            var className = "TestClassName";
            var geoms = new List<Polygon> { polygon };
            var geoJsons = geoms.Select(GeoJsonHelpers.GeometryToGeoJson).ToList();
            var geomAreas = new List<double> { 10.5, 20.3 };
            var totalGroupArea = 30.8;

            // Act
            var dto = new GroupedDumpSitesListHistoricDataDTO
            {
                ClassName = className,
                Geoms = geoms,
                GeomAreas = geomAreas,
                TotalGroupArea = totalGroupArea
            };

            // Assert
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(geoJsons, dto.GeoJsons);
            Assert.Equal(geomAreas, dto.GeomAreas);
            Assert.Equal(totalGroupArea, dto.TotalGroupArea);
        }

        [Fact]
        public void GeoJsons_ShouldReturnListOfGeoJsonStrings()
        {
            // Arrange
            var className = "TestClassName";
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0) 
            }));
            var geoms = new List<Polygon> { polygon };
            var dto = new GroupedDumpSitesListHistoricDataDTO
            {
                ClassName = className,
                Geoms = geoms
            };

            // Act
            var geoJsons = dto.GeoJsons;

            // Assert
            Assert.NotNull(geoJsons);
            Assert.NotEmpty(geoJsons);
            Assert.Single(geoJsons);
            Assert.Equal(GeoJsonHelpers.GeometryToGeoJson(polygon), geoJsons[0]);
        }
    }
}
