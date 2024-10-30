using DTOs.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using NetTopologySuite.Geometries;

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

            var geomsInIgnoreZone = new List<Polygon> { polygon };
            var geomsOutsideOfIgnoreZone = new List<Polygon> { polygon };
            var geomAreasInIgnoreZone = new List<double> { 5.0 };
            var geomAreasOutsideOfIgnoreZone = new List<double> { 15.0 };
            var totalGroupAreaInIgnoreZone = 5.0;
            var totalGroupAreaOutOfIgnoreZone = 15.0;

            // Act
            var dto = new GroupedDumpSitesListHistoricDataDTO
            {
                ClassName = className,
                Geoms = geoms,
                GeomAreas = geomAreas,
                TotalGroupArea = totalGroupArea,
                GeomsInIgnoreZone = geomsInIgnoreZone,
                GeomsOutsideOfIgnoreZone = geomsOutsideOfIgnoreZone,
                GeomAreasInIgnoreZone = geomAreasInIgnoreZone,
                GeomAreasOutsideOfIgnoreZone = geomAreasOutsideOfIgnoreZone,
                TotalGroupAreaInIgnoreZone = totalGroupAreaInIgnoreZone,
                TotalGroupAreaOutOfIgnoreZone = totalGroupAreaOutOfIgnoreZone
            };

            // Assert
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(geoJsons, dto.GeoJsons);
            Assert.Equal(geomAreas, dto.GeomAreas);
            Assert.Equal(totalGroupArea, dto.TotalGroupArea);
            Assert.Equal(geomsInIgnoreZone, dto.GeomsInIgnoreZone);
            Assert.Equal(geomsOutsideOfIgnoreZone, dto.GeomsOutsideOfIgnoreZone);
            Assert.Equal(geomAreasInIgnoreZone, dto.GeomAreasInIgnoreZone);
            Assert.Equal(geomAreasOutsideOfIgnoreZone, dto.GeomAreasOutsideOfIgnoreZone);
            Assert.Equal(totalGroupAreaInIgnoreZone, dto.TotalGroupAreaInIgnoreZone);
            Assert.Equal(totalGroupAreaOutOfIgnoreZone, dto.TotalGroupAreaOutOfIgnoreZone);
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

        [Fact]
        public void GeomsInIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            }));
            var geomsInIgnoreZone = new List<Polygon> { polygon };
            var dto = new GroupedDumpSitesListHistoricDataDTO { GeomsInIgnoreZone = geomsInIgnoreZone };

            // Act & Assert
            Assert.Equal(geomsInIgnoreZone, dto.GeomsInIgnoreZone);
        }

        [Fact]
        public void GeomsOutsideOfIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            }));
            var geomsOutsideOfIgnoreZone = new List<Polygon> { polygon };
            var dto = new GroupedDumpSitesListHistoricDataDTO { GeomsOutsideOfIgnoreZone = geomsOutsideOfIgnoreZone };

            // Act & Assert
            Assert.Equal(geomsOutsideOfIgnoreZone, dto.GeomsOutsideOfIgnoreZone);
        }

        [Fact]
        public void GeomAreasInIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var geomAreasInIgnoreZone = new List<double> { 5.0, 10.0 };
            var dto = new GroupedDumpSitesListHistoricDataDTO { GeomAreasInIgnoreZone = geomAreasInIgnoreZone };

            // Act & Assert
            Assert.Equal(geomAreasInIgnoreZone, dto.GeomAreasInIgnoreZone);
        }

        [Fact]
        public void GeomAreasOutsideOfIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var geomAreasOutsideOfIgnoreZone = new List<double> { 15.0, 20.0 };
            var dto = new GroupedDumpSitesListHistoricDataDTO { GeomAreasOutsideOfIgnoreZone = geomAreasOutsideOfIgnoreZone };

            // Act & Assert
            Assert.Equal(geomAreasOutsideOfIgnoreZone, dto.GeomAreasOutsideOfIgnoreZone);
        }

        [Fact]
        public void TotalGroupAreaInIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var totalGroupAreaInIgnoreZone = 25.5;
            var dto = new GroupedDumpSitesListHistoricDataDTO { TotalGroupAreaInIgnoreZone = totalGroupAreaInIgnoreZone };

            // Act & Assert
            Assert.Equal(totalGroupAreaInIgnoreZone, dto.TotalGroupAreaInIgnoreZone);
        }

        [Fact]
        public void TotalGroupAreaOutOfIgnoreZone_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var totalGroupAreaOutOfIgnoreZone = 35.5;
            var dto = new GroupedDumpSitesListHistoricDataDTO { TotalGroupAreaOutOfIgnoreZone = totalGroupAreaOutOfIgnoreZone };

            // Act & Assert
            Assert.Equal(totalGroupAreaOutOfIgnoreZone, dto.TotalGroupAreaOutOfIgnoreZone);
        }
    }
}
