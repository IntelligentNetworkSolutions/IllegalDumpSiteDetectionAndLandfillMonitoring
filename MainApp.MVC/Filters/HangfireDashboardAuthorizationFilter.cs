using Hangfire.Dashboard;

namespace MainApp.MVC.Filters
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            // Allow superadmin to use hangfire dashboard.
            return httpContext.User.HasClaim("SpecialAuthClaim", "superadmin");
        }
    }
}
