using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.Common.Controllers
{
    [Area("Common")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("/Error")]
        public IActionResult Error()
        {
            return View();
        }
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var view = "Error";
            switch (statusCode)
            {
                case 401:
                    view = "Error_401";
                    break;
                case 403:
                    view = "Error_403";
                    break;
                case 404:
                    view = "Error_404";
                    break;
                case 500:
                    view = "Error_500";
                    break;
            }
            return View(view);
        }
    }
}
