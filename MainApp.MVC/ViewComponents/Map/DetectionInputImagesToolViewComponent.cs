using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class DetectionInputImagesToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public DetectionInputImagesToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolDetectionInputImages) && User.HasAuthClaim(SD.AuthClaims.ViewDetectionInputImages))
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
