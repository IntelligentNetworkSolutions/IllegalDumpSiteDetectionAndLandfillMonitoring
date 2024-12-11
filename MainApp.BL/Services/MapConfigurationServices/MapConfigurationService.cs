using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapConfigurationService : IMapConfigurationService
    {
        private readonly IMapConfigurationRepository _mapConfigRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MapConfigurationService> _logger;
        public MapConfigurationService(IMapConfigurationRepository mapConfigRepository, IMapper mapper, ILogger<MapConfigurationService> logger)
        {
            _mapConfigRepository = mapConfigRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #region Read
        #region Get Mapconfig/es
        public async Task<ResultDTO<MapConfigurationDTO>> GetMapConfigurationByName(string mapName)
        {
            try
            {
                ResultDTO<MapConfiguration>? resultGetMapConfing = await _mapConfigRepository.GetMapConfigurationByName(mapName);
                if (resultGetMapConfing.IsSuccess == false && resultGetMapConfing.HandleError())
                    return ResultDTO<MapConfigurationDTO>.Fail(resultGetMapConfing.ErrMsg!);                
                if(resultGetMapConfing.Data == null)
                    return ResultDTO<MapConfigurationDTO>.Fail("Map Configuration not found");
                
                MapConfigurationDTO? mapConfigDTOs = _mapper.Map<MapConfigurationDTO>(resultGetMapConfing.Data);
                if (mapConfigDTOs == null)
                    return ResultDTO<MapConfigurationDTO>.Fail("Mapping to map configuration dto failed");
                
                return ResultDTO<MapConfigurationDTO>.Ok(mapConfigDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<MapConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
           
        }

        public async Task<ResultDTO<List<MapConfigurationDTO>>> GetAllMapConfigurations()
        {
            try
            {
                ResultDTO<IEnumerable<MapConfiguration>> resultGetAllEntities = await _mapConfigRepository.GetAll();
                if (resultGetAllEntities.IsSuccess == false && resultGetAllEntities.HandleError())                
                    return ResultDTO<List<MapConfigurationDTO>>.Fail(resultGetAllEntities.ErrMsg!);
                if(resultGetAllEntities.Data == null)
                    return ResultDTO<List<MapConfigurationDTO>>.Fail("Map configuration not found");

                List<MapConfigurationDTO> dtos = _mapper.Map<List<MapConfigurationDTO>>(resultGetAllEntities.Data);
                if(dtos == null)
                    return ResultDTO<List<MapConfigurationDTO>>.Fail("Mapping map configuration failed");

                return ResultDTO<List<MapConfigurationDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<MapConfigurationDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<MapConfigurationDTO>> GetMapConfigurationById(Guid mapConfigurationId)
        {
            try
            {
                ResultDTO<MapConfiguration?> resultGetEntity = await _mapConfigRepository.GetById(mapConfigurationId, false, "MapLayerConfigurations, MapLayerGroupConfigurations");
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO<MapConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);  
                if (resultGetEntity.Data == null)
                    return ResultDTO<MapConfigurationDTO>.Fail("Map configuration not found");

                MapConfigurationDTO dto = _mapper.Map<MapConfigurationDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<MapConfigurationDTO>.Fail("Mapping map configuration failed");

                return ResultDTO<MapConfigurationDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultDTO<MapConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }

        }
        #endregion
        #endregion

        #region Create
        public async Task<ResultDTO> CreateMapConfiguration(MapConfigurationDTO mapConfigurationDTO)
        {
            try
            {
                MapConfiguration entity = _mapper.Map<MapConfiguration>(mapConfigurationDTO);
                if (entity == null)
                    return ResultDTO.Fail("Mapping map configuration failed");

                ResultDTO resultCreate = await _mapConfigRepository.Create(entity);
                if (!resultCreate.IsSuccess && resultCreate.HandleError())                
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

        #region Update
        public async Task<ResultDTO> EditMapConfiguration(MapConfigurationDTO mapConfigurationDTO)
        {
            try
            {
                MapConfiguration mapConfigurationEntity = _mapper.Map<MapConfiguration>(mapConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map configuration failed");

                ResultDTO resultCreate = await _mapConfigRepository.Update(mapConfigurationEntity);
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
        public async Task<ResultDTO> DeleteMapConfiguration(MapConfigurationDTO mapConfigurationDTO)
        {
            try
            {
                MapConfiguration mapConfigurationEntity = _mapper.Map<MapConfiguration>(mapConfigurationDTO);
                if (mapConfigurationEntity == null)
                    return ResultDTO.Fail("Mapping map configuration failed");

                ResultDTO resultCreate = await _mapConfigRepository.Delete(mapConfigurationEntity);
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
