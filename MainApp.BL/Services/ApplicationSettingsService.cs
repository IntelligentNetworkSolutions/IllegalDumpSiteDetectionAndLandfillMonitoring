using AutoMapper;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using Entities;
using MainApp.BL.Interfaces.Services;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly IApplicationSettingsRepo _applicationSettingsRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationSettingsService> _logger;

        public ApplicationSettingsService(IApplicationSettingsRepo applicationSettingsRepo, ILogger<ApplicationSettingsService> logger, IMapper mapper)
        {
            _applicationSettingsRepo = applicationSettingsRepo;
            _logger = logger;
            _mapper = mapper;
        }

        #region Create
        public async Task<ResultDTO> CreateApplicationSetting(AppSettingDTO? appSettingDTO)
        {
            try
            {
                if (appSettingDTO is null)
                    return ResultDTO.Fail("Missing App Setting Object");

                if (string.IsNullOrWhiteSpace(appSettingDTO.Key))
                    return ResultDTO.Fail("Missing App Setting Key");

                if (string.IsNullOrWhiteSpace(appSettingDTO.Value))
                    return ResultDTO.Fail("Missing App Setting Value");

                // TODO: Check Cast to DataType

                if (string.IsNullOrEmpty(appSettingDTO.Description))
                    appSettingDTO.Description = appSettingDTO.Key;

                ApplicationSettings appSettingEnt = _mapper.Map<ApplicationSettings>(appSettingDTO);
                if (appSettingEnt is null)
                    return ResultDTO.Fail("Mapping Failed");

                bool resCreate = await _applicationSettingsRepo.CreateApplicationSetting(appSettingEnt);
                if (!resCreate)
                    return ResultDTO.Fail($"Database Error while Creating App Setting with Key: {appSettingDTO.Key} and Value: {appSettingDTO.Value}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
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

                if (string.IsNullOrWhiteSpace(appSettingDTO.Key))
                    return ResultDTO.Fail("Missing App Setting Key");

                if (string.IsNullOrWhiteSpace(appSettingDTO.Value))
                    return ResultDTO.Fail("Missing App Setting Value");

                ApplicationSettings? applicationSettingEntity = 
                    await _applicationSettingsRepo.GetApplicationSettingByKey(appSettingDTO.Key);

                if (applicationSettingEntity is null)
                    return ResultDTO.Fail("Application Setting Not Found");

                // TODO: Check Cast to DataType

                if (string.IsNullOrEmpty(appSettingDTO.Description))
                    appSettingDTO.Description = appSettingDTO.Key;

                //applicationSettingEntity = _mapper.Map<ApplicationSettings>(appSettingDTO);
                _mapper.Map(appSettingDTO, applicationSettingEntity);

                if (applicationSettingEntity is null || string.IsNullOrEmpty(applicationSettingEntity.Key))
                    return ResultDTO.Fail("Mapping Failed");

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
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Read 
        public async Task<List<AppSettingDTO>?> GetAllApplicationSettingsAsList()
        {
            try
            {
                List<ApplicationSettings> appSettings = await _applicationSettingsRepo.GetAllApplicationSettingsAsList();
                List<AppSettingDTO> appSettingDTOs = _mapper.Map<List<AppSettingDTO>>(appSettings);
                return appSettingDTOs;
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

                AppSettingDTO appSettingAsDto = _mapper.Map<AppSettingDTO>(appSetting);
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
