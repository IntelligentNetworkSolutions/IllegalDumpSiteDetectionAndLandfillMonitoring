using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DAL.Interfaces.Repositories;
using DAL.Repositories.DatasetRepositories;
using DAL.Repositories.MapConfigurationRepositories;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Interfaces.Services;
using Services;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DAL.Repositories.DetectionRepositories;
using DAL.ApplicationStorage.SeedDatabase;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Services.DetectionServices;
using Microsoft.AspNetCore.Identity;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DAL.Repositories.LegalLandfillManagementRepositories;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class InfrastructeServicesDependencyContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.TryAddScoped<IApplicationSettingsRepo, ApplicationSettingsRepository>();
            services.TryAddScoped<IUserManagementDa, UserManagementDa>();
            services.TryAddScoped<IntranetPortalUsersTokenDa>();
            services.TryAddScoped<IUserManagementService, UserManagementService>();
            services.TryAddScoped<IAuditLogsDa, AuditLogsDa>();
            services.TryAddScoped<IDatasetsRepository, DatasetsRepository>();
            services.TryAddScoped<IImageAnnotationsRepository, ImageAnnotationsRepository>();
            services.TryAddScoped<IDatasetClassesRepository, DatasetClassesRepository>();
            services.TryAddScoped<IDataset_DatasetClassRepository, Dataset_DatasetClassRepository>();
            services.TryAddScoped<IDatasetImagesRepository, DatasetImagesRepository>();
            services.TryAddScoped<IMapConfigurationRepository, MapConfigurationRepository>();
            services.TryAddScoped<IMapLayersConfigurationRepository, MapLayersConfigurationRepository>();
            services.TryAddScoped<IMapLayerGroupsConfigurationRepository, MapLayerGroupsConfigurationRepository>();
            services.TryAddScoped<IDetectionRunsRepository, DetectionRunsRepository>();
            services.TryAddScoped<IDetectedDumpSitesRepository, DetectedDumpSitesRepository>();
            services.TryAddScoped<IImageAnnotationsRepository, ImageAnnotationsRepository>();
            services.TryAddScoped<IDbInitializer, DbInitializer>();
            services.TryAddScoped<IDetectedDumpSitesRepository, DetectedDumpSitesRepository>();
            services.TryAddScoped<ILegalLandfillRepository, LegalLandfillRepository>();
            services.TryAddScoped<ILegalLandfillPointCloudFileRepository, LegalLandfillPointCloudFileRepository>();
            services.TryAddScoped<IDetectionIgnoreZonesRepository, DetectionIgnoreZonesRepository>();
            
            return services;
        }
    }
}
