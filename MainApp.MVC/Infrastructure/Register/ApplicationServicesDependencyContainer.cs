using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DAL.Interfaces.Repositories;
using DAL.Repositories.DatasetRepositories;
using DAL.Repositories.MapConfigurationRepositories;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.Interfaces.Services;
using Services;
using DAL.Helpers;
using DAL.Interfaces.Helpers;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Services.DatasetServices;
using MainApp.BL.Services.MapConfigurationServices;
using MainApp.BL.Services;
using MainApp.MVC.Helpers;
using Services.Interfaces;
using MailSend;
using MailSend.Interfaces;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.BL.Services.LegalLandfillManagementServices;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class ApplicationServicesDependencyContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.TryAddScoped<IAppSettingsAccessor, AppSettingsAccessor>();
            services.TryAddScoped<IApplicationSettingsService, ApplicationSettingsService>();
            services.TryAddScoped<PasswordValidationHelper>();
            services.TryAddScoped<ModulesAndAuthClaimsHelper>();
            services.TryAddScoped<AuditLogBl>();
            services.TryAddScoped<ILayoutService, LayoutService>();
            services.TryAddScoped<IForgotResetPasswordService, ForgotResetPasswordService>();
            services.TryAddScoped<IMailService, MailService>();
            services.TryAddScoped<ICocoUtilsService, CocoUtilsService>();
            services.TryAddScoped<IDatasetService, DatasetService>();
            services.TryAddScoped<IDatasetClassesService, DatasetClassesService>();
            services.TryAddScoped<IDatasetImagesService, DatasetImagesService>();
            services.TryAddScoped<IDataset_DatasetClassService, Dataset_DatasetClassService>();
            services.TryAddScoped<IMapConfigurationService, MapConfigurationService>();
            services.TryAddScoped<IMapLayersConfigurationService, MapLayersConfigurationService>();
            services.TryAddScoped<IMapLayerGroupsConfigurationService, MapLayerGroupsConfigurationService>();
            services.TryAddScoped<IDetectionRunService, DetectionRunService>();
            services.TryAddScoped<IImageAnnotationsService, ImageAnnotationsService>();
            services.TryAddScoped<IDetectionRunService, DetectionRunService>();
            services.TryAddScoped<ILegalLandfillService, LegalLandfillService>();
            services.TryAddScoped<ILegalLandfillPointCloudFileService, LegalLandfillPointCloudFileService>();

            return services;
        }
    }
}
