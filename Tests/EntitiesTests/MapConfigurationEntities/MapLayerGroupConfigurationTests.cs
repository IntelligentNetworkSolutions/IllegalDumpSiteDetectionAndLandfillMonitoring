using Entities.MapConfigurationEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.MapConfigurationEntities
{
    public class MapLayerGroupConfigurationTests
    {
        [Fact]
        public void MapLayerGroupConfiguration_ShouldInitialize_WithDefaultValues()
        {
            var groupConfig = new MapLayerGroupConfiguration();

            Assert.Equal(DateTime.UtcNow.Date, groupConfig.CreatedOn.Date);
            Assert.Null(groupConfig.UpdatedOn);
            Assert.Null(groupConfig.CreatedBy);
            Assert.Null(groupConfig.UpdatedBy);
            Assert.Null(groupConfig.MapConfiguration);
            Assert.Null(groupConfig.MapLayerConfigurations);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetGroupName()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var groupName = "Test Group";

            groupConfig.GroupName = groupName;

            Assert.Equal(groupName, groupConfig.GroupName);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetLayerGroupTitleJson()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var layerGroupTitleJson = "{\"en\": \"Test Group Title\"}";

            groupConfig.LayerGroupTitleJson = layerGroupTitleJson;

            Assert.Equal(layerGroupTitleJson, groupConfig.LayerGroupTitleJson);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetLayerGroupDescriptionJson()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var layerGroupDescriptionJson = "{\"en\": \"Test Group Description\"}";

            groupConfig.LayerGroupDescriptionJson = layerGroupDescriptionJson;

            Assert.Equal(layerGroupDescriptionJson, groupConfig.LayerGroupDescriptionJson);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetOrder()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var order = 1;

            groupConfig.Order = order;

            Assert.Equal(order, groupConfig.Order);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetOpacity()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var opacity = 0.75;

            groupConfig.Opacity = opacity;

            Assert.Equal(opacity, groupConfig.Opacity);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetGroupJs()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var groupJs = "groupJsScript";

            groupConfig.GroupJs = groupJs;

            Assert.Equal(groupJs, groupConfig.GroupJs);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetMapConfigurationId()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var mapConfigId = Guid.NewGuid();

            groupConfig.MapConfigurationId = mapConfigId;

            Assert.Equal(mapConfigId, groupConfig.MapConfigurationId);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetCreatedById()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var createdById = "user123";

            groupConfig.CreatedById = createdById;

            Assert.Equal(createdById, groupConfig.CreatedById);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetUpdatedById()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var updatedById = "user456";

            groupConfig.UpdatedById = updatedById;

            Assert.Equal(updatedById, groupConfig.UpdatedById);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetCreatedOn()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var createdOn = DateTime.Now;

            groupConfig.CreatedOn = createdOn;

            Assert.Equal(createdOn, groupConfig.CreatedOn);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetUpdatedOn()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var updatedOn = DateTime.Now;

            groupConfig.UpdatedOn = updatedOn;

            Assert.Equal(updatedOn, groupConfig.UpdatedOn);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetCreatedBy()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            groupConfig.CreatedBy = createdBy;

            Assert.Equal(createdBy, groupConfig.CreatedBy);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetUpdatedBy()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var updatedBy = new ApplicationUser { Id = "user456", UserName = "updateduser" };

            groupConfig.UpdatedBy = updatedBy;

            Assert.Equal(updatedBy, groupConfig.UpdatedBy);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetMapConfiguration()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var mapConfig = new MapConfiguration { Id = Guid.NewGuid(), MapName = "Test Map" };

            groupConfig.MapConfiguration = mapConfig;

            Assert.Equal(mapConfig, groupConfig.MapConfiguration);
        }

        [Fact]
        public void MapLayerGroupConfiguration_ShouldSetAndGetMapLayerConfigurations()
        {
            var groupConfig = new MapLayerGroupConfiguration();
            var mapLayerConfigurations = new List<MapLayerConfiguration>
            {
                new MapLayerConfiguration { Id = Guid.NewGuid(), LayerName = "Layer1" },
                new MapLayerConfiguration { Id = Guid.NewGuid(), LayerName = "Layer2" }
            };

            groupConfig.MapLayerConfigurations = mapLayerConfigurations;

            Assert.Equal(mapLayerConfigurations, groupConfig.MapLayerConfigurations);
        }
    }
}
