using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers;

[Area("IntranetPortal")]
public class InspectionPwaController : Controller
{
    public IConfiguration _configuration;

    public InspectionPwaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ManifestJson()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:MainAppPath", "");
        var manifestFile = _configuration.GetValue<string>("AppInstanceSettings:InstanceName", "manifest"); // Fallback if not found

        appPath = !string.IsNullOrWhiteSpace(appPath) ? "/" + appPath.Trim('/') : "";

        ViewData["AppPath"] = appPath;
        ViewData["FullAppUrl"] = $"https://{domain}{appPath}";

        HttpContext.Response.Headers["Content-Type"] = "application/json";
        return View($"Manifest/{manifestFile}");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ServiceWorker()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:MainAppPath", "");

        appPath = !string.IsNullOrWhiteSpace(appPath) ? "/" + appPath.Trim('/') : "";

        ViewData["AppPath"] = appPath;
        ViewData["FullAppUrl"] = $"https://{domain}{appPath}";

        HttpContext.Response.Headers["Content-Type"] = "text/javascript";
        return View();
    }

}
