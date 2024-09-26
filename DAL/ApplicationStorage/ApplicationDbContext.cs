using Audit.EntityFramework;
using DAL.Extensions;
using Entities;
using Entities.DatasetEntities;
using Entities.DetectionEntities;
using Entities.LegalLandfillsManagementEntites;
using Entities.MapConfigurationEntities;
using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.ApplicationStorage
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

        #region Dataset Management
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<DatasetClass> DatasetClasses { get; set; }
        public virtual DbSet<Dataset_DatasetClass> Datasets_DatasetClasses { get; set; }
        public virtual DbSet<DatasetImage> DatasetImages { get; set; }
        public virtual DbSet<ImageAnnotation> ImageAnnotations { get; set; }
        #endregion

        #region Map Configuration
        public virtual DbSet<MapConfiguration> MapConfigurations { get; set; }
        public virtual DbSet<MapLayerConfiguration> MapLayerConfigurations { get; set; }
        public virtual DbSet<MapLayerGroupConfiguration> MapLayerGroupConfigurations { get; set; }
        #endregion

        #region Training
        public virtual DbSet<TrainingRun> TrainingRuns { get; set; }
        public virtual DbSet<TrainedModel> TrainedModels { get; set; }
        public virtual DbSet<TrainedModelStatistics> TrainedModelStatistics { get; set; }
        #endregion

        #region Detection
        public virtual DbSet<DetectionRun> DetectionRuns { get; set; }
        public virtual DbSet<DetectedDumpSite> DetectedDumpSites { get; set; }
        public virtual DbSet<DetectionIgnoreZone> DetectionIgnoreZones { get; set; }
        public virtual DbSet<DetectionInputImage> DetectionInputImages { get; set; }

        #endregion

        #region LegalLandfillsManagement
        public virtual DbSet<LegalLandfill> LegalLandfills { get; set; }
        public virtual DbSet<LegalLandfillPointCloudFile> LegalLandfillPointCloudFiles { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //There is a new way of configuring, IMPORTANT! Look in RegisterDbContext.cs

            //if (!optionsBuilder.IsConfigured)
            //{
            //    var connectionString = _configuration["ConnectionStrings:MasterDatabase"];

            //    string applicationStartMode = _configuration.GetSection("ApplicationStartupMode").Value;

            //    optionsBuilder.UseNpgsql(connectionString, db => db.UseNetTopologySuite()
            //                    .MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name));
            //}
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension("uuid-ossp");
            builder.HasPostgresExtension("postgis");

            //Custom Entity DB Naming object naming convention for PostgreSQL
            builder.SetEntityNamingConvention();

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            //todo: definicija na indeksi za geom i drugi indeksirani koloni posto ne moze preku data anotoacii
            //primer:
            //entity.HasIndex(e => e.Geom)
            //    .HasName("admin_granica_oc_geom_idx")
            //    .HasMethod("gist"); //ova e biten metod na indeksiranje za geom
        }
    }
}
