using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.ViewComponents.Documentation
{
    public class DocumentationViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public DocumentationViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string path)
        {
            string baseUrl = _configuration["Documentation:Guide"];
            string fullUrl = string.IsNullOrWhiteSpace(baseUrl) ? null : $"{baseUrl}{path}";
            ViewData["DocumentationUrl"] = fullUrl;

            return View();
        }
    }
}
