using Entities.DetectionEntities;
using Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.DetectionEntities
{
    public class DetectionIgnoreZoneTests
    {
        [Fact]
        public void DetectionIgnoreZone_ShouldInitialize_WithDefaultValues()
        {
            var ignoreZone = new DetectionIgnoreZone();

            Assert.Equal(DateTime.UtcNow.Date, ignoreZone.CreatedOn.Date); 
            Assert.Null(ignoreZone.Description);
            Assert.Null(ignoreZone.CreatedBy);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetName()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var name = "Test Ignore Zone";

            ignoreZone.Name = name;

            Assert.Equal(name, ignoreZone.Name);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetDescription()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var description = "This is a test description";

            ignoreZone.Description = description;

            Assert.Equal(description, ignoreZone.Description);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetGeom()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var coordinates = new Coordinate[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            };
            var geom = new Polygon(new LinearRing(coordinates));

            ignoreZone.Geom = geom;

            Assert.Equal(geom, ignoreZone.Geom);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetGeoJson()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var geoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[0,0],[0,1],[1,1],[1,0],[0,0]]]}";

            ignoreZone.GeoJson = geoJson;

            Assert.Equal(geoJson, ignoreZone.GeoJson);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetCreatedById()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var createdById = "user123";

            ignoreZone.CreatedById = createdById;

            Assert.Equal(createdById, ignoreZone.CreatedById);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetCreatedOn()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var createdOn = DateTime.Now;

            ignoreZone.CreatedOn = createdOn;

            Assert.Equal(createdOn, ignoreZone.CreatedOn);
        }

        [Fact]
        public void DetectionIgnoreZone_ShouldSetAndGetCreatedBy()
        {
            var ignoreZone = new DetectionIgnoreZone();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            ignoreZone.CreatedBy = createdBy;

            Assert.Equal(createdBy, ignoreZone.CreatedBy);
        }
               
        [Fact]
        public void DetectionIgnoreZone_ShouldNotMapGeoJsonProperty()
        {
            var ignoreZone = new DetectionIgnoreZone();
            ignoreZone.GeoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[0,0],[0,1],[1,1],[1,0],[0,0]]]}";

            var geoJsonProperty = ignoreZone.GetType().GetProperty("GeoJson");
            var notMappedAttribute = Attribute.IsDefined(geoJsonProperty, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute));

            Assert.True(notMappedAttribute);
        }
    }
}
