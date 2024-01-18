using DAL.Interfaces.Repositories;
using Entities;
using MainApp.BL.Interfaces.Services;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly IApplicationSettingsRepo _applicationSettingsRepo;
        private readonly ILogger<ApplicationSettingsService> _logger;

        public ApplicationSettingsService(IApplicationSettingsRepo applicationSettingsRepo, ILogger<ApplicationSettingsService> logger)
        {
            _applicationSettingsRepo = applicationSettingsRepo;
            _logger = logger;
        }

        //public async Task<>

        #region Create
        public async Task<ResultDTO> CreateApplicationSetting(AppSettingDTO appSettingDTO)
        {
            try
            {
                if (appSettingDTO is null)
                    return ResultDTO.Fail("Missing App Setting Object");

                if (string.IsNullOrWhiteSpace(appSettingDTO.Value))
                    return ResultDTO.Fail("Missing App Setting Value");

                // TODO: Check Cast to DataType

                // TODO: Use Mapper
                ApplicationSettings applicationSettingEntity = new ApplicationSettings()
                {
                    Key = appSettingDTO.Key,
                    Value = appSettingDTO.Value,
                    Description = string.IsNullOrWhiteSpace(appSettingDTO.Description) ? appSettingDTO.Key : appSettingDTO.Description,
                    DataType = appSettingDTO.DataType,
                    Module = appSettingDTO.Module
                };

                bool resCreate = await _applicationSettingsRepo.CreateApplicationSetting(applicationSettingEntity);
                if (!resCreate)
                    return ResultDTO.Fail($"Database Error while Creating App Setting with Key: {appSettingDTO.Key} and Value: {appSettingDTO.Value}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.Fail(ex.Message);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO> UpdateApplicationSetting(AppSettingDTO appSettingDTO)
        {
            try
            {
                if (appSettingDTO is null)
                    return ResultDTO.Fail("Missing App Setting Object");

                if (string.IsNullOrWhiteSpace(appSettingDTO.Value))
                    return ResultDTO.Fail("Missing App Setting Value");

                ApplicationSettings? applicationSettingEntity = 
                    await _applicationSettingsRepo.GetApplicationSettingByKey(appSettingDTO.Key);

                if (applicationSettingEntity is null)
                    return ResultDTO.Fail("Application Setting Not Found");

                // TODO: Check Cast to DataType

                // TODO: Use Mapper
                applicationSettingEntity.Value = appSettingDTO.Value;
                applicationSettingEntity.Description = string.IsNullOrWhiteSpace(appSettingDTO.Description)
                                                        ? appSettingDTO.Key
                                                        : appSettingDTO.Description;
                applicationSettingEntity.DataType = appSettingDTO.DataType;
                applicationSettingEntity.Module = appSettingDTO.Module;

                bool resUpdate = await _applicationSettingsRepo.UpdateApplicationSetting(applicationSettingEntity);
                if (!resUpdate)
                    return ResultDTO.Fail($"Database Error while Updating App Setting with key: {appSettingDTO.Key}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO> DeleteApplicationSetting(string appSettingKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appSettingKey))
                    return ResultDTO.Fail("Missing App Setting Value");

                bool? resDelete = await _applicationSettingsRepo.DeleteApplicationSettingByKey(appSettingKey);

                if (!resDelete.HasValue)
                    return ResultDTO.Fail("Application Setting Not Found");

                if (!resDelete.Value)
                    return ResultDTO.Fail($"Database Error while Deleting App Setting with key: {appSettingKey}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.Fail(ex.Message);
            }
        }
        #endregion

        #region Read 
        public async Task<List<AppSettingDTO>?> GetAllApplicationSettingsAsList()
        {
            try
            {
                var appSettings = await _applicationSettingsRepo.GetAllApplicationSettingsAsList();

                List<AppSettingDTO> appSettingDTOs = appSettings.Select(x => new AppSettingDTO()
                {
                    Key = x.Key,
                    Value = x.Value,
                    Description = x.Description,
                    DataType = x.DataType,
                    Module = x.Module
                }).ToList();

                return appSettingDTOs;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<IQueryable<AppSettingDTO>?> GetAllApplicationSettingsAsQueryable()
        {
            try
            {
                IQueryable<ApplicationSettings>? appSettingsAsQuery = 
                    await _applicationSettingsRepo.GetAllApplicationSettingsAsQueryable();

                if(appSettingsAsQuery is null)
                    return null;

                IQueryable<AppSettingDTO> appSettingsAsDtosAsQuery = appSettingsAsQuery.Select(x => new AppSettingDTO()
                {
                    Key = x.Key,
                    Value = x.Value,
                    Description = x.Description,
                    DataType = x.DataType,
                    Module = x.Module
                });

                return appSettingsAsDtosAsQuery;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<List<string>?> GetAllApplicationSettingsKeysAsList()
        {
            try
            {
                return await _applicationSettingsRepo.GetAllApplicationSettingsKeysAsList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<AppSettingDTO?> GetApplicationSettingByKey(string appSettingKey)
        {
            try
            {
                ApplicationSettings? appSetting = await _applicationSettingsRepo.GetApplicationSettingByKey(appSettingKey);

                if (appSetting == null) 
                    return null;

                AppSettingDTO appSettingAsDto = new AppSettingDTO()
                {
                    Key = appSetting.Key,
                    Value = appSetting.Value,
                    Description = appSetting.Description,
                    DataType = appSetting.DataType,
                    Module = appSetting.Module
                };

                return appSettingAsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        #endregion

    }
}
