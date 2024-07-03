using System.Globalization;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using SD.Helpers;
using Westwind.Globalization;
using Westwind.Globalization.AspnetCore;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterLocalization
    {
        public static void RegisterWestwindLocalization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Properties";
            });

            services.AddSingleton(typeof(IStringLocalizerFactory),
                      typeof(DbResStringLocalizerFactory));
            services.AddSingleton(typeof(IHtmlLocalizerFactory),
                                  typeof(DbResHtmlLocalizerFactory));

            services.AddWestwindGlobalization(opt =>
            {
                opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;
                opt.DbResourceDataManagerType = typeof(DbResourcePostgreSqlDataManager);
                opt.ConnectionString = configuration.GetConnectionString("MasterDatabase");
                opt.DataProvider = DbResourceProviderTypes.PostgreSql;
                opt.ResourceTableName = "Localizations";
                opt.AddMissingResources = true;
                opt.ResxBaseFolder = "~/Properties/";
                opt.ConfigureAuthorizeLocalizationAdministration(actionContext =>
                {
                    return actionContext.HttpContext.User.HasCustomClaim("SpecialAuthClaim", "superadmin");
                });

            });
            CultureInfo[] supportedCultures = new[]
            {
                new CultureInfo("en")
            };

            CultureInfo[] supportedUiCultures = new[]
            {
                new CultureInfo("mk"),
                new CultureInfo("sq"),
                new CultureInfo("en")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en", "en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedUiCultures;
                options.RequestCultureProviders =
                    new List<IRequestCultureProvider>
                    {
                        new LocalizationQueryProvider { QureyParamterName = "culture" },
                        new CookieRequestCultureProvider()
                    };
            });
        }
    }
}
