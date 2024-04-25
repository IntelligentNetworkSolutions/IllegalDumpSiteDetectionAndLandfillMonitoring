using DAL.ApplicationStorage;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using System.Reflection;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterDbContext
    {
        public static void RegisterMainAppDb(this IServiceCollection services, IConfiguration configuration)
        {            
            var connectionString = configuration.GetConnectionString("MasterDatabase");
            
            //string applicationStartMode = _configuration.GetSection("ApplicationStartupMode").Value;

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseNetTopologySuite();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<ApplicationDbContext>((options) =>
            {
                options.UseNpgsql(dataSource, o =>
                {
                    o.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                    //o.MigrationsHistoryTable(schemaService.MigrationsTableName, schemaService.Name);
                    o.UseNetTopologySuite();
                });
            });
        }
    }
}

