using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class MapConfigurationProfileBLTests
    {
        private readonly IMapper _mapper;

        public MapConfigurationProfileBLTests()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MapConfigurationProfileBL>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_MapConfiguration_To_MapConfigurationDTO()
        {
            // Arrange
            var mapConfiguration = new MapConfiguration
            {
                Id = Guid.NewGuid(),
                MapName = "Test Map",
                Projection = "EPSG:4326",
                TileGridJs = "tileGrid.js",
                CenterX = 1.0,
                CenterY = 2.0,
                MinX = 0.0,
                MinY = 0.0,
                MaxX = 3.0,
                MaxY = 4.0,
                Resolutions = "1,2,3",
                DefaultResolution = 1.0,
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "user456",
                UpdatedOn = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var result = _mapper.Map<MapConfigurationDTO>(mapConfiguration);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapConfiguration.Id, result.Id);
            Assert.Equal(mapConfiguration.MapName, result.MapName);
            Assert.Equal(mapConfiguration.Projection, result.Projection);
            Assert.Equal(mapConfiguration.TileGridJs, result.TileGridJs);
            Assert.Equal(mapConfiguration.CenterX, result.CenterX);
            Assert.Equal(mapConfiguration.CenterY, result.CenterY);
            Assert.Equal(mapConfiguration.MinX, result.MinX);
            Assert.Equal(mapConfiguration.MinY, result.MinY);
            Assert.Equal(mapConfiguration.MaxX, result.MaxX);
            Assert.Equal(mapConfiguration.MaxY, result.MaxY);
            Assert.Equal(mapConfiguration.Resolutions, result.Resolutions);
            Assert.Equal(mapConfiguration.DefaultResolution, result.DefaultResolution);
            Assert.Equal(mapConfiguration.CreatedById, result.CreatedById);
            Assert.Equal(mapConfiguration.CreatedOn, result.CreatedOn);
            Assert.Equal(mapConfiguration.UpdatedById, result.UpdatedById);
            Assert.Equal(mapConfiguration.UpdatedOn, result.UpdatedOn);
        }

        [Fact]
        public void Should_Map_MapConfigurationDTO_To_MapConfiguration()
        {
            // Arrange
            var mapConfigurationDTO = new MapConfigurationDTO
            {
                Id = Guid.NewGuid(),
                MapName = "Test Map DTO",
                Projection = "EPSG:3857",
                TileGridJs = "tileGridDTO.js",
                CenterX = 5.0,
                CenterY = 6.0,
                MinX = 4.0,
                MinY = 4.0,
                MaxX = 8.0,
                MaxY = 10.0,
                Resolutions = "4,5,6",
                DefaultResolution = 5.0,
                CreatedById = "user789",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "user012",
                UpdatedOn = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var result = _mapper.Map<MapConfiguration>(mapConfigurationDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapConfigurationDTO.Id, result.Id);
            Assert.Equal(mapConfigurationDTO.MapName, result.MapName);
            Assert.Equal(mapConfigurationDTO.Projection, result.Projection);
            Assert.Equal(mapConfigurationDTO.TileGridJs, result.TileGridJs);
            Assert.Equal(mapConfigurationDTO.CenterX, result.CenterX);
            Assert.Equal(mapConfigurationDTO.CenterY, result.CenterY);
            Assert.Equal(mapConfigurationDTO.MinX, result.MinX);
            Assert.Equal(mapConfigurationDTO.MinY, result.MinY);
            Assert.Equal(mapConfigurationDTO.MaxX, result.MaxX);
            Assert.Equal(mapConfigurationDTO.MaxY, result.MaxY);
            Assert.Equal(mapConfigurationDTO.Resolutions, result.Resolutions);
            Assert.Equal(mapConfigurationDTO.DefaultResolution, result.DefaultResolution);
            Assert.Equal(mapConfigurationDTO.CreatedById, result.CreatedById);
            Assert.Equal(mapConfigurationDTO.CreatedOn, result.CreatedOn);
            Assert.Equal(mapConfigurationDTO.UpdatedById, result.UpdatedById);
            Assert.Equal(mapConfigurationDTO.UpdatedOn, result.UpdatedOn);
        }

        [Fact]
        public void Should_Return_Null_When_MapConfiguration_Is_Null()
        {
            // Arrange
            MapConfiguration mapConfiguration = null;

            // Act
            var result = _mapper.Map<MapConfigurationDTO>(mapConfiguration);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Should_Return_Null_When_MapConfigurationDTO_Is_Null()
        {
            // Arrange
            MapConfigurationDTO mapConfigurationDTO = null;

            // Act
            var result = _mapper.Map<MapConfiguration>(mapConfigurationDTO);

            // Assert
            Assert.Null(result);
        }       

    }
}
