using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;
namespace MainApp.MVC.ViewComponents.Map

{
    public class DetectionIgnoreZonesToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public DetectionIgnoreZonesToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolDetectionIgnoreZones) && User.HasAuthClaim(SD.AuthClaims.ManageDetectionIgnoreZones))
            {
                return View();
            }
            else
            {
                return Task.FromResult<IViewComponentResult>(Content(string.Empty)).Result;
            }
        }
    }
}
