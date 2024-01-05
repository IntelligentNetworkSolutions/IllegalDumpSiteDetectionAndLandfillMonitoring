using Microsoft.AspNetCore.Authorization;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterAuthorization
    {
        public static void RegisterAuthorizationMiddleware(this IServiceCollection services)
        {
            services.AddAuthorization(options => new AuthorizationOptions()
            {

            });
        }
    }
}
