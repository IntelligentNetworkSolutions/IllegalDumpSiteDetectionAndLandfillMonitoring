using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSGeo.GDAL;
using SD;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Presentation;
using DTOs.MainApp.BL.DatasetDTOs;

namespace MainApp.BL.Services.DetectionServices
{
    public class DetectionRunService : IDetectionRunService
    {
        private readonly IDetectionRunsRepository _detectionRunRepository;
        private readonly IDetectedDumpSitesRepository _detectedDumpSitesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DetectionRunService> _logger;
        private readonly IConfiguration _configuration;

        // TODO: Think Refactoring
        private string CondaExeFileAbsPath = string.Empty;
        private string CliLogsDirectoryAbsPath = string.Empty;

        private string OutputBaseDirectoryMMDetectionRelPath = string.Empty;

        private string WorkingMMDetectionDirectoryAbsPath = string.Empty;

        private string TrainedModelConfigFileRelPath = string.Empty;
        private string TrainedModelModelFileRelPath = string.Empty;

        private string DetectionResultDummyDatasetClassId = string.Empty;

        public DetectionRunService(IDetectionRunsRepository detectionRunRepository, IMapper mapper, ILogger<DetectionRunService> logger, IConfiguration configuration, IDetectedDumpSitesRepository detectedDumpSitesRepository)
        {
            _detectionRunRepository = detectionRunRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;

            string? condaExeFileAbsPath = _configuration["AppSettings:MMDetection:CondaExeFileAbsPath"];
            if (string.IsNullOrEmpty(condaExeFileAbsPath))
                throw new Exception($"{nameof(condaExeFileAbsPath)} is missing");
            string? cliLogsDirectoryAbsPath = _configuration["AppSettings:MMDetection:CliLogsDirectoryAbsPath"];
            if (string.IsNullOrEmpty(cliLogsDirectoryAbsPath))
                throw new Exception($"{nameof(cliLogsDirectoryAbsPath)} is missing");
            string? outputBaseDirectoryMMDetectionRelPath = _configuration["AppSettings:MMDetection:OutputBaseDirectoryMMDetectionRelPath"];
            if (string.IsNullOrEmpty(outputBaseDirectoryMMDetectionRelPath))
                throw new Exception($"{nameof(outputBaseDirectoryMMDetectionRelPath)} is missing");
            string? workingMMDetectionDirectoryAbsPath = _configuration["AppSettings:MMDetection:WorkingMMDetectionDirectoryAbsPath"];
            if (string.IsNullOrEmpty(workingMMDetectionDirectoryAbsPath))
                throw new Exception($"{nameof(workingMMDetectionDirectoryAbsPath)} is missing");
            string? trainedModelConfigFileRelPath = _configuration["AppSettings:MMDetection:TrainedModelConfigFileRelPath"];
            if (string.IsNullOrEmpty(trainedModelConfigFileRelPath))
                throw new Exception($"{nameof(trainedModelConfigFileRelPath)} is missing");
            string? trainedModelModelFileRelPath = _configuration["AppSettings:MMDetection:TrainedModelModelFileRelPath"];
            if (string.IsNullOrEmpty(trainedModelModelFileRelPath))
                throw new Exception($"{nameof(trainedModelModelFileRelPath)} is missing");

            string? detectionResultDummyDatasetClassId = _configuration["AppSettings:MMDetection:DetectionResultDummyDatasetClassId"];
            if (string.IsNullOrEmpty(detectionResultDummyDatasetClassId))
                throw new Exception($"{nameof(detectionResultDummyDatasetClassId)} is missing");

            CondaExeFileAbsPath = condaExeFileAbsPath;
            CliLogsDirectoryAbsPath = cliLogsDirectoryAbsPath;
            OutputBaseDirectoryMMDetectionRelPath = outputBaseDirectoryMMDetectionRelPath;
            WorkingMMDetectionDirectoryAbsPath = workingMMDetectionDirectoryAbsPath;
            TrainedModelConfigFileRelPath = trainedModelConfigFileRelPath;
            TrainedModelModelFileRelPath = trainedModelModelFileRelPath;
            _detectedDumpSitesRepository = detectedDumpSitesRepository;

            DetectionResultDummyDatasetClassId = detectionResultDummyDatasetClassId;
        }

        public async Task<List<HistoricDataLayerDTO>> GetDetectionRunsWithClassesHistoricDataLayer()
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

        public async Task<List<DetectionRunDTO>> GetDetectionRunsWithClasses()
        {
            try
            {
                var detectionRuns = await _detectionRunRepository.GetDetectionRunsWithClasses() ?? throw new Exception("Object not found");
                var listDetectionRunsDTOs = _mapper.Map<List<DetectionRunDTO>>(detectionRuns) ?? throw new Exception("Object not found");
                return listDetectionRunsDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
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

            string relativeOutputImageDirectoryMMDetection = Path.Combine(OutputBaseDirectoryMMDetectionRelPath, Path.GetFileNameWithoutExtension(imageToRunDetectionOnPath));
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

                if (resultCreate.Data is null)
                    return ResultDTO.Fail("Error Creating Detection Run");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> IsCompleteUpdateDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                ResultDTO<DetectionRun?> resultGetEntity = await _detectionRunRepository.GetById(detectionRunDTO.Id.Value, track: true);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if(resultGetEntity.Data is null)
                    return ResultDTO.Fail($"No Detection Run found with ID: {detectionRunDTO.Id}");

                DetectionRun detectionRunEntity = resultGetEntity.Data!;
                detectionRunEntity.IsCompleted = true;
                
                ResultDTO resultUpdate = await _detectionRunRepository.Update(detectionRunEntity);
                if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
                    return ResultDTO.Fail(resultUpdate.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
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
                        trainedModelConfigPath: TrainedModelConfigFileRelPath,
                        trainedModelModelPath: TrainedModelModelFileRelPath,
                        isSmallImage: true, hasGPU: false);

                var powerShellResults = await Cli.Wrap(CondaExeFileAbsPath)
                        .WithWorkingDirectory(WorkingMMDetectionDirectoryAbsPath)
                        .WithValidation(CommandResultValidation.None)
                        .WithArguments(detectionCommand.ToLower())
                        .WithStandardOutputPipe(PipeTarget.ToFile(Path.Combine(CliLogsDirectoryAbsPath, $"succ_{DateTime.Now.Ticks}.txt")))
                        .WithStandardErrorPipe(PipeTarget.ToFile(Path.Combine(CliLogsDirectoryAbsPath, $"error_{DateTime.Now.Ticks}.txt")))
                        .ExecuteBufferedAsync();

                if (powerShellResults.StandardOutput.Contains("results have been saved") == false)
                    return ResultDTO.Fail($"Detection Run failed with error: {powerShellResults.StandardError}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<(string visualizedFilePath, string bboxesFilePath)>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId, string imgFileExtension)
        {
            string relDetectionRunResultsDirPath =
                Path.Combine(OutputBaseDirectoryMMDetectionRelPath, detectionRunId.ToString());

            string absDetectionRunResultsDirPath =
                Path.Combine(WorkingMMDetectionDirectoryAbsPath, relDetectionRunResultsDirPath);

            string absDetectionRunResultsVizualizedDirPath =
                Path.Combine(absDetectionRunResultsDirPath, "vis");
            string absDetectionRunResultsBBoxesDirPath =
                Path.Combine(absDetectionRunResultsDirPath, "preds");

            string absDetectionRunResultsVizualizedFilePath =
                Path.Combine(absDetectionRunResultsVizualizedDirPath, detectionRunId.ToString() + imgFileExtension);
            string absDetectionRunResultsBBoxesFilePath =
                Path.Combine(absDetectionRunResultsBBoxesDirPath, detectionRunId.ToString() + ".json");

            if (File.Exists(absDetectionRunResultsVizualizedFilePath) == false)
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
                await NewtonsoftJsonHelper.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(absBBoxResultsFilePath);

            return resultJsonDeserialization;
        }

        public async Task<ResultDTO<DetectionRunFinishedResponse>> ConvertBBoxResultToImageProjection
            (string absoluteImagePath, DetectionRunFinishedResponse detectionRunFinishedResponse)
        {
            if (string.IsNullOrEmpty(absoluteImagePath))
                return ResultDTO<DetectionRunFinishedResponse>.Fail($"{nameof(absoluteImagePath)} is null or empty");

            if(detectionRunFinishedResponse is null)
                return ResultDTO<DetectionRunFinishedResponse>.Fail($"{nameof(detectionRunFinishedResponse)} is null");

            if (detectionRunFinishedResponse.bboxes is null || detectionRunFinishedResponse.bboxes.Length == 0)
                return ResultDTO<DetectionRunFinishedResponse>.Fail($"{nameof(detectionRunFinishedResponse.bboxes)} is null");

            OSGeo.GDAL.Dataset? gdalDataset = null;
            try
            {
                Gdal.AllRegister();  // This registers all GDAL drivers
                // Initialize GDAL
                GdalConfiguration.ConfigureGdal();

                // Open the image using GDAL
                gdalDataset = OSGeo.GDAL.Gdal.Open(absoluteImagePath, OSGeo.GDAL.Access.GA_ReadOnly);

                // Get the extent of the image
                double[] geoTransform = new double[6];
                gdalDataset.GetGeoTransform(geoTransform);
                double ulx = geoTransform[0];
                double xres = geoTransform[1];
                double uly = geoTransform[3];
                double yres = geoTransform[5];
                double lrx = ulx + (gdalDataset.RasterXSize * xres);
                double lry = uly + (gdalDataset.RasterYSize * yres);

                // Georeference the data
                foreach (var bbox in detectionRunFinishedResponse.bboxes)
                {
                    bbox[0] = ulx + (bbox[0] * xres);
                    bbox[1] = uly + (bbox[1] * yres);
                    bbox[2] = ulx + (bbox[2] * xres);
                    bbox[3] = uly + (bbox[3] * yres);
                }

                // Create and return a new DetectionRunFinishedResponse object with the updated bboxes
                DetectionRunFinishedResponse updatedResponse = new DetectionRunFinishedResponse
                {
                    labels = detectionRunFinishedResponse.labels,
                    scores = detectionRunFinishedResponse.scores,
                    bboxes = detectionRunFinishedResponse.bboxes
                };

                // Close the dataset
                gdalDataset.Dispose();

                return ResultDTO<DetectionRunFinishedResponse>.Ok(updatedResponse);
            }
            catch(Exception ex)
            {
                if(gdalDataset is not null)
                    gdalDataset.Dispose();

                _logger.LogError(ex.Message, ex);
                return ResultDTO<DetectionRunFinishedResponse>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<DetectedDumpSite>>> CreateDetectedDumpsSitesFromDetectionRun
            (Guid detectionRunId, DetectionRunFinishedResponse detectedDumpSitesProjectedResponse)
        {
            string detectedZonesDatasetClassIdStr = DetectionResultDummyDatasetClassId;
            Guid detectedZonesDatasetClassId = Guid.Parse(detectedZonesDatasetClassIdStr);

            List<DetectedDumpSiteDTO> detectedDumpSites = new List<DetectedDumpSiteDTO>();

            GeometryFactory factory = new GeometryFactory();
            List<Polygon> polygons = new List<Polygon>();
            for (int i = 0; i < detectedDumpSitesProjectedResponse.bboxes.Length; i++)
            {
                var box = detectedDumpSitesProjectedResponse.bboxes[i];
                var lowerLeft = new Coordinate(box[0], box[1]);
                var upperRight = new Coordinate(box[2], box[3]);

                // Create a rectangle polygon from the bounding box
                Coordinate[] coordinates = new Coordinate[] {
                    lowerLeft, new Coordinate(upperRight.X, lowerLeft.Y), upperRight, new Coordinate(lowerLeft.X, upperRight.Y),
                    lowerLeft  // Closed linear ring 
                };

                //polygons.Add(factory.CreatePolygon(coordinates));
                detectedDumpSites.Add(new DetectedDumpSiteDTO()
                {
                    DetectionRunId = detectionRunId,
                    DatasetClassId = detectedZonesDatasetClassId,
                    ConfidenceRate = detectedDumpSitesProjectedResponse.scores[i],
                    Geom = factory.CreatePolygon(coordinates)
                });
            }

            List<DetectedDumpSite> detectedDumpSitesEntities = _mapper.Map<List<DetectedDumpSite>>(detectedDumpSites);

            foreach (DetectedDumpSite detectedDumpSite in detectedDumpSitesEntities)
            {
                ResultDTO resultCreate = await _detectedDumpSitesRepository.Create(detectedDumpSite);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                    return ResultDTO<List<DetectedDumpSite>>.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO<List<DetectedDumpSite>>.Ok(detectedDumpSitesEntities);
        }
    }
}
