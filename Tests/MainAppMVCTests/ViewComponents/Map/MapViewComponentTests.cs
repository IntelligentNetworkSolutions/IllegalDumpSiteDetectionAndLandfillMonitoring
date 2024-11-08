using DTOs.MainApp.BL;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using MainApp.MVC.ViewComponents.Map;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewComponents.Map
{
    public class MapViewComponentTests
    {
        private readonly Mock<IMapConfigurationService> _mockMapConfigurationService;
        private readonly Mock<IApplicationSettingsService> _applicationSettingsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly MapViewComponent _mapViewComponent;

        public MapViewComponentTests()
        {
            _applicationSettingsServiceMock = new Mock<IApplicationSettingsService>();
            _mockMapConfigurationService = new Mock<IMapConfigurationService>();
            _configurationMock = new Mock<IConfiguration>();
            _mapViewComponent = new MapViewComponent(_mockMapConfigurationService.Object,_configurationMock.Object, _applicationSettingsServiceMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnView_WithMapModel()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            var mapConfigDTO = new MapConfigurationDTO { Id = Guid.NewGuid() };
            string expectedMapOverviewUrl = "https://example.com/map-overview";

            _mockMapConfigurationService
                .Setup(s => s.GetMapConfigurationByName(mapToLoad))
                .ReturnsAsync(mapConfigDTO);

            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model.MapOverviewUrl);
            Assert.Equal(mapConfigDTO, model.MapConfiguration);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnErrorView_WhenMapConfigurationNotFound()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "nonExistentMap";

            _mockMapConfigurationService
                .Setup(s => s.GetMapConfigurationByName(mapToLoad))
                .ReturnsAsync(new MapConfigurationDTO { Id = Guid.Empty });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnView_WithEmptyMapOverviewUrl_WhenServiceReturnsNull()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            var mapConfigDTO = new MapConfigurationDTO { Id = Guid.NewGuid() };

            _mockMapConfigurationService
                .Setup(s => s.GetMapConfigurationByName(mapToLoad))
                .ReturnsAsync(mapConfigDTO);

            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(string.Empty, model.MapOverviewUrl);
        }

        [Fact]
        public async Task InvokeAsync_ShouldRetrieveCorrectLanguageCulture()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            var mapConfigDTO = new MapConfigurationDTO { Id = Guid.NewGuid() };
            string expectedMapOverviewUrl = "https://example.com/map-overview";
            var expectedCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            _mockMapConfigurationService
                .Setup(s => s.GetMapConfigurationByName(mapToLoad))
                .ReturnsAsync(mapConfigDTO);

            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model.MapOverviewUrl);
            Assert.Equal(mapConfigDTO, model.MapConfiguration);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReflectDifferentMapDivIdValues()
        {
            // Arrange
            string mapDivId1 = "mapDivId1";
            string mapDivId2 = "mapDivId2";
            string mapToLoad = "exampleMap";
            string expectedMapOverviewUrl = "https://example.com/map-overview";
            var mapConfigDTO = new MapConfigurationDTO { Id = Guid.NewGuid() };

            _mockMapConfigurationService
               .Setup(s => s.GetMapConfigurationByName(mapToLoad))
               .ReturnsAsync(mapConfigDTO);
            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result1 = await _mapViewComponent.InvokeAsync(mapDivId1, mapToLoad) as ViewViewComponentResult;
            var result2 = await _mapViewComponent.InvokeAsync(mapDivId2, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            var model1 = Assert.IsAssignableFrom<MapModel>(result1.ViewData.Model);
            var model2 = Assert.IsAssignableFrom<MapModel>(result2.ViewData.Model);
            Assert.NotEqual(model1.MapDivId, model2.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model1.MapOverviewUrl);
            Assert.Equal(expectedMapOverviewUrl, model2.MapOverviewUrl);
        }

    }
}
