using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Helpers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests.DTOsTests.Helpers
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
            LineString lineString = new LineString([ new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(3, 3) ]);

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
                    new LinearRing([ new Coordinate(0, 0), new Coordinate(0, 1), new Coordinate(1, 1), new Coordinate(1, 0), new Coordinate(0, 0) ]));

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
            MultiPoint multiPoint = new MultiPoint([ new Point(1, 1), new Point(2, 2) ]);

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
                new GeometryCollection([ new Point(1, 1), new LineString([new Coordinate(2, 2), new Coordinate(3, 3)]) ]);

            // Act
            string result = GeoJsonHelpers.GeometryToGeoJson(geometryCollection);

            // Assert
            JObject json = JObject.Parse(result);
            Assert.Equal("GeometryCollection", json["type"]);
            Assert.Equal(2, json["geometries"].Count());
            Assert.Equal("Point", json["geometries"][0]["type"]);
            Assert.Equal("LineString", json["geometries"][1]["type"]);
        }

        [Fact]
        public void GeoJsonFeatureToGeometry_ValidPointFeature_ShouldReturnPoint()
        {
            // Arrange
            string geoJson = @"{
            ""type"": ""Feature"",
            ""geometry"": {
                ""type"": ""Point"",
                ""coordinates"": [100.0, 0.0]
            }
        }";

            // Act
            var result = GeoJsonHelpers.GeoJsonFeatureToGeometry(geoJson);

            // Assert
            Assert.IsType<Point>(result);
            var point = (Point)result;
            Assert.Equal(100.0, point.X);
            Assert.Equal(0.0, point.Y);
        }

        [Fact]
        public void GeoJsonFeatureToGeometry_InvalidGeoJson_ShouldThrowException()
        {
            // Arrange
            string invalidGeoJson = "{ invalid json }";

            // Act & Assert
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => GeoJsonHelpers.GeoJsonFeatureToGeometry(invalidGeoJson));
        }

        [Fact]
        public void ConvertBoundingBoxToPolygon_ValidBBox_ShouldReturnPolygon()
        {
            // Arrange
            List<float> bbox = new List<float> { 0, 0, 100, 100 };

            // Act
            var result = GeoJsonHelpers.ConvertBoundingBoxToPolygon(bbox);

            // Assert
            Assert.IsType<Polygon>(result);
            Assert.Equal(5, result.Coordinates.Length); // 4 corners + closing point
            Assert.Equal(new Coordinate(0, 0), result.Coordinates[0]);
            Assert.Equal(new Coordinate(100, 100), result.Coordinates[2]);
        }

        [Fact]
        public void ConvertBoundingBoxToPolygon_InvalidBBox_ShouldThrowArgumentException()
        {
            // Arrange
            List<float> invalidBBox = new List<float> { 0, 0, 100 }; // Missing one element

            // Act & Assert
            Assert.Throws<ArgumentException>(() => GeoJsonHelpers.ConvertBoundingBoxToPolygon(invalidBBox));
        }

        [Fact]
        public void GeometryListToGeoJson_ValidPolygonList_ShouldReturnGeoJsonList()
        {
            // Arrange
            var polygon1 = new Polygon(new LinearRing(new Coordinate[]
            {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(1, 0),
            new Coordinate(0, 0)
            }));
            var polygon2 = new Polygon(new LinearRing(new Coordinate[]
            {
            new Coordinate(1, 1),
            new Coordinate(1, 2),
            new Coordinate(2, 2),
            new Coordinate(2, 1),
            new Coordinate(1, 1)
            }));
            var polygonList = new List<Polygon> { polygon1, polygon2 };

            // Act
            var result = GeoJsonHelpers.GeometryListToGeoJson(polygonList);

            // Assert
            Assert.Equal(2, result.Count);
            foreach (var geoJson in result)
            {
                var json = JObject.Parse(geoJson);
                Assert.Equal("Polygon", json["type"]);
            }
        }

        [Fact]
        public void GeometryBBoxToTopLeftBottomRight_ValidGeometry_ShouldReturnDictionary()
        {
            // Arrange
            var polygon = new Polygon(new LinearRing(new Coordinate[]
            {
            new Coordinate(0, 0),
            new Coordinate(0, 100),
            new Coordinate(100, 100),
            new Coordinate(100, 0),
            new Coordinate(0, 0)
            }));

            // Act
            var result = GeoJsonHelpers.GeometryBBoxToTopLeftBottomRight(polygon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(0, result[0]); // top
            Assert.Equal(0, result[0]); // left
            Assert.Equal(100, result[100]); // bottom
            Assert.Equal(100, result[100]); // right
        }

        [Fact]
        public void GeometryBBoxToTopLeftBottomRight_NullGeometry_ShouldReturnNull()
        {
            // Act
            var result = GeoJsonHelpers.GeometryBBoxToTopLeftBottomRight(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GeometryBBoxToTopLeftWidthHeight_ValidGeometry_ShouldReturnDictionary()
        {
            // Arrange
            Polygon polygon = 
                new Polygon(
                    new LinearRing([ new Coordinate(0, 0), new Coordinate(0, 100), 
                                        new Coordinate(100, 100), new Coordinate(100, 0), 
                                            new Coordinate(0, 0) ]));

            // Act
            Dictionary<int, int>? result = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeight(polygon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(100, result.ElementAt(0).Value); // top (MaxY)
            Assert.Equal(0, result.ElementAt(0).Key); // left (MinX)
            Assert.Equal(100, result.ElementAt(1).Key); // width
            Assert.Equal(100, result.ElementAt(1).Value); // height
        }

        [Fact]
        public void GeometryBBoxToTopLeftWidthHeight_NullGeometry_ShouldReturnNull()
        {
            // Act
            Dictionary<int, int>? result = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeight(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GeometryBBoxToTopLeftWidthHeightList_ValidGeometry_ShouldReturnList()
        {
            // Arrange
            Polygon polygon = 
                new Polygon(
                    new LinearRing([new Coordinate(0, 0), new Coordinate(0, 100),
                                    new Coordinate(100, 100), new Coordinate(100, 0),
                                    new Coordinate(0, 0)]));

            // Act
            List<float>? result = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(polygon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal(0f, result[0]); // left (MinX)
            Assert.Equal(0f, result[1]); // top (MinY)
            Assert.Equal(100f, result[2]); // width
            Assert.Equal(100f, result[3]); // height
        }

        [Fact]
        public void GeometryBBoxToTopLeftWidthHeightList_NullGeometry_ShouldReturnNull()
        {
            // Act
            List<float>? result = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GeoJsonFeatureToGeometry_LineStringFeature_ShouldReturnLineString()
        {
            string geoJson = @"{
            ""type"": ""Feature"",
            ""geometry"": {
                ""type"": ""LineString"",
                ""coordinates"": [[100.0, 0.0], [101.0, 1.0]]
            }
        }";

            Geometry result = GeoJsonHelpers.GeoJsonFeatureToGeometry(geoJson);

            Assert.IsType<LineString>(result);
            LineString lineString = (LineString)result;
            Assert.Equal(2, lineString.Coordinates.Length);
        }

        [Fact]
        public void GeoJsonFeatureToGeometry_FeatureWithProperties_ShouldIgnoreProperties()
        {
            string geoJson = @"{
            ""type"": ""Feature"",
            ""properties"": {""name"": ""Test Feature""},
            ""geometry"": {
                ""type"": ""Point"",
                ""coordinates"": [100.0, 0.0]
            }
        }";

            Geometry result = GeoJsonHelpers.GeoJsonFeatureToGeometry(geoJson);

            Assert.IsType<Point>(result);
        }

        [Fact]
        public void GeoJsonFeatureToGeometry_NonFeatureGeoJson_ShouldThrowException()
        {
            string geoJson = @"{
            ""type"": ""Point"",
            ""coordinates"": [100.0, 0.0]
        }";

            Assert.Throws<JsonReaderException>(() => GeoJsonHelpers.GeoJsonFeatureToGeometry(geoJson));
        }

        [Fact]
        public void ConvertBoundingBoxToPolygon_NegativeValues_ShouldReturnValidPolygon()
        {
            List<float> bbox = [-10, -10, 20, 20];

            Polygon result = GeoJsonHelpers.ConvertBoundingBoxToPolygon(bbox);

            Assert.IsType<Polygon>(result);
            Assert.Equal(new Coordinate(-10, -10), result.Coordinates[0]);
            Assert.Equal(new Coordinate(10, 10), result.Coordinates[2]);
        }

        [Fact]
        public void ConvertBoundingBoxToPolygon_LargeValues_ShouldReturnValidPolygon()
        {
            List<float> bbox = [float.MinValue, float.MinValue, float.MaxValue, float.MaxValue];

            Polygon result = GeoJsonHelpers.ConvertBoundingBoxToPolygon(bbox);

            Assert.IsType<Polygon>(result);
            Assert.Equal(new Coordinate(float.MinValue, float.MinValue), result.Coordinates[0]);
        }

        [Fact]
        public void ConvertBoundingBoxToPolygon_NullInput_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => GeoJsonHelpers.ConvertBoundingBoxToPolygon(null));
        }

        [Fact]
        public void GeometryListToGeoJson_EmptyList_ShouldReturnEmptyList()
        {
            List<string> result = GeoJsonHelpers.GeometryListToGeoJson([]);

            Assert.Empty(result);
        }

        [Fact]
        public void GeometryListToGeoJson_MixedGeometries_ShouldReturnValidGeoJsonList()
        {
            List<Polygon> geometries =
                [new Polygon(new LinearRing([new(0, 0), new(0, 1), new(1, 1), new(1, 0), new(0, 0)])),
                    new Polygon(new LinearRing([new(0, 0), new(0, 2), new(2, 2), new(2, 0), new(0, 0)]))];

            List<string> result = GeoJsonHelpers.GeometryListToGeoJson(geometries);

            Assert.Equal(2, result.Count);
            foreach (var geoJson in result)
            {
                JObject json = JObject.Parse(geoJson);
                Assert.Equal("Polygon", json["type"]);
            }
        }

        [Fact]
        public void GeometryBBoxToTopLeftWidthHeightList_MultiPolygonGeometry_ShouldReturnValidList()
        {
            Polygon polygon1 = new Polygon(new LinearRing([new(0, 0), new(0, 1), new(1, 1), new(1, 0), new(0, 0)]));
            Polygon polygon2 = new Polygon(new LinearRing([new(2, 2), new(2, 3), new(3, 3), new(3, 2), new(2, 2)]));
            MultiPolygon multiPolygon = new MultiPolygon([polygon1, polygon2]);

            List<float>? result = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(multiPolygon);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal(0f, result[0]); // left
            Assert.Equal(0f, result[1]); // top
            Assert.Equal(3f, result[2]); // width
            Assert.Equal(3f, result[3]); // height
        }
    }
}
