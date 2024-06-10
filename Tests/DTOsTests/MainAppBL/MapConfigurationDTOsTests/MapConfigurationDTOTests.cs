using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.MapConfigurationDTOs;

namespace Tests.DTOsTests.MainAppBL.MapConfigurationDTOsTests
{
    public class MapConfigurationDTOTests
    {
        [Fact]
        public void MapConfigurationDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mapName = "Test Map";
            var projection = "EPSG:4326";
            var tileGridJs = "TileGridJs";
            var centerX = 1.0;
            var centerY = 1.0;
            var minX = 0.0;
            var minY = 0.0;
            var maxX = 10.0;
            var maxY = 10.0;
            var resolutions = "0.5, 1.0, 2.0";
            var defaultResolution = 1.0;
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO();
            var updatedById = "user456";
            var updatedOn = DateTime.UtcNow;
            var updatedBy = new UserDTO();
            var mapLayerConfigurations = new List<MapLayerConfigurationDTO>();
            var mapLayerGroupConfigurations = new List<MapLayerGroupConfigurationDTO>();

            // Act
            var dto = new MapConfigurationDTO
            {
                Id = id,
                MapName = mapName,
                Projection = projection,
                TileGridJs = tileGridJs,
                CenterX = centerX,
                CenterY = centerY,
                MinX = minX,
                MinY = minY,
                MaxX = maxX,
                MaxY = maxY,
                Resolutions = resolutions,
                DefaultResolution = defaultResolution,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                UpdatedBy = updatedBy,
                MapLayerConfigurations = mapLayerConfigurations,
                MapLayerGroupConfigurations = mapLayerGroupConfigurations
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(mapName, dto.MapName);
            Assert.Equal(projection, dto.Projection);
            Assert.Equal(tileGridJs, dto.TileGridJs);
            Assert.Equal(centerX, dto.CenterX);
            Assert.Equal(centerY, dto.CenterY);
            Assert.Equal(minX, dto.MinX);
            Assert.Equal(minY, dto.MinY);
            Assert.Equal(maxX, dto.MaxX);
            Assert.Equal(maxY, dto.MaxY);
            Assert.Equal(resolutions, dto.Resolutions);
            Assert.Equal(defaultResolution, dto.DefaultResolution);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(updatedById, dto.UpdatedById);
            Assert.Equal(updatedOn, dto.UpdatedOn);
            Assert.Equal(updatedBy, dto.UpdatedBy);
            Assert.Equal(mapLayerConfigurations, dto.MapLayerConfigurations);
            Assert.Equal(mapLayerGroupConfigurations, dto.MapLayerGroupConfigurations);
        }

        [Fact]
        public void MapConfigurationDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new MapConfigurationDTO();

            // Assert
            Assert.Equal(Guid.Empty, dto.Id);
            Assert.Null(dto.MapName);
            Assert.Null(dto.Projection);
            Assert.Null(dto.TileGridJs);
            Assert.Equal(0.0, dto.CenterX);
            Assert.Equal(0.0, dto.CenterY);
            Assert.Equal(0.0, dto.MinX);
            Assert.Equal(0.0, dto.MinY);
            Assert.Equal(0.0, dto.MaxX);
            Assert.Equal(0.0, dto.MaxY);
            Assert.Null(dto.Resolutions);
            Assert.Equal(0.0, dto.DefaultResolution);
            Assert.Null(dto.CreatedById);
            Assert.NotEqual(DateTime.MinValue, dto.CreatedOn);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.UpdatedById);
            Assert.Null(dto.UpdatedOn);
            Assert.Null(dto.UpdatedBy);
            Assert.Null(dto.MapLayerConfigurations);
            Assert.Null(dto.MapLayerGroupConfigurations);
        }

        [Fact]
        public void MapConfigurationDTO_ShouldBeRecordType()
        {
            // Arrange & Act
            var dto = new MapConfigurationDTO();

            // Assert
            Assert.IsType<MapConfigurationDTO>(dto);
        }
    }
}
