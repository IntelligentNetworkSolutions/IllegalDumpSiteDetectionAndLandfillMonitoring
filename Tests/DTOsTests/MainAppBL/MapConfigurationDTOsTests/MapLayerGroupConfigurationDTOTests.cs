using DTOs.MainApp.BL.MapConfigurationDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.MapConfigurationDTOsTests
{
    public class MapLayerGroupConfigurationDTOTests
    {
        [Fact]
        public void MapLayerGroupConfigurationDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var groupName = "Test Group";
            var layerGroupTitleJson = "{\"title\": \"Test Group Title\"}";
            var layerGroupDescriptionJson = "{\"description\": \"Test Group Description\"}";
            var order = 1;
            var opacity = 0.5;
            var groupJs = "GroupJs";
            var mapConfigurationId = Guid.NewGuid();
            var mapConfiguration = new MapConfigurationDTO();
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();
            var updatedById = "user456";
            var updatedOn = DateTime.UtcNow;
            var updatedBy = new UserDTO();
            var mapLayerConfigurations = new List<MapLayerConfigurationDTO>();

            // Act
            var dto = new MapLayerGroupConfigurationDTO
            {
                Id = id,
                GroupName = groupName,
                LayerGroupTitleJson = layerGroupTitleJson,
                LayerGroupDescriptionJson = layerGroupDescriptionJson,
                Order = order,
                Opacity = opacity,
                GroupJs = groupJs,
                MapConfigurationId = mapConfigurationId,
                MapConfiguration = mapConfiguration,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy,
                MapLayerConfigurations = mapLayerConfigurations
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(groupName, dto.GroupName);
            Assert.Equal(layerGroupTitleJson, dto.LayerGroupTitleJson);
            Assert.Equal(layerGroupDescriptionJson, dto.LayerGroupDescriptionJson);
            Assert.Equal(order, dto.Order);
            Assert.Equal(opacity, dto.Opacity);
            Assert.Equal(groupJs, dto.GroupJs);
            Assert.Equal(mapConfigurationId, dto.MapConfigurationId);
            Assert.Equal(mapConfiguration, dto.MapConfiguration);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
            Assert.Equal(mapLayerConfigurations, dto.MapLayerConfigurations);
        }               

        [Fact]
        public void MapLayerGroupConfigurationDTO_ShouldBeRecordType()
        {
            // Arrange & Act
            var dto = new MapLayerGroupConfigurationDTO();

            // Assert
            Assert.IsType<MapLayerGroupConfigurationDTO>(dto);
        }
    }
}
