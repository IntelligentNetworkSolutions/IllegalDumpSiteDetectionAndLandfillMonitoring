using DTOs.MainApp.BL;
using Entities;
using MainApp.BL.Interfaces.Services;
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
        private readonly Mock<IApplicationSettingsService> _applicationSettingsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly MapViewComponent _mapViewComponent;

        public MapViewComponentTests()
        {
            _applicationSettingsServiceMock = new Mock<IApplicationSettingsService>();
            _configurationMock = new Mock<IConfiguration>();
            _mapViewComponent = new MapViewComponent(_configurationMock.Object, _applicationSettingsServiceMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnView_WithMapModel()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            string expectedMapOverviewUrl = "https://example.com/map-overview";

            _configurationMock.Setup(c => c["ApplicationStartupMode"]).Returns("TestMode");
            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model.MapOverviewUrl);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnView_WithEmptyMapOverviewUrl_WhenServiceReturnsNull()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";

            _configurationMock.Setup(c => c["ApplicationStartupMode"]).Returns("TestMode");
            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync((AppSettingDTO)null);

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(string.Empty, model.MapOverviewUrl);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnView_WhenApplicationStartupModeIsMissing()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            string expectedMapOverviewUrl = "https://example.com/map-overview";

            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model.MapOverviewUrl);
        }
      
        [Fact]
        public async Task InvokeAsync_ShouldReflectDifferentMapDivIdValues()
        {
            // Arrange
            string mapDivId1 = "mapDivId1";
            string mapDivId2 = "mapDivId2";
            string mapToLoad = "exampleMap";
            string expectedMapOverviewUrl = "https://example.com/map-overview";

            _configurationMock.Setup(c => c["ApplicationStartupMode"]).Returns("TestMode");
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

        [Fact]
        public async Task InvokeAsync_ShouldRetrieveCorrectLanguageCulture()
        {
            // Arrange
            string mapDivId = "mapDivId";
            string mapToLoad = "exampleMap";
            string expectedMapOverviewUrl = "https://example.com/map-overview";
            var expectedCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            _configurationMock.Setup(c => c["ApplicationStartupMode"]).Returns("TestMode");
            _applicationSettingsServiceMock
                .Setup(s => s.GetApplicationSettingByKey("MapOverviewUrl"))
                .ReturnsAsync(new AppSettingDTO { Value = expectedMapOverviewUrl });

            // Act
            var result = await _mapViewComponent.InvokeAsync(mapDivId, mapToLoad) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<MapModel>(result.ViewData.Model);
            Assert.Equal(mapDivId, model.MapDivId);
            Assert.Equal(expectedMapOverviewUrl, model.MapOverviewUrl);
        }

    }
}
