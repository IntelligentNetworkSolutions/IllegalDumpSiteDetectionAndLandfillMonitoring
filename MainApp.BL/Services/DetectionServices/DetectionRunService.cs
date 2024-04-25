using System.Text.Json;
using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.DetectionServices
{
    public class DetectionRunService : IDetectionRunService
    {
        private readonly IDetectionRunsRepository _detectionRunRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DetectionRunService> _logger;

        private const string absoluteCondaExePath = "C:\\ProgramData\\anaconda3\\_conda.exe";
        private const string absoluteCliLogsDirectoryPath = "C:\\Logs\\WasteDetectionPlatform";

        private const string relativeOutputBaseDirectoryMMDetection = "ins_development\\detections_outputs";

        private const string bodanWorkingMMDetectionDirectory = "C:\\vs_code_workspaces\\mmdetection\\mmdetection";

        private const string relativeTrainedModelConfigPath = "ins_development\\trained_models\\igor\\home_48\\faster_rcnn_r50_fpn_2x_coco_add300dataset.py";
        private const string relativeTrainedModelModelPath = "ins_development\\trained_models\\igor\\home_48\\epoch_48.pth";


        public DetectionRunService(IDetectionRunsRepository detectionRunRepository, IMapper mapper, ILogger<DetectionRunService> logger)
        {
            _detectionRunRepository = detectionRunRepository;
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

        private string GeneratePythonDetectionCommandByType(string imageToRunDetectionOnPath,
            string trainedModelConfigPath, string trainedModelModelPath, bool isSmallImage = true, bool hasGPU = false)
        {
            string detectionCommandStr = string.Empty;
            //string scriptName = isSmallImage ? "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\demo\\image_demo.py" : "C:\\vs_code_workspaces\\mmdetection\\mmdetection\\demo\\large_image_demo.py";
            string scriptName = isSmallImage ? "demo\\image_demo.py" : "demo\\large_image_demo.py";
            string weightsCommandParamStr = isSmallImage ? "--weights" : string.Empty;
            string cpuDetectionCommandPart = hasGPU == false ? "--device cpu" : string.Empty;
            
            string relativeOutputImageDirectoryMMDetection = Path.Combine(relativeOutputBaseDirectoryMMDetection, Path.GetFileNameWithoutExtension(imageToRunDetectionOnPath));
            string outDirCommandPart = $"--out-dir {relativeOutputImageDirectoryMMDetection}"; 

            detectionCommandStr = $"run -n openmmlab python {scriptName} \"{imageToRunDetectionOnPath}\" " +
                $"{trainedModelConfigPath} {weightsCommandParamStr} {trainedModelModelPath} {outDirCommandPart} {cpuDetectionCommandPart}";

            return detectionCommandStr;
        }

        public async Task<ResultDTO> CreateDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                DetectionRun detectionRunEntity = _mapper.Map<DetectionRun>(detectionRunDTO);

                ResultDTO<DetectionRun> resultCreate = await _detectionRunRepository.CreateAndReturnEntity(detectionRunEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                    return ResultDTO.Fail(resultCreate.ErrMsg!);

                if(resultCreate.Data is null)
                    return ResultDTO.Fail("Error Creating Detection Run");

                return ResultDTO.Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> StartDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                if (detectionRunDTO is null)
                    return ResultDTO.Fail("");

                string detectionCommand = 
                    GeneratePythonDetectionCommandByType(
                        imageToRunDetectionOnPath: detectionRunDTO.ImagePath, 
                        trainedModelConfigPath: relativeTrainedModelConfigPath, 
                        trainedModelModelPath: relativeTrainedModelModelPath, 
                        isSmallImage: true, hasGPU: false);

                var powerShellResults = await Cli.Wrap(absoluteCondaExePath)
                        .WithWorkingDirectory(bodanWorkingMMDetectionDirectory)
                        .WithValidation(CommandResultValidation.None)
                        .WithArguments(detectionCommand.ToLower())
                        .WithStandardOutputPipe(PipeTarget.ToFile(Path.Combine(absoluteCliLogsDirectoryPath,$"succ_{DateTime.Now.Ticks}.txt")))
                        .WithStandardErrorPipe(PipeTarget.ToFile(Path.Combine(absoluteCliLogsDirectoryPath, $"error_{DateTime.Now.Ticks}.txt")))
                        .ExecuteBufferedAsync();

                if (powerShellResults.StandardOutput.Contains("results have been saved") == false)
                    return ResultDTO.Fail($"Detection Run failed with error: {powerShellResults.StandardError}");

                return ResultDTO.Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<(string visualizedFilePath, string bboxesFilePath)>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId, string imgFileExtension)
        {
            string relDetectionRunResultsDirPath = 
                Path.Combine(relativeOutputBaseDirectoryMMDetection, detectionRunId.ToString());

            string absDetectionRunResultsDirPath = 
                Path.Combine(bodanWorkingMMDetectionDirectory, relDetectionRunResultsDirPath);

            string absDetectionRunResultsVizualizedDirPath = 
                Path.Combine(absDetectionRunResultsDirPath, "vis");
            string absDetectionRunResultsBBoxesDirPath = 
                Path.Combine(absDetectionRunResultsDirPath, "preds");

            string absDetectionRunResultsVizualizedFilePath =
                Path.Combine(absDetectionRunResultsVizualizedDirPath, detectionRunId.ToString() + imgFileExtension);
            string absDetectionRunResultsBBoxesFilePath =
                Path.Combine(absDetectionRunResultsBBoxesDirPath, detectionRunId.ToString() + ".json");

            if(File.Exists(absDetectionRunResultsVizualizedFilePath) == false)
                return ResultDTO<(string, string)>.Fail("No Visualized Detection Run Results Found");

            if (File.Exists(absDetectionRunResultsBBoxesFilePath) == false)
                return ResultDTO<(string, string)>.Fail("No Polygonized Predictions Detection Run Results Found");

            return await Task.FromResult(
                ResultDTO<(string, string)>.Ok((absDetectionRunResultsVizualizedFilePath, absDetectionRunResultsBBoxesFilePath))
                );
        }

        public async Task<ResultDTO<DetectionRunFinishedResponse>> GetBBoxResultsDeserialized(string absBBoxResultsFilePath)
        {
            var resultJsonDeserialization =
                await ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(absBBoxResultsFilePath);

            return resultJsonDeserialization;
        }

        public static async Task<ResultDTO<TJson>> ReadFromFileAsDeserializedJson<TJson>(string filePath) where TJson : class
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);

                if (IsValidJson(jsonContent) == false)
                    return ResultDTO<TJson>.Fail("JSON is not valid");

                TJson? response = JsonSerializer.Deserialize<TJson>(jsonContent);
                if (response is null)
                    return ResultDTO<TJson>.Fail("Failed Deserialization");

                return ResultDTO<TJson>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResultDTO<TJson>.ExceptionFail(ex.Message, ex);
            }
        }

        public static bool IsValidJson(string jsonString)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    // If it gets here, the JSON is well-formed
                    return true;
                }
            }
            catch (JsonException)
            {
                // JSON was not in a valid format
                return false;
            }
        }

    }
}
