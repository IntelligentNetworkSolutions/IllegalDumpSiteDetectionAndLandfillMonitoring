using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapLayersConfigurationService : IMapLayersConfigurationService
    {
        private readonly IMapLayersConfigurationRepository _mapLayersConfigRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MapLayersConfigurationService> _logger;
        public MapLayersConfigurationService(IMapLayersConfigurationRepository mapLayersConfigRepository, IMapper mapper, ILogger<MapLayersConfigurationService> logger)
        {
            _mapLayersConfigRepository = mapLayersConfigRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #region GetMapLayerConfigs
        public async Task<ResultDTO<List<MapLayerConfigurationDTO>>> GetAllMapLayerConfigurations()
        {
            try
            {
                ResultDTO<IEnumerable<MapLayerConfiguration>>? resultGetAllEntities = await _mapLayersConfigRepository.GetAll();
                if (resultGetAllEntities.IsSuccess == false && resultGetAllEntities.HandleError())                
                    return ResultDTO<List<MapLayerConfigurationDTO>>.Fail(resultGetAllEntities.ErrMsg!);
                if(resultGetAllEntities.Data == null)
                    return ResultDTO<List<MapLayerConfigurationDTO>>.Fail("Map layer configuration not found");

                List<MapLayerConfigurationDTO>? dtos = _mapper.Map<List<MapLayerConfigurationDTO>>(resultGetAllEntities.Data);
                if(dtos == null)
                    return ResultDTO<List<MapLayerConfigurationDTO>>.Fail("Mapping map layer configuration list failed");

                return ResultDTO<List<MapLayerConfigurationDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<MapLayerConfigurationDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<MapLayerConfigurationDTO>> GetMapLayerConfigurationById(Guid mapLayerConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerConfiguration?>? resultGetEntity = await _mapLayersConfigRepository.GetById(mapLayerConfigurationId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                if( resultGetEntity.Data == null)
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Map layer configuration not found");

                MapLayerConfigurationDTO? dto = _mapper.Map<MapLayerConfigurationDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Mapping map layer configuration failed");

                return ResultDTO<MapLayerConfigurationDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultDTO<MapLayerConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }

        }
        #endregion

        #region Create
        public async Task<ResultDTO<MapLayerConfigurationDTO>> CreateMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO)
        {
            try
            {
                MapLayerConfiguration? entity = _mapper.Map<MapLayerConfiguration>(mapLayerConfigurationDTO);
                if (entity == null)
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Mapping map layer configuration failed");

                ResultDTO<MapLayerConfiguration>? resultCreate = await _mapLayersConfigRepository.CreateAndReturnEntity(entity);
                if (!resultCreate.IsSuccess && resultCreate.HandleError())                
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(resultCreate.ErrMsg!);
                if (resultCreate.Data == null)
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Map layer configuration not found");

                MapLayerConfigurationDTO? returnEntity = _mapper.Map<MapLayerConfigurationDTO>(resultCreate.Data);
                if (returnEntity == null)
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Mapping map layer configuration failed");

                return ResultDTO<MapLayerConfigurationDTO>.Ok(returnEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<MapLayerConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO> EditMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO)
        {
            try
            {
                MapLayerConfiguration? mapConfigurationEntity = _mapper.Map<MapLayerConfiguration>(mapLayerConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map layer configuration failed");

                ResultDTO? resultCreate = await _mapLayersConfigRepository.Update(mapConfigurationEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                
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
        public async Task<ResultDTO> DeleteMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO)
        {
            try
            {
                MapLayerConfiguration? mapConfigurationEntity = _mapper.Map<MapLayerConfiguration>(mapLayerConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map layer configuration failed");

                ResultDTO? resultCreate = await _mapLayersConfigRepository.Delete(mapConfigurationEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }


        #endregion
    }
}
