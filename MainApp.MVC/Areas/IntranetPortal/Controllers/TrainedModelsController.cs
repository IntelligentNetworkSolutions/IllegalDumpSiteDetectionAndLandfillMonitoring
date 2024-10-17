using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class TrainedModelsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
