using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.DetectionServices
{
    public class DetectionRunService : IDetectionRunService
    {
        private readonly IDetectionRunsRepository _detectionRunRepository;
        private readonly IDetectedDumpSitesRepository detectedDumpSitesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DetectionRunService> _logger;

        public DetectionRunService(IDetectionRunsRepository detectionRunRepository, IDetectedDumpSitesRepository detectedDumpSitesRepository, IMapper mapper, ILogger<DetectionRunService> logger)
        {
            _detectionRunRepository = detectionRunRepository;
            this.detectedDumpSitesRepository = detectedDumpSitesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRuns()
        {
            try
            {
                ResultDTO<IEnumerable<DetectionRun>> resultGetAllEntites =
                    await _detectionRunRepository.GetAll(includeProperties: "CreatedBy");

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                    return ResultDTO<List<DetectionRunDTO>>.Fail(resultGetAllEntites.ErrMsg!);

                List<DetectionRunDTO> dtos = _mapper.Map<List<DetectionRunDTO>>(resultGetAllEntites.Data);

                return ResultDTO<List<DetectionRunDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRunsIncludingDetectedDumpSites()
        {
            try
            {
                // DetectedDumpSites might throw error
                ResultDTO<IEnumerable<DetectionRun>> resultGetAllEntites =
                    await _detectionRunRepository.GetAll(includeProperties: "CreatedBy,DetectedDumpSites");

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                    return ResultDTO<List<DetectionRunDTO>>.Fail(resultGetAllEntites.ErrMsg!);

                List<DetectionRunDTO> dtos = _mapper.Map<List<DetectionRunDTO>>(resultGetAllEntites.Data);

                return ResultDTO<List<DetectionRunDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid id, bool track = false)
        {
            try
            {
                ResultDTO<DetectionRun?> resultGetAllEntites =
                    await _detectionRunRepository.GetById(id, track: track, includeProperties: "CreatedBy");

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                    return ResultDTO<DetectionRunDTO>.Fail(resultGetAllEntites.ErrMsg!);

                DetectionRunDTO dto = _mapper.Map<DetectionRunDTO>(resultGetAllEntites.Data);

                return ResultDTO<DetectionRunDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<DetectionRunDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        //public async Task<ResultDTO> PrepareInitialDetectionRunJson()
        //{

        //}

        //public async Task<ResultDTO> InitializeDetectionRun()
        //{
        //    await PrepareInitialDetectionRunJson();
        //}        

        //public async Task<ResultDTO> ScheduleDetectionRun()
        //{

        //}

        //public async Task<ResultDTO<DetectionRunFinishedResponse>> ReceiveDeserializedDetectionRunResponseJson()
        //{
            


        //}

        //public async Task<ResultDTO> ProcessDetectionRunResponseJson()
        //{

        //}

        //public async Task<ResultDTO> FinalizeDetectionRun()
        //{

        //}
    }
}
