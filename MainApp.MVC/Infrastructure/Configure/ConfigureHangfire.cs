using Hangfire;
using MainApp.MVC.Filters;
using SD;

namespace MainApp.MVC.Infrastructure.Configure
{
    public static class ConfigureHangfire
    {
        public static void ConfigureHangfireDashboardAndJobs(this IApplicationBuilder app)
        {          
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                   new HangfireDashboardAuthorizationFilter()
                },
                IgnoreAntiforgeryToken = true,
                
            });
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });
        }
    }
}
