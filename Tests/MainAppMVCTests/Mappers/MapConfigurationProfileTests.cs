using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Mappers
{
    public class MapConfigurationProfileTests
    {
        private readonly IMapper _mapper;

        public MapConfigurationProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapConfigurationProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Map_MapConfigurationDTO_To_ViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var updatedOn = DateTime.UtcNow.AddDays(1);
            var dto = new MapConfigurationDTO
            {
                Id = Guid.NewGuid(),
                MapName = "Test Map",
                Projection = "EPSG:3857",
                TileGridJs = "tileGrid.js",
                CenterX = 10.5,
                CenterY = 20.5,
                MinX = 0,
                MinY = 0,
                MaxX = 100,
                MaxY = 100,
                Resolutions = "[1,2,3]",
                DefaultResolution = 1.0,
                CreatedById = "user1",
                CreatedOn = createdOn,
                CreatedBy = new UserDTO(),
                UpdatedById = "user2",
                UpdatedOn = updatedOn,
                UpdatedBy = new UserDTO(),
                MapLayerConfigurations = new List<MapLayerConfigurationDTO>(),
                MapLayerGroupConfigurations = new List<MapLayerGroupConfigurationDTO>()
            };

            // Act
            var viewModel = _mapper.Map<MapConfigurationViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.MapName, viewModel.MapName);
            Assert.Equal(dto.Projection, viewModel.Projection);
            Assert.Equal(dto.TileGridJs, viewModel.TileGridJs);
            Assert.Equal(dto.CenterX, viewModel.CenterX);
            Assert.Equal(dto.CenterY, viewModel.CenterY);
            Assert.Equal(dto.MinX, viewModel.MinX);
            Assert.Equal(dto.MinY, viewModel.MinY);
            Assert.Equal(dto.MaxX, viewModel.MaxX);
            Assert.Equal(dto.MaxY, viewModel.MaxY);
            Assert.Equal(dto.Resolutions, viewModel.Resolutions);
            Assert.Equal(dto.DefaultResolution, viewModel.DefaultResolution);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.CreatedBy);
            Assert.Equal(dto.UpdatedById, viewModel.UpdatedById);
            Assert.Equal(dto.UpdatedOn, viewModel.UpdatedOn);
            Assert.NotNull(viewModel.UpdatedBy);
            Assert.NotNull(viewModel.MapLayerConfigurations);
            Assert.NotNull(viewModel.MapLayerGroupConfigurations);
        }

        [Fact]
        public void Map_MapLayerConfigurationDTO_To_ViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var updatedOn = DateTime.UtcNow.AddDays(1);
            var dto = new MapLayerConfigurationDTO
            {
                Id = Guid.NewGuid(),
                LayerName = "Test Layer",
                LayerTitleJson = "{\"title\":\"Test\"}",
                LayerDescriptionJson = "{\"description\":\"Test\"}",
                Order = 1,
                LayerJs = "layer.js",
                MapConfigurationId = Guid.NewGuid(),
                MapConfiguration = new MapConfigurationDTO(),
                MapLayerGroupConfigurationId = Guid.NewGuid(),
                MapLayerGroupConfiguration = new MapLayerGroupConfigurationDTO(),
                CreatedById = "user1",
                CreatedOn = createdOn,
                CreatedBy = new UserDTO(),
                UpdatedById = "user2",
                UpdatedOn = updatedOn,
                UpdatedBy = new UserDTO()
            };

            // Act
            var viewModel = _mapper.Map<MapLayerConfigurationViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.LayerName, viewModel.LayerName);
            Assert.Equal(dto.LayerTitleJson, viewModel.LayerTitleJson);
            Assert.Equal(dto.LayerDescriptionJson, viewModel.LayerDescriptionJson);
            Assert.Equal(dto.Order, viewModel.Order);
            Assert.Equal(dto.LayerJs, viewModel.LayerJs);
            Assert.Equal(dto.MapConfigurationId, viewModel.MapConfigurationId);
            Assert.NotNull(viewModel.MapConfiguration);
            Assert.Equal(dto.MapLayerGroupConfigurationId, viewModel.MapLayerGroupConfigurationId);
            Assert.NotNull(viewModel.MapLayerGroupConfiguration);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.CreatedBy);
            Assert.Equal(dto.UpdatedById, viewModel.UpdatedById);
            Assert.Equal(dto.UpdatedOn, viewModel.UpdatedOn);
            Assert.NotNull(viewModel.UpdatedBy);
        }

        [Fact]
        public void Map_MapLayerGroupConfigurationDTO_To_ViewModel_Should_Map_All_Properties()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var updatedOn = DateTime.UtcNow.AddDays(1);
            var dto = new MapLayerGroupConfigurationDTO
            {
                Id = Guid.NewGuid(),
                GroupName = "Test Group",
                LayerGroupTitleJson = "{\"title\":\"Test\"}",
                LayerGroupDescriptionJson = "{\"description\":\"Test\"}",
                Order = 1,
                Opacity = 0.8,
                GroupJs = "group.js",
                MapConfigurationId = Guid.NewGuid(),
                MapConfiguration = new MapConfigurationDTO(),
                CreatedById = "user1",
                CreatedOn = createdOn,
                CreatedBy = new UserDTO(),
                UpdatedById = "user2",
                UpdatedOn = updatedOn,
                UpdatedBy = new UserDTO(),
                MapLayerConfigurations = new List<MapLayerConfigurationDTO>()
            };

            // Act
            var viewModel = _mapper.Map<MapLayerGroupConfigurationViewModel>(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.GroupName, viewModel.GroupName);
            Assert.Equal(dto.LayerGroupTitleJson, viewModel.LayerGroupTitleJson);
            Assert.Equal(dto.LayerGroupDescriptionJson, viewModel.LayerGroupDescriptionJson);
            Assert.Equal(dto.Order, viewModel.Order);
            Assert.Equal(dto.Opacity, viewModel.Opacity);
            Assert.Equal(dto.GroupJs, viewModel.GroupJs);
            Assert.Equal(dto.MapConfigurationId, viewModel.MapConfigurationId);
            Assert.NotNull(viewModel.MapConfiguration);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.NotNull(viewModel.CreatedBy);
            Assert.Equal(dto.UpdatedById, viewModel.UpdatedById);
            Assert.Equal(dto.UpdatedOn, viewModel.UpdatedOn);
            Assert.NotNull(viewModel.UpdatedBy);
            Assert.NotNull(viewModel.MapLayerConfigurations);
        }

        [Fact]
        public void Map_Null_DTOs_Should_Return_Null_ViewModels()
        {
            // Arrange
            MapConfigurationDTO mapConfigDto = null;
            MapLayerConfigurationDTO layerConfigDto = null;
            MapLayerGroupConfigurationDTO groupConfigDto = null;

            // Act
            var mapConfigViewModel = _mapper.Map<MapConfigurationViewModel>(mapConfigDto);
            var layerConfigViewModel = _mapper.Map<MapLayerConfigurationViewModel>(layerConfigDto);
            var groupConfigViewModel = _mapper.Map<MapLayerGroupConfigurationViewModel>(groupConfigDto);

            // Assert
            Assert.Null(mapConfigViewModel);
            Assert.Null(layerConfigViewModel);
            Assert.Null(groupConfigViewModel);
        }

        [Fact]
        public void Map_Empty_Collections_Should_Initialize_Empty_Collections()
        {
            // Arrange
            var mapConfigDto = new MapConfigurationDTO();
            var groupConfigDto = new MapLayerGroupConfigurationDTO();

            // Act
            var mapConfigViewModel = _mapper.Map<MapConfigurationViewModel>(mapConfigDto);
            var groupConfigViewModel = _mapper.Map<MapLayerGroupConfigurationViewModel>(groupConfigDto);

            // Assert
            Assert.NotNull(mapConfigViewModel.MapLayerConfigurations);
            Assert.Empty(mapConfigViewModel.MapLayerConfigurations);
            Assert.NotNull(mapConfigViewModel.MapLayerGroupConfigurations);
            Assert.Empty(mapConfigViewModel.MapLayerGroupConfigurations);
            Assert.NotNull(groupConfigViewModel.MapLayerConfigurations);
            Assert.Empty(groupConfigViewModel.MapLayerConfigurations);
        }
    }
}
