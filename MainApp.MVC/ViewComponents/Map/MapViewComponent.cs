using MainApp.BL.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MainApp.MVC.ViewComponents.Map
{    
    //TODO: To refactor and fill in missing parts

    public class MapModel
    {
        public string LayersJavaScript { get; set; }
        public string MapDivId { get; set; }
        public string MapOverviewUrl { get; set; }

        //public MapConfiguration MapConfiguration { get; set; }
    }

    public class MapViewComponent : ViewComponent
    {
        //private readonly MapConfigurationDa _mapcConfigurationDa;            
        private readonly IConfiguration _configuration;
        private IApplicationSettingsService _applicationSettingsService;

        public MapViewComponent(
            //MapConfigurationDa mapConfigurationDa,
            IConfiguration configuration,
            IApplicationSettingsService applicationSettingsService)
        {
            //_mapcConfigurationDa = mapConfigurationDa;
            _configuration = configuration;
            _applicationSettingsService = applicationSettingsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string mapDivId, string mapToLoad)
        {
            //var mapConf = await _mapcConfigurationDa.GetMapConfigurations(mapToLoad);

            //if (mapConf.Count < 1)
            //{
            //    return View("Error");
            //}                

            var applicationStartMode = _configuration["ApplicationStartupMode"];
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            //var olMapHelper = new OpenLayersMapHelper(mapConf.First(), tenant, language, applicationStartMode);
            var mapOverviewUrl = await _applicationSettingsService.GetApplicationSettingByKey("MapOverviewUrl");

            var model = new MapModel
            {
                //MapConfiguration = mapConf.First(),
                //LayersJavaScript = olMapHelper.GetLayersJavaScript(),
                MapDivId = mapDivId,
                MapOverviewUrl = mapOverviewUrl?.Value ?? ""
            };

            return View(model);
        }

    }
    
}
