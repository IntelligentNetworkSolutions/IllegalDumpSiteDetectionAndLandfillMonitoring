using Audit.Core;
using SD;

namespace MainApp.MVC.Infrastructure.Configure
{
    public static class ConfigureAudit
    {
        public static void ConfigureAuditNet(this IApplicationBuilder app, string applicationStartMode)
        {
            Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
            {
                scope.SetCustomField("audit_internal_user", String.Empty);
                if (applicationStartMode == ApplicationStartModes.IntranetPortal)
                {
                    var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                    scope.SetCustomField("audit_internal_user", httpContextAccessor.HttpContext?.User?.Identities?
                                                                                    .First()?.FindFirst("Username")?.Value);
                }
            });
        }
    }
}
