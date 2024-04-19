using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class GoToCoordinatesToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;

        public GoToCoordinatesToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.GoToCoordinatesTool) && User.HasAuthClaim(SD.AuthClaims.GoToCoordinatesTool))
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