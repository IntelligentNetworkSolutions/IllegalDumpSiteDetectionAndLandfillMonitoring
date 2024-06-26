﻿using MainApp.MVC.ViewModels.IntranetPortal.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Westwind.Globalization;

namespace MainApp.MVC.Areas.Common.Controllers
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

        [AllowAnonymous]
        public IActionResult Index()
        {
            string defaultRoute = "";
            string appStartMode = _configuration["ApplicationStartupMode"];

            if (appStartMode == SD.ApplicationStartModes.IntranetPortal)
            {
                if (User.Identity.IsAuthenticated)
                {
                    defaultRoute = "~/IntranetPortal/Map/Index";
                }
                else
                {
                    defaultRoute = "~/IntranetPortal/LandingPage";
                }
                    
            }
            else
            {
                defaultRoute = "~/PublicPortal/Map/Index";
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