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

                List<DetectionIgnoreZoneDTO> dtos = _mapper.Map<List<DetectionIgnoreZoneDTO>>(resultGetAllEntites.Data);

                return ResultDTO<List<DetectionIgnoreZoneDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionIgnoreZoneDTO>>.ExceptionFail(ex.Message, ex);
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

                ResultDTO resultCreate =
                    await _detectionIgnoreZonesRepository.Create(ignoreZone);
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

                ResultDTO resultDelete = await _detectionIgnoreZonesRepository.Delete(resultGetById.Data!);
                if (resultDelete.IsSuccess == false && resultDelete.HandleError())
                    return ResultDTO.Fail(resultDelete.ErrMsg!);

                return resultDelete;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
