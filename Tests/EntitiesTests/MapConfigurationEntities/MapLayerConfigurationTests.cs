using Entities.MapConfigurationEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.MapConfigurationEntities
{
    public class MapLayerConfigurationTests
    {
        [Fact]
        public void MapLayerConfiguration_ShouldInitialize_WithDefaultValues()
        {
            var layerConfig = new MapLayerConfiguration();

            Assert.Equal(DateTime.UtcNow.Date, layerConfig.CreatedOn.Date);
            Assert.Null(layerConfig.UpdatedOn);
            Assert.Null(layerConfig.CreatedBy);
            Assert.Null(layerConfig.UpdatedBy);
            Assert.Null(layerConfig.MapConfiguration);
            Assert.Null(layerConfig.MapLayerGroupConfiguration);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetLayerName()
        {
            var layerConfig = new MapLayerConfiguration();
            var layerName = "Test Layer";

            layerConfig.LayerName = layerName;

            Assert.Equal(layerName, layerConfig.LayerName);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetLayerTitleJson()
        {
            var layerConfig = new MapLayerConfiguration();
            var layerTitleJson = "{\"en\": \"Test Layer Title\"}";

            layerConfig.LayerTitleJson = layerTitleJson;

            Assert.Equal(layerTitleJson, layerConfig.LayerTitleJson);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetLayerDescriptionJson()
        {
            var layerConfig = new MapLayerConfiguration();
            var layerDescriptionJson = "{\"en\": \"Test Layer Description\"}";

            layerConfig.LayerDescriptionJson = layerDescriptionJson;

            Assert.Equal(layerDescriptionJson, layerConfig.LayerDescriptionJson);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetOrder()
        {
            var layerConfig = new MapLayerConfiguration();
            var order = 1;

            layerConfig.Order = order;

            Assert.Equal(order, layerConfig.Order);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetLayerJs()
        {
            var layerConfig = new MapLayerConfiguration();
            var layerJs = "layerJsScript";

            layerConfig.LayerJs = layerJs;

            Assert.Equal(layerJs, layerConfig.LayerJs);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetMapConfigurationId()
        {
            var layerConfig = new MapLayerConfiguration();
            var mapConfigId = Guid.NewGuid();

            layerConfig.MapConfigurationId = mapConfigId;

            Assert.Equal(mapConfigId, layerConfig.MapConfigurationId);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetMapLayerGroupConfigurationId()
        {
            var layerConfig = new MapLayerConfiguration();
            var groupConfigId = Guid.NewGuid();

            layerConfig.MapLayerGroupConfigurationId = groupConfigId;

            Assert.Equal(groupConfigId, layerConfig.MapLayerGroupConfigurationId);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetCreatedById()
        {
            var layerConfig = new MapLayerConfiguration();
            var createdById = "user123";

            layerConfig.CreatedById = createdById;

            Assert.Equal(createdById, layerConfig.CreatedById);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetUpdatedById()
        {
            var layerConfig = new MapLayerConfiguration();
            var updatedById = "user456";

            layerConfig.UpdatedById = updatedById;

            Assert.Equal(updatedById, layerConfig.UpdatedById);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetCreatedOn()
        {
            var layerConfig = new MapLayerConfiguration();
            var createdOn = DateTime.Now;

            layerConfig.CreatedOn = createdOn;

            Assert.Equal(createdOn, layerConfig.CreatedOn);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetUpdatedOn()
        {
            var layerConfig = new MapLayerConfiguration();
            var updatedOn = DateTime.Now;

            layerConfig.UpdatedOn = updatedOn;

            Assert.Equal(updatedOn, layerConfig.UpdatedOn);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetCreatedBy()
        {
            var layerConfig = new MapLayerConfiguration();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            layerConfig.CreatedBy = createdBy;

            Assert.Equal(createdBy, layerConfig.CreatedBy);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetUpdatedBy()
        {
            var layerConfig = new MapLayerConfiguration();
            var updatedBy = new ApplicationUser { Id = "user456", UserName = "updateduser" };

            layerConfig.UpdatedBy = updatedBy;

            Assert.Equal(updatedBy, layerConfig.UpdatedBy);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetMapConfiguration()
        {
            var layerConfig = new MapLayerConfiguration();
            var mapConfig = new MapConfiguration { Id = Guid.NewGuid(), MapName = "Test Map" };

            layerConfig.MapConfiguration = mapConfig;

            Assert.Equal(mapConfig, layerConfig.MapConfiguration);
        }

        [Fact]
        public void MapLayerConfiguration_ShouldSetAndGetMapLayerGroupConfiguration()
        {
            var layerConfig = new MapLayerConfiguration();
            var mapLayerGroupConfig = new MapLayerGroupConfiguration { Id = Guid.NewGuid(), GroupName = "Test Group" };

            layerConfig.MapLayerGroupConfiguration = mapLayerGroupConfig;

            Assert.Equal(mapLayerGroupConfig, layerConfig.MapLayerGroupConfiguration);
        }
    }
}
