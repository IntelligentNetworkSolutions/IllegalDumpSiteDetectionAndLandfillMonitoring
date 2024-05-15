using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Security.Claims;
using Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using MainApp.MVC.ViewModels.IntranetPortal.Account;
using DAL.Repositories;
using Services.Interfaces.Services;
using DAL.Interfaces.Helpers;
using SD;
using Westwind.Globalization;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class LandingPageController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactForm(string name, string email, string message)
        {
            Console.WriteLine($"Name: {name}, Email: {email}, Message: {message}");

            return RedirectToAction("Index");
        }

    }
}
