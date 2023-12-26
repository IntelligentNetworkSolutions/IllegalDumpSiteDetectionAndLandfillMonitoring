namespace MainApp.MVC.Infrastructure.Configure
{
    public static class ConfigurePublicEndpoints
    {
        public static void ConfigurePublicPortalEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
