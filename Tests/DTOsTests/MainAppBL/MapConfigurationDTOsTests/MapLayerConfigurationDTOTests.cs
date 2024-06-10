using DTOs.MainApp.BL.MapConfigurationDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.MapConfigurationDTOsTests
{
    public class MapLayerConfigurationDTOTests
    {
        [Fact]
        public void MapLayerConfigurationDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var layerName = "Test Layer";
            var layerTitleJson = "{\"title\": \"Test Layer Title\"}";
            var layerDescriptionJson = "{\"description\": \"Test Layer Description\"}";
            var order = 1;
            var layerJs = "LayerJs";
            var mapConfigurationId = Guid.NewGuid();
            var mapConfiguration = new MapConfigurationDTO();
            var mapLayerGroupConfigurationId = Guid.NewGuid();
            var mapLayerGroupConfiguration = new MapLayerGroupConfigurationDTO();
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();
            var updatedById = "user456";
            var updatedOn = DateTime.UtcNow;
            var updatedBy = new UserDTO();

            // Act
            var dto = new MapLayerConfigurationDTO
            {
                Id = id,
                LayerName = layerName,
                LayerTitleJson = layerTitleJson,
                LayerDescriptionJson = layerDescriptionJson,
                Order = order,
                LayerJs = layerJs,
                MapConfigurationId = mapConfigurationId,
                MapConfiguration = mapConfiguration,
                MapLayerGroupConfigurationId = mapLayerGroupConfigurationId,
                MapLayerGroupConfiguration = mapLayerGroupConfiguration,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(layerName, dto.LayerName);
            Assert.Equal(layerTitleJson, dto.LayerTitleJson);
            Assert.Equal(layerDescriptionJson, dto.LayerDescriptionJson);
            Assert.Equal(order, dto.Order);
            Assert.Equal(layerJs, dto.LayerJs);
            Assert.Equal(mapConfigurationId, dto.MapConfigurationId);
            Assert.Equal(mapConfiguration, dto.MapConfiguration);
            Assert.Equal(mapLayerGroupConfigurationId, dto.MapLayerGroupConfigurationId);
            Assert.Equal(mapLayerGroupConfiguration, dto.MapLayerGroupConfiguration);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
        }

        [Fact]
        public void MapLayerConfigurationDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new MapLayerConfigurationDTO();

            // Assert
            Assert.Equal(Guid.Empty, dto.Id);
            Assert.Null(dto.LayerName);
            Assert.Null(dto.LayerTitleJson);
            Assert.Null(dto.LayerDescriptionJson);
            Assert.Equal(0, dto.Order);
            Assert.Null(dto.LayerJs);
            Assert.Null(dto.MapConfigurationId);
            Assert.Null(dto.MapConfiguration);
            Assert.Null(dto.MapLayerGroupConfigurationId);
            Assert.Null(dto.MapLayerGroupConfiguration);
            Assert.Null(dto.CreatedById);
            Assert.NotEqual(DateTime.MinValue, dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.UpdatedBy);
        }

        [Fact]
        public void MapLayerConfigurationDTO_ShouldBeRecordType()
        {
            // Arrange & Act
            var dto = new MapLayerConfigurationDTO();

            // Assert
            Assert.IsType<MapLayerConfigurationDTO>(dto);
        }
    }
}
