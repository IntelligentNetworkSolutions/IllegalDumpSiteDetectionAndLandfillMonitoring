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
        var pwaAppPath = _configuration.GetValue<string>("DomainSettings:PwaAppPath", "PWA");
        var manifestFile = _configuration.GetValue<string>("AppInstanceSettings:InstanceName", "WasteDetection");
        var appStartupMode = _configuration.GetValue<string>("ApplicationStartupMode");

        appPath = !string.IsNullOrWhiteSpace(appPath) ? "/" + appPath.Trim('/') : "";

        var baseUrl = HttpContext.Request.IsHttps ? "https://" : "http://";
        var fullDomain = $"{baseUrl}{domain}";

        ViewData["AppPath"] = appPath;
        ViewData["PwaAppPath"] = pwaAppPath;
        ViewData["FullAppUrl"] = $"/IntranetPortal/InspectionPwa/";
        ViewData["InstanceName"] = manifestFile;

        Response.Headers.Add("Content-Type", "application/json");
        return View($"Manifest/{manifestFile}");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ServiceWorker()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:MainAppPath", "");
        var appStartupMode = _configuration.GetValue<string>("ApplicationStartupMode");
        var instanceName = _configuration.GetValue<string>("AppInstanceSettings:InstanceName", "WasteDetection");

        appPath = !string.IsNullOrWhiteSpace(appPath) ? "/" + appPath.Trim('/') : "";

        // For local development vs production
        var baseUrl = HttpContext.Request.IsHttps ? "https://" : "http://";
        var fullDomain = $"{baseUrl}{domain}";

        ViewData["AppPath"] = $"{fullDomain}";
        ViewData["InstanceName"] = instanceName;

        Response.Headers.Add("Content-Type", "text/javascript");
        return View();
    }
}