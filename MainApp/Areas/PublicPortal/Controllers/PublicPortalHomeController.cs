﻿using Microsoft.AspNetCore.Mvc;

namespace MainApp.Areas.PublicPortal.Controllers
{
    [Area("PublicPortal")]
    public class PublicPortalHomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
