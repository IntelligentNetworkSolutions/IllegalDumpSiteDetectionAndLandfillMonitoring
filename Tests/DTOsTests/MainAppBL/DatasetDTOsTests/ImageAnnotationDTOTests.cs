using DTOs.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using NetTopologySuite.Geometries;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class ImageAnnotationDTOTests
    {
        [Fact]
        public void ImageAnnotationDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var annotationJson = "{\"type\": \"Feature\", \"geometry\": {\"type\": \"Polygon\", \"coordinates\": [[[0, 0], [1, 0], [1, 1], [0, 1], [0, 0]]]} }";
            var geom = new Polygon(new LinearRing(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            }));
            var isEnabled = true;
            var datasetImageId = Guid.NewGuid();
            var datasetImage = new DatasetImageDTO();
            var datasetClassId = Guid.NewGuid();
            var datasetClass = new DatasetClassDTO();
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var updatedById = "user456";
            var updatedOn = DateTime.UtcNow.AddHours(1);
            var createdBy = new UserDTO();
            var updatedBy = new UserDTO();

            // Act
            var dto = new ImageAnnotationDTO
            {
                Id = id,
                AnnotationJson = annotationJson,
                Geom = geom,
                IsEnabled = isEnabled,
                DatasetImageId = datasetImageId,
                DatasetImage = datasetImage,
                DatasetClassId = datasetClassId,
                DatasetClass = datasetClass,
                CreatedById = createdById,
                CreatedOn = createdOn,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(annotationJson, dto.AnnotationJson);
            Assert.Equal(geom, dto.Geom);
            Assert.Equal(isEnabled, dto.IsEnabled);
            Assert.Equal(datasetImageId, dto.DatasetImageId);
            Assert.Equal(datasetImage, dto.DatasetImage);
            Assert.Equal(datasetClassId, dto.DatasetClassId);
            Assert.Equal(datasetClass, dto.DatasetClass);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedBy, dto.UpdatedBy);
        }

        [Fact]
        public void ImageAnnotationDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new ImageAnnotationDTO();

            // Assert
            Assert.Null(dto.Id);
            Assert.Null(dto.AnnotationJson);
            Assert.Null(dto.Geom);
            Assert.False(dto.IsEnabled);
            Assert.Null(dto.DatasetImageId);
            Assert.Null(dto.DatasetImage);
            Assert.Null(dto.DatasetClassId);
            Assert.Null(dto.DatasetClass);
            Assert.Null(dto.CreatedById);
            Assert.Equal(DateTime.UtcNow, dto.CreatedOn, TimeSpan.FromSeconds(1));
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedBy);
        }

        [Fact]
        public void ImageAnnotationDTO_GeoJson_ShouldReturnExpectedGeoJson()
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
            var dto = new ImageAnnotationDTO { Geom = geom };
            var actualGeoJson = dto.GeoJson;

            // Assert
            Assert.Equal(expectedGeoJson, actualGeoJson);
        }

        //[Fact]
        //public void ImageAnnotationDTO_TopLeftBottomRight_ShouldReturnExpectedDictionary()
        //{
        //    // Arrange
        //    var geom = new Polygon(new LinearRing(new[]
        //    {
        //        new Coordinate(0, 0),
        //        new Coordinate(1, 0),
        //        new Coordinate(1, 1),
        //        new Coordinate(0, 1),
        //        new Coordinate(0, 0)
        //    }));
        //    var expectedDictionary = GeoJsonHelpers.GeometryBBoxToTopLeftBottomRight(geom);

        //    // Act
        //    var dto = new ImageAnnotationDTO { Geom = geom };
        //    var actualDictionary = dto.TopLeftBottomRight;

        //    // Assert
        //    Assert.Equal(expectedDictionary, actualDictionary);
        //}

        //[Fact]
        //public void ImageAnnotationDTO_TopLeftWidthHeight_ShouldReturnExpectedDictionary()
        //{
        //    // Arrange
        //    var geom = new Polygon(new LinearRing(new[]
        //    {
        //        new Coordinate(0, 0),
        //        new Coordinate(1, 0),
        //        new Coordinate(1, 1),
        //        new Coordinate(0, 1),
        //        new Coordinate(0, 0)
        //    }));
        //    var expectedDictionary = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeight(geom);

        //    // Act
        //    var dto = new ImageAnnotationDTO { Geom = geom };
        //    var actualDictionary = dto.TopLeftWidthHeight;

        //    // Assert
        //    Assert.Equal(expectedDictionary, actualDictionary);
        //}

    }
}
