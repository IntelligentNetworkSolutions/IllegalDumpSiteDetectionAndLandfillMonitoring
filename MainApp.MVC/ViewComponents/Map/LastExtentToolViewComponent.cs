using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class LastExtentToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public LastExtentToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {            
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolLastExtent) && User.HasAuthClaim(SD.AuthClaims.MapToolLastExtent))
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
