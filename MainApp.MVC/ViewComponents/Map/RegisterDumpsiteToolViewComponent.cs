using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map;

public class RegisterDumpsiteToolViewComponent : ViewComponent
{
    private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
    public RegisterDumpsiteToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
    {
        _modulesAndAuthClaims = modulesAndAuthClaims;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolRegisterDumpsites) && User.HasAuthClaim(SD.AuthClaims.MapToolRegisterDumpsites))
        {
            return View();
        }
        else
        {
            return Task.FromResult<IViewComponentResult>(Content(string.Empty)).Result;
        }
    }
}
