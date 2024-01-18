using DAL.Interfaces.Helpers;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]

    public class SpecialActionsController : Controller
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;

        public SpecialActionsController(ModulesAndAuthClaimsHelper modulesAndAuthClaims, IConfiguration configuration, IAppSettingsAccessor appSettingsAccessor)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
            _configuration = configuration;
            _appSettingsAccessor = appSettingsAccessor;
        }

        public async Task<IActionResult> Index()
        {
            // TODO
            //if (!User.HasAuthClaim(SD.AuthClaims.SpecialActions) || !_modulesAndAuthClaims.HasModule(SD.Modules.SpecialActions))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            return View();
        }       
    }
}
