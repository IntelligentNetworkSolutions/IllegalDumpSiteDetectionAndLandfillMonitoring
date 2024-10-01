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

        //Images
        Task<ResultDTO<List<DetectionInputImageDTO>>> GetAllImages();
        Task<ResultDTO> CreateDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO> EditDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO> DeleteDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO);
        Task<ResultDTO<List<DetectionRunDTO>>> GetDetectionInputImageByDetectionRunId(Guid detectionInputImageId);
        Task<ResultDTO<DetectionInputImageDTO>> GetDetectionInputImageById(Guid detectionInputImageId);
        Task<ResultDTO<List<DetectionInputImageDTO>>> GetSelectedInputImagesById(List<Guid> selectedImagesIds);
    }
}
