using DocumentFormat.OpenXml.InkML;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Options;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterHangfire
    {
        public static void RegisterHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => {
                config.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(configuration.GetConnectionString("MasterDatabase")));
                config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
            });
        }

        //public static void AddHangfireProcessingServer(this IServiceCollection services)
        //{
        //    services.AddHangfireServer();
        //}
              
    }
}
