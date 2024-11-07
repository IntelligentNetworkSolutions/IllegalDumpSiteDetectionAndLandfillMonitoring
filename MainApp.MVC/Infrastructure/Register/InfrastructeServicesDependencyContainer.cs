using DAL.ApplicationStorage.SeedDatabase;
using DAL.Interfaces.Repositories;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DAL.Repositories;
using DAL.Repositories.DatasetRepositories;
using DAL.Repositories.DetectionRepositories;
using DAL.Repositories.LegalLandfillManagementRepositories;
using DAL.Repositories.MapConfigurationRepositories;
using DAL.Repositories.TrainingRepositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using Services.Interfaces.Services;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class InfrastructeServicesDependencyContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.TryAddScoped<IApplicationSettingsRepo, ApplicationSettingsRepository>();
            services.TryAddScoped<IUserManagementDa, UserManagementDa>();
            services.TryAddScoped<IIntranetPortalUsersTokenDa, IntranetPortalUsersTokenDa>();
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
            services.TryAddScoped<ILegalLandfillTruckRepository, LegalLandfillTruckRepository>();
            services.TryAddScoped<ILegalLandfillWasteImportRepository, LegalLandfillWasteImportRepository>();
            services.TryAddScoped<ILegalLandfillWasteTypeRepository, LegalLandfillWasteTypeRepository>();
            services.TryAddScoped<IDetectionIgnoreZonesRepository, DetectionIgnoreZonesRepository>();
            services.TryAddScoped<IDetectionInputImageRepository, DetectionInputImageRepository>();
            services.TryAddScoped<ITrainingRunsRepository, TrainingRunsRepository>();
            services.TryAddScoped<ITrainedModelsRepository, TrainedModelsRepository>();
            services.TryAddScoped<ITrainingRunTrainParamsRepository, TrainingRunTrainParamsRepository>();

            return services;
        }
    }
}
