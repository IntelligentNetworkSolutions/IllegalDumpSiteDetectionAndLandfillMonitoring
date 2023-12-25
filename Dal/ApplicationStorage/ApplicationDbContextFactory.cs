using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dal.ApplicationStorage
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// A must-have method in order to create migrations for DBContext that is in separate class library
        /// https://medium.com/@speedforcerun/implementing-idesigntimedbcontextfactory-in-asp-net-core-2-0-2-1-3718bba6db84
        /// https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("Please provide the connection string as a command-line argument.");

            string connectionString = args[0];

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            optionsBuilder.UseNpgsql(connectionString, db => db.UseNetTopologySuite());

            // cd DAL
            // dotnet ef migrations add MigrationName -- "YourConnectionString"
            // dotnet ef migrations remove -- "YourConnectionString"
            // dotnet ef database update -- "YourConnectionString"

            return new ApplicationDbContext(optionsBuilder.Options, null);
        }
    }
}
