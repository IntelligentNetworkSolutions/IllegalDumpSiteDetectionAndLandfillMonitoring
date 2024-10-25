using Entities.Helpers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace Tests.EntitiesTests.Helpers
{
    public class GeoJsonHelpersTests
    {
        [Fact]
        public void GeometryToGeoJson_Point_ShouldReturnValidGeoJson()
        {
            // Arrange
            Point point = new Point(1, 2);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(point);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("Point", json["type"]);
            Assert.Equal(1, json["coordinates"][0]);
            Assert.Equal(2, json["coordinates"][1]);
        }

        [Fact]
        public void GeometryToGeoJson_LineString_ShouldReturnValidGeoJson()
        {
            // Arrange
            LineString lineString = new LineString([new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(3, 3)]);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(lineString);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("LineString", json["type"]);
            Assert.Equal(3, json["coordinates"].Count());
            Assert.Equal(1, json["coordinates"][0][0]);
            Assert.Equal(3, json["coordinates"][2][1]);
        }

        [Fact]
        public void GeometryToGeoJson_Polygon_ShouldReturnValidGeoJson()
        {
            // Arrange
            Polygon polygon =
                new Polygon(
                    new LinearRing([new Coordinate(0, 0), new Coordinate(0, 1), new Coordinate(1, 1), new Coordinate(1, 0), new Coordinate(0, 0)]));

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(polygon);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("Polygon", json["type"]);
            Assert.Equal(1, json["coordinates"].Count());
            Assert.Equal(5, json["coordinates"][0].Count());
            Assert.Equal(0, json["coordinates"][0][0][0]);
            Assert.Equal(1, json["coordinates"][0][2][1]);
        }

        [Fact]
        public void GeometryToGeoJson_MultiPoint_ShouldReturnValidGeoJson()
        {
            // Arrange
            MultiPoint multiPoint = new MultiPoint([new Point(1, 1), new Point(2, 2)]);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(multiPoint);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("MultiPoint", json["type"]);
            Assert.Equal(2, json["coordinates"].Count());
            Assert.Equal(1, json["coordinates"][0][0]);
            Assert.Equal(2, json["coordinates"][1][1]);
        }

        [Fact]
        public void GeometryToGeoJson_EmptyGeometry_ShouldReturnEmptyGeoJson()
        {
            // Arrange
            Geometry emptyGeometry = new GeometryFactory().CreateEmpty(Dimension.Unknown);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(emptyGeometry);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("GeometryCollection", json["type"]);
            Assert.Empty(json["geometries"]);
        }

        [Fact]
        public void GeometryToGeoJson_NullGeometry_ShouldThrowArgumentNullException()
        {
            // Arrange
            Geometry nullGeometry = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GeoJsonHelpers.GeometryToGeoJson(nullGeometry));
        }

        [Fact]
        public void GeometryToGeoJson_GeometryCollection_ShouldReturnValidGeoJson()
        {
            // Arrange
            GeometryCollection geometryCollection =
                new GeometryCollection([new Point(1, 1), new LineString([new Coordinate(2, 2), new Coordinate(3, 3)])]);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(geometryCollection);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("GeometryCollection", json["type"]);
            Assert.Equal(2, json["geometries"].Count());
            Assert.Equal("Point", json["geometries"][0]["type"]);
            Assert.Equal("LineString", json["geometries"][1]["type"]);
        }
    }
}
