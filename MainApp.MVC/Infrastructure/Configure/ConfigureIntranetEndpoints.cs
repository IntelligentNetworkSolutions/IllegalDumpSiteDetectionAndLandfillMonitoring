namespace MainApp.MVC.Infrastructure.Configure
{
    public static class ConfigureIntranetEndpoints
    {
        public static void ConfigureIntranetPortalEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
                                                pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}")
                        //.RequireAuthorization()
                        ;
            });
        }
    }
}
