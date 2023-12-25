using Dal.Helpers;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;
using Westwind.Globalization;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]

    public class SpecialActionsController : Controller
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        private readonly ApplicationSettingsHelper _applicationSettingsHelper;
        private readonly IConfiguration _configuration;

        public SpecialActionsController(ModulesAndAuthClaimsHelper modulesAndAuthClaims, ApplicationSettingsHelper applicationSettingsHelper, IConfiguration configuration)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
            _applicationSettingsHelper = applicationSettingsHelper;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.HasAuthClaim(SD.AuthClaims.SpecialActions) || !_modulesAndAuthClaims.HasModule(SD.Modules.SpecialActions))
            {
                var errorPath = _configuration["ErrorViewsPath:Error403"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return StatusCode(403);
                }
            }
            return View();
        }       
    }
}
