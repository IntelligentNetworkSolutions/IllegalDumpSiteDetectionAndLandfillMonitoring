using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class ZoomToExtentToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public ZoomToExtentToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolZoomToExtent) && User.HasAuthClaim(SD.AuthClaims.MapToolZoomToExtent))
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
