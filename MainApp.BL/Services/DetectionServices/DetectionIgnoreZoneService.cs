using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.DetectionServices
{
    public class DetectionIgnoreZoneService : IDetectionIgnoreZoneService
    {
        private readonly IDetectionIgnoreZonesRepository _detectionIgnoreZonesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DetectionIgnoreZoneService> _logger;

        public DetectionIgnoreZoneService(IDetectionIgnoreZonesRepository detectionIgnoreZonesRepository, IMapper mapper, ILogger<DetectionIgnoreZoneService> logger)
        {
            _detectionIgnoreZonesRepository = detectionIgnoreZonesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDTO<List<DetectionIgnoreZoneDTO>>> GetAllIgnoreZonesDTOs()
        {
            try
            {
                ResultDTO<IEnumerable<DetectionIgnoreZone>> resultGetAllEntites =
                    await _detectionIgnoreZonesRepository.GetAll(includeProperties: "CreatedBy");

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                    return ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail("Detection ignore zone list not found");

                List<DetectionIgnoreZoneDTO>? dtos = _mapper.Map<List<DetectionIgnoreZoneDTO>>(resultGetAllEntites.Data);
                if (dtos == null)
                    return ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail("Mapping deteciton ignore zoen list failed");

                return ResultDTO<List<DetectionIgnoreZoneDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionIgnoreZoneDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<DetectionIgnoreZoneDTO?>> GetIgnoreZoneById(Guid id)
        {
            try
            {
                ResultDTO<DetectionIgnoreZone?> resultGetEntity = await _detectionIgnoreZonesRepository.GetById(id, includeProperties: "CreatedBy");
                if(resultGetEntity.IsSuccess == false && !resultGetEntity.HandleError())                
                    return ResultDTO<DetectionIgnoreZoneDTO?>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<DetectionIgnoreZoneDTO?>.Fail("Detection ignore zone not found");
                
                DetectionIgnoreZoneDTO dto = _mapper.Map<DetectionIgnoreZoneDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<DetectionIgnoreZoneDTO?>.Fail("Mapping detection ingore zone failed");

                return ResultDTO<DetectionIgnoreZoneDTO?>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<DetectionIgnoreZoneDTO?>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> CreateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto)
        {
            try
            {
                if (dto is null)
                    return ResultDTO.Fail("DTO Object is null");

                DetectionIgnoreZone ignoreZone = _mapper.Map<DetectionIgnoreZone>(dto);
                if (ignoreZone is null)
                    return ResultDTO.Fail("DTO not mapped");

                ResultDTO resultCreate = await _detectionIgnoreZonesRepository.Create(ignoreZone);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                    return ResultDTO.Fail(resultCreate.ErrMsg!);

                return resultCreate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> UpdateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto)
        {
            try
            {
                if (dto is null)
                    return ResultDTO.Fail("DTO Object is null");

                if (dto.Id is null)
                    return ResultDTO.Fail("DTO Id is null");

                DetectionIgnoreZone ignoreZone = _mapper.Map<DetectionIgnoreZone>(dto);
                if (ignoreZone is null)
                    return ResultDTO.Fail("DTO not mapped");

                ResultDTO<DetectionIgnoreZone?> resultGetById =
                    await _detectionIgnoreZonesRepository.GetById(ignoreZone.Id, track: true);
                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO.Fail(resultGetById.ErrMsg!);
                if (resultGetById.Data == null)
                    return ResultDTO.Fail("Detection ingore zone not found");
                   
                if(dto.Geom == null)
                {
                    dto.Geom = resultGetById.Data?.Geom;
                }
                dto.CreatedOn = resultGetById.Data?.CreatedOn;
                dto.CreatedById = resultGetById.Data?.CreatedById;
                _mapper.Map(dto, resultGetById.Data);               

                ResultDTO resultUpdate = await _detectionIgnoreZonesRepository.Update(resultGetById.Data!);
                if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
                    return ResultDTO.Fail(resultUpdate.ErrMsg!);

                return resultUpdate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto)
        {
            try
            {
                if (dto is null)
                    return ResultDTO.Fail("DTO Object is null");

                if (dto.Id is null)
                    return ResultDTO.Fail("DTO Id is null");

                DetectionIgnoreZone ignoreZone = _mapper.Map<DetectionIgnoreZone>(dto);
                if (ignoreZone is null)
                    return ResultDTO.Fail("DTO not mapped");

                ResultDTO<DetectionIgnoreZone?> resultGetById =
                    await _detectionIgnoreZonesRepository.GetById(ignoreZone.Id, track: true);
                if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                    return ResultDTO.Fail(resultGetById.ErrMsg!);
                if (resultGetById.Data == null)
                    return ResultDTO.Fail("Detection ingore zone not found");

                ResultDTO resultDelete = await _detectionIgnoreZonesRepository.Delete(resultGetById.Data!);
                if (resultDelete.IsSuccess == false && resultDelete.HandleError())
                    return ResultDTO.Fail(resultDelete.ErrMsg!);

                return resultDelete;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
    }
}

