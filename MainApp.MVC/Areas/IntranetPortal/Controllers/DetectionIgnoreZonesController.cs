using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DetectionIgnoreZonesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
