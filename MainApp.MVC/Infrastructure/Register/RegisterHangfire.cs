using Hangfire;
using Hangfire.PostgreSql;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterHangfire
    {
        public static void RegisterHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(configuration.GetConnectionString("MasterDatabase"))));
        }

        public static void AddHangfireProcessingServer(this IServiceCollection services)
        {
            services.AddHangfireServer();
        }
    }
}
