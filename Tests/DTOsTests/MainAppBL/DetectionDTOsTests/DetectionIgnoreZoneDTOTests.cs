using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Helpers;

namespace Tests.DTOsTests.MainAppBL.DetectionDTOsTests
{
    public class DetectionIgnoreZoneDTOTests
    {
        [Fact]
        public void DetectionIgnoreZoneDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Zone";
            var description = "Test Zone Description";
            var geom = new Polygon(new LinearRing(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            }));
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();

            // Act
            var dto = new DetectionIgnoreZoneDTO
            {
                Id = id,
                Name = name,
                Description = description,
                Geom = geom,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(geom, dto.Geom);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
        }

        [Fact]
        public void DetectionIgnoreZoneDTO_GeoJson_ShouldReturnExpectedGeoJson()
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
            var dto = new DetectionIgnoreZoneDTO { Geom = geom };
            var actualGeoJson = dto.GeoJson;

            // Assert
            Assert.Equal(expectedGeoJson, actualGeoJson);
        }
    }
}
