using DAL.ApplicationStorage;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterDbContext
    {
        public static void RegisterMainAppDb(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();
        }
    }
}
