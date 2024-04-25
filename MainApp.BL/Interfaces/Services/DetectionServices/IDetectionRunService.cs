using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using SD;

namespace MainApp.BL.Interfaces.Services.DetectionServices
{
    public interface IDetectionRunService
    {
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRuns();
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRunsIncludingDetectedDumpSites();
        Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid id, bool track = false);

        Task<ResultDTO> CreateDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO> StartDetectionRun(DetectionRunDTO detectionRunDTO);
        Task<ResultDTO<(string visualizedFilePath, string bboxesFilePath)>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId, string filePath);

        Task<ResultDTO<DetectionRunFinishedResponse>> GetBBoxResultsDeserialized(string absBBoxResultsFilePath);
    }
}
