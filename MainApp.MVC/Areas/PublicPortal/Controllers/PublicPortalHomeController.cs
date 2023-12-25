using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.PublicPortal.Controllers
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
