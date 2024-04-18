using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Newtonsoft.Json.Linq;

namespace MainApp.MVC.Helpers
{
    public class OpenLayersMapHelper
    {
        private readonly MapConfigurationDTO _mapConfig;
        private readonly string _language;
        private readonly IConfiguration _configuration;

        public OpenLayersMapHelper(
            MapConfigurationDTO mapConfig,
            string language,
            IConfiguration configuration)
        {
            _mapConfig = mapConfig;
            _language = language;
            _configuration = configuration;
        }

        public string GetLayersJavaScript()
        {
            var html = "[";

            if (_mapConfig.MapLayerGroupConfigurations?.Count > 0)
            {
                html += GenerateGroups(_mapConfig.MapLayerGroupConfigurations.OrderBy(o => o.Order));
            }
            if (_mapConfig.MapLayerConfigurations?.Count > 0)
            {
                html += html == "[" ? "" : ",";
                html += GenerateLayers(_mapConfig.MapLayerConfigurations.OrderBy(o => o.Order));
            }
            html += "]";

            return html;
        }

        public string GenerateLayers(IOrderedEnumerable<MapLayerConfigurationDTO> layers)
        {
            string? mapServerIpOrDomain = _configuration["AppSettings:GeoServerIpOrDomain"];
            string? mapServerPort = _configuration["AppSettings:GeoServerPort"];
            string? mapServerAppFolder = _configuration["AppSettings:GeoServerWorkspace"];

            string html = "";

            string tileGridJs = _mapConfig.TileGridJs;
            tileGridJs = tileGridJs.Replace("%MINX%", _mapConfig.MinX.ToString())
                .Replace("%MINY%", _mapConfig.MinY.ToString())
                .Replace("%MAXX%", _mapConfig.MaxX.ToString())
                .Replace("%MAXY%", _mapConfig.MaxY.ToString())
                .Replace("%RESOLUTIONS%", _mapConfig.Resolutions);

            foreach (var layer in layers)
            {
                string serverAndPort = mapServerIpOrDomain + (mapServerPort != "" ? ":" + mapServerPort : "");

                dynamic jToken = JToken.Parse(layer.LayerTitleJson);
                string title = (jToken[_language] != null) ? jToken[_language] : jToken["en"];


                string layerJs = layer.LayerJs;
                layerJs = layerJs.Replace("%SERVER_AND_PORT%", serverAndPort)
                    .Replace("%SERVER%", mapServerIpOrDomain)
                    .Replace("%PORT%", mapServerPort)
                    .Replace("%MS4WAPP%", mapServerAppFolder)
                    .Replace("%TILEGRID%", tileGridJs)
                    .Replace("%PROJECTION%", _mapConfig.Projection)
                    .Replace("%NAME%", layer.LayerName)
                    .Replace("%TITLE%", title)
                    .Replace("%ORDER%", layer.Order.ToString());

                html += (html == "" ? "" : ",") + "new " + layerJs;
            }

            return html;
        }

        public string GenerateGroups(IOrderedEnumerable<MapLayerGroupConfigurationDTO> groups)
        {
            string html = "";

            foreach (var group in groups)
            {
                string layers = "[" + GenerateLayers(group.MapLayerConfigurations.OrderBy(o => o.Order)) + "]";

                dynamic jToken = JToken.Parse(group.LayerGroupTitleJson);
                string title = (jToken[_language] != null) ? jToken[_language] : jToken["en"];
                dynamic descriptionJToken = JToken.Parse(group.LayerGroupDescriptionJson);
                string description = descriptionJToken[_language];

                string groupJs = group.GroupJs;
                groupJs = groupJs.Replace("%LAYERS%", layers)
                    .Replace("%NAME%", group.GroupName)
                    .Replace("%TITLE%", title)
                    .Replace("%DESCRIPTION%", description)
                    .Replace("%ORDER%", group.Order.ToString());

                html += (html == "" ? "" : ",") + "new " + groupJs;

            }

            return html;
        }
    }
}
