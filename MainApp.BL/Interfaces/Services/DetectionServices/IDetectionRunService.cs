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
        Task<ResultDTO<List<DetectionRunDTO>>> GetSelectedDetectionRunsIncludingDetectedDumpSites(List<Guid> selectedDetectionRunsIds, List<ConfidenceRateDTO> selectedConfidenceRates);
        Task<ResultDTO<List<AreaComparisonAvgConfidenceRateReportDTO>>> GenerateAreaComparisonAvgConfidenceRateData(List<Guid> selectedDetectionRunsIds, int selectedConfidenceRate);
        Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid id, bool track = false);
        Task<ResultDTO> DeleteDetectionRun(Guid detectionRunId, string wwwrootPath);
        Task<ResultDTO> CreateDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> StartDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> IsCompleteUpdateDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> UpdateStatus(Guid detectionRunId, string status);
        Task<ResultDTO<string>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId);
        Task<ResultDTO<DetectionRunFinishedResponse>> GetBBoxResultsDeserialized(string absBBoxResultsFilePath);
        Task<ResultDTO<List<DetectionRunDTO>>> GetDetectionRunsWithClasses();
        Task<ResultDTO<DetectionRunFinishedResponse>> ConvertBBoxResultToImageProjection
            (string absoluteImagePath, DetectionRunFinishedResponse detectionRunFinishedResponse);
        Task<ResultDTO<List<DetectedDumpSite>>> CreateDetectedDumpsSitesFromDetectionRun
            (Guid detectionRunId, DetectionRunFinishedResponse detectedDumpSitesProjectedResponse);

        //Images
        Task<ResultDTO<List<DetectionInputImageDTO>>> GetAllImages();
        Task<ResultDTO<DetectionInputImageDTO>> CreateDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO> EditDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO> DeleteDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO<List<DetectionRunDTO>>> GetDetectionInputImageByDetectionRunId(Guid detectionInputImageId);
        Task<ResultDTO<DetectionInputImageDTO>> GetDetectionInputImageById(Guid detectionInputImageId);
        Task<ResultDTO<List<DetectionInputImageDTO>>> GetSelectedInputImagesById(List<Guid> selectedImagesIds);
    }
}
