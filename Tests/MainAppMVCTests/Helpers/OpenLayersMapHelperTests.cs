using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.MVC.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Helpers
{
    public class OpenLayersMapHelperTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly OpenLayersMapHelper _openLayersMapHelper;

        public OpenLayersMapHelperTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _openLayersMapHelper = new OpenLayersMapHelper(
                new MapConfigurationDTO(),
                "en",
                _mockConfiguration.Object
            );
        }

        [Fact]
        public void GetLayersJavaScript_ReturnsEmptyArray_WhenNoLayersAndGroups()
        {
            var result = _openLayersMapHelper.GetLayersJavaScript();

            Assert.Equal("[]", result);
        }

        [Fact]
        public void GenerateLayers_ReturnsCorrectHtml_WhenLayersExist()
        {
            var layers = new List<MapLayerConfigurationDTO>
            {
                new MapLayerConfigurationDTO
                {
                    LayerTitleJson = "{\"en\": \"Layer1\"}",
                    LayerJs = "LayerJs1",
                    Order = 1,
                    LayerName = "LayerName1"
                },
                new MapLayerConfigurationDTO
                {
                    LayerTitleJson = "{\"en\": \"Layer2\"}",
                    LayerJs = "LayerJs2",
                    Order = 2,
                    LayerName = "LayerName2"
                }
            };

            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerIpOrDomain"]).Returns("localhost");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerPort"]).Returns("8080");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerWorkspace"]).Returns("workspace");

            var mapConfig = new MapConfigurationDTO
            {
                MinX = 0,
                MinY = 0,
                MaxX = 100,
                MaxY = 100,
                Resolutions = "[]",
                TileGridJs = "tileGridJs",
                Projection = "EPSG:3857",
                MapLayerGroupConfigurations = new List<MapLayerGroupConfigurationDTO>(),
                MapLayerConfigurations = layers
            };

            var helper = new OpenLayersMapHelper(mapConfig, "en", _mockConfiguration.Object);

            var result = helper.GenerateLayers(layers.OrderBy(o => o.Order));

            var expected = "new LayerJs1,new LayerJs2";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GenerateGroups_ReturnsCorrectHtml_WhenGroupsExist()
        {
            var groups = new List<MapLayerGroupConfigurationDTO>
            {
                new MapLayerGroupConfigurationDTO
                {
                    LayerGroupTitleJson = "{\"en\": \"Group1\"}",
                    LayerGroupDescriptionJson = "{\"en\": \"Description1\"}",
                    GroupJs = "GroupJs1",
                    Order = 1,
                    GroupName = "GroupName1",
                    MapLayerConfigurations = new List<MapLayerConfigurationDTO>
                    {
                        new MapLayerConfigurationDTO
                        {
                            LayerTitleJson = "{\"en\": \"Layer1\"}",
                            LayerJs = "LayerJs1",
                            Order = 1,
                            LayerName = "LayerName1"
                        }
                    }
                },
                new MapLayerGroupConfigurationDTO
                {
                    LayerGroupTitleJson = "{\"en\": \"Group2\"}",
                    LayerGroupDescriptionJson = "{\"en\": \"Description2\"}",
                    GroupJs = "GroupJs2",
                    Order = 2,
                    GroupName = "GroupName2",
                    MapLayerConfigurations = new List<MapLayerConfigurationDTO>
                    {
                        new MapLayerConfigurationDTO
                        {
                            LayerTitleJson = "{\"en\": \"Layer2\"}",
                            LayerJs = "LayerJs2",
                            Order = 2,
                            LayerName = "LayerName2"
                        }
                    }
                }
            };

            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerIpOrDomain"]).Returns("localhost");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerPort"]).Returns("8080");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerWorkspace"]).Returns("workspace");

            var mapConfig = new MapConfigurationDTO
            {
                MinX = 0,
                MinY = 0,
                MaxX = 100,
                MaxY = 100,
                Resolutions = "[]",
                TileGridJs = "tileGridJs",
                Projection = "EPSG:3857",
                MapLayerGroupConfigurations = groups,
                MapLayerConfigurations = new List<MapLayerConfigurationDTO>()
            };

            var helper = new OpenLayersMapHelper(mapConfig, "en", _mockConfiguration.Object);

            var result = helper.GenerateGroups(groups.OrderBy(o => o.Order));

            var expected = "new GroupJs1,new GroupJs2";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GenerateLayers_ReturnsEmptyString_WhenNoLayersExist()
        {
            var layers = new List<MapLayerConfigurationDTO>();

            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerIpOrDomain"]).Returns("localhost");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerPort"]).Returns("8080");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerWorkspace"]).Returns("workspace");

            var mapConfig = new MapConfigurationDTO
            {
                MinX = 0,
                MinY = 0,
                MaxX = 100,
                MaxY = 100,
                Resolutions = "[]",
                TileGridJs = "tileGridJs",
                Projection = "EPSG:3857",
                MapLayerGroupConfigurations = new List<MapLayerGroupConfigurationDTO>(),
                MapLayerConfigurations = layers
            };

            var helper = new OpenLayersMapHelper(mapConfig, "en", _mockConfiguration.Object);

            var result = helper.GenerateLayers(layers.OrderBy(o => o.Order));

            Assert.Equal("", result);
        }
        
        [Fact]
        public void GenerateGroups_ReturnsEmptyString_WhenNoGroupsExist()
        {
            var groups = new List<MapLayerGroupConfigurationDTO>();

            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerIpOrDomain"]).Returns("localhost");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerPort"]).Returns("8080");
            _mockConfiguration.SetupGet(x => x["AppSettings:GeoServerWorkspace"]).Returns("workspace");

            var mapConfig = new MapConfigurationDTO
            {
                MinX = 0,
                MinY = 0,
                MaxX = 100,
                MaxY = 100,
                Resolutions = "[]",
                TileGridJs = "tileGridJs",
                Projection = "EPSG:3857",
                MapLayerGroupConfigurations = groups,
                MapLayerConfigurations = new List<MapLayerConfigurationDTO>()
            };

            var helper = new OpenLayersMapHelper(mapConfig, "en", _mockConfiguration.Object);

            var result = helper.GenerateGroups(groups.OrderBy(o => o.Order));

            Assert.Equal("", result);
        }


    }
}
