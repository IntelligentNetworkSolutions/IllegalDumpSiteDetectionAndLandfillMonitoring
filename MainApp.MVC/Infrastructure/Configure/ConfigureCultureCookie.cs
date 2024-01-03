using Microsoft.AspNetCore.Localization;

namespace MainApp.MVC.Infrastructure.Configure
{
    public static class ConfigureCultureCookie
    {
        public static void ConfigureMissingCultureCookie(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(async (context, next) =>
            {
                var cookie = context.Request.Cookies[".AspNetCore.Culture"];
                var cultureParam = context.Request.Query["culture"];

                if (string.IsNullOrEmpty(cookie) && string.IsNullOrEmpty(cultureParam))
                {
                    var host_url = context.Request.Host;
                    string localeId = configuration.GetValue<string>("DefaultLocale:" + host_url, "en");

                    var newCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(localeId));
                    context.Response.Cookies.Append(".AspNetCore.Culture", newCookieValue);
                    context.Response.Redirect(context.Request.PathBase + context.Request.Path);
                }

                await next();
            });
        }
    }
}
