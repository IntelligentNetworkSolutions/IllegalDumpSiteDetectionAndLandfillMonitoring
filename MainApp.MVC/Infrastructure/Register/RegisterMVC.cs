using Microsoft.AspNetCore.Mvc.Localization;
using Westwind.Globalization.AspnetCore;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterMVC
    {
        public static void RegisterMvc(this IServiceCollection services)
        {
            services.AddMvc()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        // required for LocalizationAdmin interface for dynamic serialization
                        // (options part required for GeoJson serialization)
                    })
                    .AddViewLocalization() // for MVC/RazorPages localization
                    .AddDataAnnotationsLocalization() // for ViewModel Error Annotation Localization
                    ;

            // This *has to go here* after view localization has been initialized so that Pages can localize
            // Note required even if you're not using the DbResource manager.
            services.AddTransient<IViewLocalizer, DbResViewLocalizer>();
        }
    }
}
