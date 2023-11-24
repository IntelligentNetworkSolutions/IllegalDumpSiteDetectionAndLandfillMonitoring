using Audit.EntityFramework;
using Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dal.ApplicationStorage
{
	public class ApplicationDbContext : AuditIdentityDbContext<ApplicationUser>
	{

		public IConfiguration _configuration { get; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
			IConfiguration configuration)
			: base(options)
		{
			_configuration = configuration;
		}

        public virtual DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public virtual DbSet<IntranetPortalUsersToken> IntranetPortalUsersTokens { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var connectionString = _configuration.GetConnectionString("MasterDatabase");

				string applicationStartMode = _configuration.GetSection("ApplicationStartupMode").Value;

				optionsBuilder.UseNpgsql(connectionString, db => db.UseNetTopologySuite()
								.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name));
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			// builder.HasPostgresExtension("postgis");

			//Custom Entity DB Naming object naming convention for PostgreSQL
			builder.SetEntityNamingConvention();

			//todo: definicija na indeksi za geom i drugi indeksirani koloni posto ne moze preku data anotoacii
			//primer:
			//entity.HasIndex(e => e.Geom)
			//    .HasName("admin_granica_oc_geom_idx")
			//    .HasMethod("gist"); //ova e biten metod na indeksiranje za geom

			

		}

	}
}
