using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
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


        #region Read
        #region Get DetectionRun/s
        public async Task<List<HistoricDataLayerDTO>> GetDetectionRunsWithClasses()
        {
            var list = await _detectionRunRepository.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
            var groupedDumpSites = list.Select(detectionRun => new
            {
                DetectionRun = detectionRun,
                GroupedDumpSites = detectionRun.DetectedDumpSites
            .GroupBy(dumpSite => dumpSite.DatasetClass.ClassName)
            .ToDictionary(group => group.Key, group => group.ToList())
            }).ToList();

            List<HistoricDataLayerDTO> listDTO = new();
            foreach ( var group in groupedDumpSites)
            {
                HistoricDataLayerDTO model = new();
                model.DetectionRunId = group.DetectionRun?.Id;
                model.DetectionRunName = group.DetectionRun?.Name;
                model.DetectionRunDescription = group.DetectionRun?.Description;
                model.CreatedBy = group.DetectionRun?.CreatedBy?.UserName;
                model.CreatedOn = group.DetectionRun?.CreatedOn;
                model.IsCompleted = group.DetectionRun?.IsCompleted;
                model.GroupedDumpSitesList = new();
                model.AllConfidenceRates = new();
                foreach (var item in group.GroupedDumpSites)
                {
                    GroupedDumpSitesListHistoricDataDTO dumpSiteModel = new();
                    
                    dumpSiteModel.ClassName = item.Key;                    
                    dumpSiteModel.Geoms = new();
                    dumpSiteModel.GeomAreas = new();
                    foreach (var i in item.Value)
                    {
                        dumpSiteModel.Geoms.Add(i.Geom);                        
                        dumpSiteModel.GeomAreas.Add(i.Geom.Area);
                        model.AllConfidenceRates.Add(i.ConfidenceRate);
                    }
                    dumpSiteModel.TotalGroupArea = dumpSiteModel.GeomAreas.Sum();
                    model.GroupedDumpSitesList.Add(dumpSiteModel);
                    model.TotalAreaOfDetectionRun = model.GroupedDumpSitesList.Sum(x => x.TotalGroupArea);
                    model.AvgConfidenceRate = model.AllConfidenceRates.Average();
                }
                listDTO.Add(model);
            }
            return listDTO;
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
        #endregion
        #endregion

        #region Create
        #endregion

        #region Update

        #endregion

        #region Delete
        #endregion

      
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
