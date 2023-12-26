using Dal.ApplicationStorage;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterIdentityProvider
    {
        public static void RegisterMicrosoftIdentity(this IServiceCollection services)
        {
            // TODO: Review
            services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            // {
            //     options.User.RequireUniqueEmail = true;
            //     options.SignIn.RequireConfirmedAccount = false;
            //     options.SignIn.RequireConfirmedEmail = false;
            //     options.SignIn.RequireConfirmedPhoneNumber = false;
            //     options.ClaimsIdentity.UserIdClaimType = "useridentifier";
            //     options.ClaimsIdentity.UserNameClaimType = "username";
            //     options.ClaimsIdentity.EmailClaimType = "email";
            //     options.ClaimsIdentity.RoleClaimType = "rolename";
            //     options.ClaimsIdentity.SecurityStampClaimType = "securitystamp";
            //     options.Lockout.MaxFailedAccessAttempts = 50;
            // })
            ////.AddRoleManager<RoleManager<IdentityRole>>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            ////.AddDefaultTokenProviders()
            ////.AddClaimsPrincipalFactory<ClaimsFactory>()
            //;
        }
    }
}
