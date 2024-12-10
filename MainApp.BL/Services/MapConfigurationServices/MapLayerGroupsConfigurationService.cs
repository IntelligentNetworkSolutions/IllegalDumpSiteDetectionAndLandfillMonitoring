using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapLayerGroupsConfigurationService : IMapLayerGroupsConfigurationService
    {
        private readonly IMapLayerGroupsConfigurationRepository _mapLayerGroupsConfigRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MapLayersConfigurationService> _logger;
        public MapLayerGroupsConfigurationService(IMapLayerGroupsConfigurationRepository mapLayerGroupsConfigRepository, IMapper mapper, ILogger<MapLayersConfigurationService> logger)
        {
            _mapLayerGroupsConfigRepository = mapLayerGroupsConfigRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Read
        #region Get MapConfigLayers

        public async Task<ResultDTO<List<MapLayerGroupConfigurationDTO>>> GetAllMapLayerGroupConfigurations()
        {
            try
            {
                ResultDTO<IEnumerable<MapLayerGroupConfiguration>>? resultGetAllEntities = await _mapLayerGroupsConfigRepository.GetAll(includeProperties: "MapLayerConfigurations");
                if (resultGetAllEntities.IsSuccess == false && resultGetAllEntities.HandleError())                
                    return ResultDTO<List<MapLayerGroupConfigurationDTO>>.Fail(resultGetAllEntities.ErrMsg!);
                if (resultGetAllEntities.Data == null)
                    return ResultDTO<List<MapLayerGroupConfigurationDTO>>.Fail("Map layer group configuration list not found");

                List<MapLayerGroupConfigurationDTO>? dtos = _mapper.Map<List<MapLayerGroupConfigurationDTO>>(resultGetAllEntities.Data);
                if (dtos == null)
                    return ResultDTO<List<MapLayerGroupConfigurationDTO>>.Fail("Mapping map layer group list failed");

                return ResultDTO<List<MapLayerGroupConfigurationDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<MapLayerGroupConfigurationDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetAllGroupLayersById(Guid mapLayerGroupConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerGroupConfiguration?> resultGetEntity = await _mapLayerGroupsConfigRepository.GetById(mapLayerGroupConfigurationId, includeProperties: "MapLayerConfigurations");
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())                
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Map layer group configuration not found");

                MapLayerGroupConfigurationDTO? dto = _mapper.Map<MapLayerGroupConfigurationDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Mapping map layer group failed");

                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetMapLayerGroupConfigurationById(Guid mapLayerGroupConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerGroupConfiguration?>? resultGetEntity = await _mapLayerGroupsConfigRepository.GetById(mapLayerGroupConfigurationId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Map layer group configuration not found");

                MapLayerGroupConfigurationDTO? dto = _mapper.Map<MapLayerGroupConfigurationDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Mapping map layer group failed");

                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }

        }
        #endregion
        #endregion

        #region Create
        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> CreateMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO)
        {
            try
            {
                MapLayerGroupConfiguration? entity = _mapper.Map<MapLayerGroupConfiguration>(mapLayerGroupConfigurationDTO);
                if (entity == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Mapping map layer group failed");

                ResultDTO<MapLayerGroupConfiguration>? resultCreate = await _mapLayerGroupsConfigRepository.CreateAndReturnEntity(entity);
                if (!resultCreate.IsSuccess && resultCreate.HandleError())                
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultCreate.ErrMsg!);
                if (resultCreate.Data == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Map layer group configuration not found");

                MapLayerGroupConfigurationDTO? returnEntity = _mapper.Map<MapLayerGroupConfigurationDTO>(resultCreate.Data);
                if (returnEntity == null)
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Mapping map layer group failed");

                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(returnEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO> EditMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO)
        {
            try
            {
                MapLayerGroupConfiguration? mapConfigurationEntity = _mapper.Map<MapLayerGroupConfiguration>(mapLayerGroupConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map layer group failed");

                ResultDTO? resultCreate = await _mapLayerGroupsConfigRepository.Update(mapConfigurationEntity);
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
        public async Task<ResultDTO> DeleteMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO)
        {
            try
            {
                MapLayerGroupConfiguration? mapConfigurationEntity = _mapper.Map<MapLayerGroupConfiguration>(mapLayerGroupConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map layer group failed");

                ResultDTO? resultCreate = await _mapLayerGroupsConfigRepository.Delete(mapConfigurationEntity);
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
