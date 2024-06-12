using Entities.MapConfigurationEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.MapConfigurationEntities
{
    public class MapConfigurationTests
    {
        [Fact]
        public void MapConfiguration_ShouldInitialize_WithDefaultValues()
        {
            var mapConfig = new MapConfiguration();

            Assert.Equal(DateTime.UtcNow.Date, mapConfig.CreatedOn.Date); 
            Assert.Null(mapConfig.UpdatedOn);
            Assert.Null(mapConfig.CreatedBy);
            Assert.Null(mapConfig.UpdatedBy);
            Assert.Null(mapConfig.MapLayerConfigurations);
            Assert.Null(mapConfig.MapLayerGroupConfigurations);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetMapName()
        {
            var mapConfig = new MapConfiguration();
            var mapName = "Test Map";

            mapConfig.MapName = mapName;

            Assert.Equal(mapName, mapConfig.MapName);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetProjection()
        {
            var mapConfig = new MapConfiguration();
            var projection = "EPSG:4326";

            mapConfig.Projection = projection;

            Assert.Equal(projection, mapConfig.Projection);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetTileGridJs()
        {
            var mapConfig = new MapConfiguration();
            var tileGridJs = "tileGrid";

            mapConfig.TileGridJs = tileGridJs;

            Assert.Equal(tileGridJs, mapConfig.TileGridJs);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetCenterCoordinates()
        {
            var mapConfig = new MapConfiguration();
            var centerX = 12.34;
            var centerY = 56.78;

            mapConfig.CenterX = centerX;
            mapConfig.CenterY = centerY;

            Assert.Equal(centerX, mapConfig.CenterX);
            Assert.Equal(centerY, mapConfig.CenterY);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetBoundingBox()
        {
            var mapConfig = new MapConfiguration();
            var minX = 0.0;
            var minY = 0.0;
            var maxX = 100.0;
            var maxY = 100.0;

            mapConfig.MinX = minX;
            mapConfig.MinY = minY;
            mapConfig.MaxX = maxX;
            mapConfig.MaxY = maxY;

            Assert.Equal(minX, mapConfig.MinX);
            Assert.Equal(minY, mapConfig.MinY);
            Assert.Equal(maxX, mapConfig.MaxX);
            Assert.Equal(maxY, mapConfig.MaxY);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetResolutions()
        {
            var mapConfig = new MapConfiguration();
            var resolutions = "0.5,1,2,4,8";

            mapConfig.Resolutions = resolutions;

            Assert.Equal(resolutions, mapConfig.Resolutions);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetDefaultResolution()
        {
            var mapConfig = new MapConfiguration();
            var defaultResolution = 1.0;

            mapConfig.DefaultResolution = defaultResolution;

            Assert.Equal(defaultResolution, mapConfig.DefaultResolution);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetCreatedById()
        {
            var mapConfig = new MapConfiguration();
            var createdById = "user123";

            mapConfig.CreatedById = createdById;

            Assert.Equal(createdById, mapConfig.CreatedById);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetUpdatedById()
        {
            var mapConfig = new MapConfiguration();
            var updatedById = "user456";

            mapConfig.UpdatedById = updatedById;

            Assert.Equal(updatedById, mapConfig.UpdatedById);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetCreatedOn()
        {
            var mapConfig = new MapConfiguration();
            var createdOn = DateTime.Now;

            mapConfig.CreatedOn = createdOn;

            Assert.Equal(createdOn, mapConfig.CreatedOn);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetUpdatedOn()
        {
            var mapConfig = new MapConfiguration();
            var updatedOn = DateTime.Now;

            mapConfig.UpdatedOn = updatedOn;

            Assert.Equal(updatedOn, mapConfig.UpdatedOn);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetCreatedBy()
        {
            var mapConfig = new MapConfiguration();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            mapConfig.CreatedBy = createdBy;

            Assert.Equal(createdBy, mapConfig.CreatedBy);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetUpdatedBy()
        {
            var mapConfig = new MapConfiguration();
            var updatedBy = new ApplicationUser { Id = "user456", UserName = "updateduser" };

            mapConfig.UpdatedBy = updatedBy;

            Assert.Equal(updatedBy, mapConfig.UpdatedBy);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetMapLayerConfigurations()
        {
            var mapConfig = new MapConfiguration();
            var mapLayerConfigurations = new List<MapLayerConfiguration>
            {
                new MapLayerConfiguration { Id = Guid.NewGuid(), LayerName = "Layer1" },
                new MapLayerConfiguration { Id = Guid.NewGuid(), LayerName = "Layer2" }
            };

            mapConfig.MapLayerConfigurations = mapLayerConfigurations;

            Assert.Equal(mapLayerConfigurations, mapConfig.MapLayerConfigurations);
        }

        [Fact]
        public void MapConfiguration_ShouldSetAndGetMapLayerGroupConfigurations()
        {
            var mapConfig = new MapConfiguration();
            var mapLayerGroupConfigurations = new List<MapLayerGroupConfiguration>
            {
                new MapLayerGroupConfiguration { Id = Guid.NewGuid(), GroupName = "Group1" },
                new MapLayerGroupConfiguration { Id = Guid.NewGuid(), GroupName = "Group2" }
            };

            mapConfig.MapLayerGroupConfigurations = mapLayerGroupConfigurations;

            Assert.Equal(mapLayerGroupConfigurations, mapConfig.MapLayerGroupConfigurations);
        }
    }
}
