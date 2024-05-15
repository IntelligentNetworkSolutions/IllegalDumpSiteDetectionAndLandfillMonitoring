using DTOs.MainApp.BL.DetectionDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using SD;

namespace MainApp.BL.Interfaces.Services.DetectionServices
{
    public interface IDetectionRunService
    {
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRuns();
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRunsIncludingDetectedDumpSites();
        Task<ResultDTO<List<DetectionRunDTO>>> GetSelectedDetectionRunsIncludingDetectedDumpSites(List<Guid> selectedDetectionRunsIds);
        Task<List<AreaComparisonAvgConfidenceRateReportDTO>> GenerateAreaComparisonAvgConfidenceRateData(List<Guid> selectedDetectionRunsIds);
        Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid id, bool track = false);
        Task<ResultDTO> CreateDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> StartDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> IsCompleteUpdateDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO<(string visualizedFilePath, string bboxesFilePath)>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId, string filePath);
        Task<ResultDTO<DetectionRunFinishedResponse>> GetBBoxResultsDeserialized(string absBBoxResultsFilePath);
        Task<List<DetectionRunDTO>> GetDetectionRunsWithClasses();
        Task<ResultDTO<DetectionRunFinishedResponse>> ConvertBBoxResultToImageProjection
            (string absoluteImagePath, DetectionRunFinishedResponse detectionRunFinishedResponse);
        Task<ResultDTO<List<DetectedDumpSite>>> CreateDetectedDumpsSitesFromDetectionRun
            (Guid detectionRunId, DetectionRunFinishedResponse detectedDumpSitesProjectedResponse);
    }
}
