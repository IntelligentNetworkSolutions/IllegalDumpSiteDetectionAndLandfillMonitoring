using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using System.Diagnostics;
using Westwind.Globalization;

namespace MainApp.Areas.Common.Controllers
{
    [Area("Common")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration _configuration { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string defaultRoute = "";
            string appStartMode = _configuration["ApplicationStartupMode"];

            if (appStartMode == SD.ApplicationStartModes.IntranetPortal)
            {
                defaultRoute = "~/IntranetPortal/UserManagement/Index";
            }
            else
            {
                defaultRoute = "~/PublicPortal/PublicPortalHome/Index";
            }

            return Redirect(defaultRoute);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetTranslationCache()
        {
            try
            {
                DbResourceConfiguration.ClearResourceCache();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}