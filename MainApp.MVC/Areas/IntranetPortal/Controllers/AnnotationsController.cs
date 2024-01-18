using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class AnnotationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Annotate(string? datasetImageId = null)
        {
            string imgToAnnotatePath = string.Empty;
            if(string.IsNullOrEmpty(datasetImageId))
            {
                imgToAnnotatePath = "";
            }

            imgToAnnotatePath = datasetImageId;




            return View((imgToAnnotatePath));
        }

        [HttpGet]
        public async Task<IActionResult> VGGAnnotator(string? datasetImageId = null)
        {
            string imgToAnnotatePath = string.Empty;
            if (string.IsNullOrEmpty(datasetImageId))
            {
                imgToAnnotatePath = "";
            }

            imgToAnnotatePath = datasetImageId;




            return View((imgToAnnotatePath));
        }
    }
}
