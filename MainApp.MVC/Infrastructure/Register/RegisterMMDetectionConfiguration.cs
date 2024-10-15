using DocumentFormat.OpenXml.Drawing.Charts;
using DTOs.ObjectDetection.API;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Services;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterMMDetectionConfiguration
    {
        public static IServiceCollection AddMMDetectionConfigurationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMMDetectionConfigurationService, MMDetectionConfigurationService>();
            return services;
        }
    }
}
