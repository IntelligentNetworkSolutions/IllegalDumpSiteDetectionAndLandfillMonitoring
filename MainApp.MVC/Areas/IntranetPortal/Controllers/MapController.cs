using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class MapController : Controller
    {
        // A TEST MAP CONTROLLER
        public IActionResult Index(Guid? detectionRunId)
        {
            return View(detectionRunId);
        }
    }
}
