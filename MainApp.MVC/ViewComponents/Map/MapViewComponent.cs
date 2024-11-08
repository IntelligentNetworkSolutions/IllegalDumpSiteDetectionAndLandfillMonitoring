using DTOs.MainApp.BL;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MainApp.MVC.ViewComponents.Map
{    
    public class MapModel
    {
        public string LayersJavaScript { get; set; }
        public string MapDivId { get; set; }
        public string MapOverviewUrl { get; set; }
        public MapConfigurationDTO MapConfiguration { get; set; }
    }

    public class MapViewComponent : ViewComponent
    {
        private readonly IMapConfigurationService _mapConfigurationService;            
        private readonly IConfiguration _configuration;
        private IApplicationSettingsService _applicationSettingsService;

        public MapViewComponent(
            IMapConfigurationService mapConfigurationService,
            IConfiguration configuration,
            IApplicationSettingsService applicationSettingsService)
        {
            _mapConfigurationService = mapConfigurationService;
            _configuration = configuration;
            _applicationSettingsService = applicationSettingsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string mapDivId, string mapToLoad)
        {
            var mapConf = await _mapConfigurationService.GetMapConfigurationByName(mapToLoad);

            if (mapConf.Id == Guid.Empty)
            {
                return View("Error");
            }

            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var olMapHelper = new OpenLayersMapHelper(mapConf, language, _configuration);
            var mapOverviewUrl = await _applicationSettingsService.GetApplicationSettingByKey("MapOverviewUrl");

            var model = new MapModel
            {
                MapConfiguration = mapConf,
                LayersJavaScript = olMapHelper.GetLayersJavaScript(),
                MapDivId = mapDivId,
                MapOverviewUrl = mapOverviewUrl?.Value ?? ""
            };

            return View(model);
        }

    }
    
}
