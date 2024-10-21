using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DetectionServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using SD;
using SD.Helpers;

namespace MainApp.BL.Services.DetectionServices
{
    public class DetectionRunService : IDetectionRunService
    {
        private readonly IDetectionRunsRepository _detectionRunRepository;
        private readonly IDetectedDumpSitesRepository _detectedDumpSitesRepository;
        private readonly IDetectionInputImageRepository _detectionInputImageRepository;
        protected readonly IMMDetectionConfigurationService _MMDetectionConfiguration;
        private readonly IMapper _mapper;
        private readonly ILogger<DetectionRunService> _logger;
        private readonly IConfiguration _configuration;

        // TODO: Think Refactoring
        private string DetectionResultDummyDatasetClassId = string.Empty;
        private string HasGPU = string.Empty;

        public DetectionRunService(IDetectionRunsRepository detectionRunRepository, IMapper mapper, ILogger<DetectionRunService> logger, IConfiguration configuration, IDetectedDumpSitesRepository detectedDumpSitesRepository, IDetectionInputImageRepository detectionInputImageRepository, IMMDetectionConfigurationService mMDetectionConfiguration)
        {
            _detectionRunRepository = detectionRunRepository;
            _detectionInputImageRepository = detectionInputImageRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;

           
            string? hasGPU = _configuration["AppSettings:MMDetection:HasGPU"];
            if (string.IsNullOrEmpty(hasGPU))
                throw new Exception($"{nameof(hasGPU)} is missing");

            string? detectionResultDummyDatasetClassId = _configuration["AppSettings:MMDetection:DetectionResultDummyDatasetClassId"];
            if (string.IsNullOrEmpty(detectionResultDummyDatasetClassId))
                throw new Exception($"{nameof(detectionResultDummyDatasetClassId)} is missing");
         
            HasGPU = hasGPU;
            _detectedDumpSitesRepository = detectedDumpSitesRepository;
            DetectionResultDummyDatasetClassId = detectionResultDummyDatasetClassId;
            _MMDetectionConfiguration = mMDetectionConfiguration;
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

        public async Task<ResultDTO<List<DetectionRunDTO>>> GetSelectedDetectionRunsIncludingDetectedDumpSites(List<Guid> selectedDetectionRunIds, List<ConfidenceRateDTO> selectedConfidenceRates)
        {
            try
            {
                ResultDTO<IEnumerable<DetectionRun>> resultGetAllEntities = await _detectionRunRepository
                    .GetAll(filter: x => selectedDetectionRunIds.Contains(x.Id), includeProperties: "CreatedBy,DetectedDumpSites,DetectionInputImage");
                if (!resultGetAllEntities.IsSuccess && resultGetAllEntities.HandleError())
                    return ResultDTO<List<DetectionRunDTO>>.Fail(resultGetAllEntities.ErrMsg!);

                var detectionRuns = resultGetAllEntities.Data.ToList();
                var confidenceRateDict = selectedConfidenceRates.ToDictionary(cr => cr.detectionRunId, cr => cr.confidenceRate);

                foreach (var detectionRun in detectionRuns)
                {
                    if (confidenceRateDict.TryGetValue(detectionRun.Id, out var confidenceThreshold))
                    {
                        detectionRun.DetectedDumpSites = detectionRun.DetectedDumpSites?.Where(dds => dds.ConfidenceRate > confidenceThreshold/100).ToList();
                    }
                }

                List<DetectionRunDTO> dtos = _mapper.Map<List<DetectionRunDTO>>(detectionRuns);

                return ResultDTO<List<DetectionRunDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }
        
        public async Task<List<AreaComparisonAvgConfidenceRateReportDTO>> GenerateAreaComparisonAvgConfidenceRateData(List<Guid> selectedDetectionRunsIds)
        {
            List<DetectionRun> list = await _detectionRunRepository.GetSelectedDetectionRunsWithClasses(selectedDetectionRunsIds) ?? throw new Exception("Object not found");

            var groupedDumpSites = list.Select(detectionRun => new
            {
                DetectionRun = detectionRun,
                GroupedDumpSites = detectionRun.DetectedDumpSites
            .GroupBy(dumpSite => dumpSite.DatasetClass.ClassName)
            .ToDictionary(group => group.Key, group => group.ToList())
            }).ToList();

            List<AreaComparisonAvgConfidenceRateReportDTO> listDTO = new();
            foreach (var group in groupedDumpSites)
            {
                AreaComparisonAvgConfidenceRateReportDTO model = new();
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

        public async Task<ResultDTO> DeleteDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                DetectionRun detectionRun = _mapper.Map<DetectionRun>(detectionRunDTO);
                ResultDTO resultDeleteEntity = await _detectionRunRepository.Delete(detectionRun);

                if (resultDeleteEntity.IsSuccess == false && resultDeleteEntity.HandleError())
                    return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);               

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
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
            string trainedModelConfigPath, string trainedModelModelPath, Guid detectionRunId, bool isSmallImage = false, bool hasGPU = false)
        {
            string detectionCommandStr = string.Empty;
            string scriptName = isSmallImage ? "demo\\image_demo.py" : "ins_development\\scripts\\large_image_annotated_inference.py";
            scriptName = CommonHelper.PathToLinuxRegexSlashReplace(scriptName);
            string weightsCommandParamStr = isSmallImage ? "--weights" : string.Empty;
            string patchSizeCommand = isSmallImage ? string.Empty : "--patch-size 1280";
            string cpuDetectionCommandPart = hasGPU == false ? "--device cpu" : string.Empty;

            string outDirAbsPath = _MMDetectionConfiguration.GetDetectionRunOutputDirAbsPathByRunId(detectionRunId);
            string outDirCommandPart = $"--out-dir {CommonHelper.PathToLinuxRegexSlashReplace(outDirAbsPath)}";

            string openmmlabAbsPath = _MMDetectionConfiguration.GetOpenMMLabAbsPath();

            detectionCommandStr = $"run -p {openmmlabAbsPath} python {scriptName} " +
                $"\"{CommonHelper.PathToLinuxRegexSlashReplace(imageToRunDetectionOnPath)}\" " +
                $"{CommonHelper.PathToLinuxRegexSlashReplace(trainedModelConfigPath)} " +
                $"{weightsCommandParamStr} {CommonHelper.PathToLinuxRegexSlashReplace(trainedModelModelPath)} " +
                $"{outDirCommandPart} {cpuDetectionCommandPart} {patchSizeCommand}";

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
        public async Task<ResultDTO> UpdateStatus(Guid detectionRunId, string status)
        {
            try
            {
                ResultDTO<DetectionRun?> resultGetEntity = await _detectionRunRepository.GetById(detectionRunId, track: true);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data is null)
                    return ResultDTO.Fail($"No Detection Run found with ID: {detectionRunId}");

                DetectionRun detectionRunEntity = resultGetEntity.Data!;
                detectionRunEntity.Status = status;

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
        public async Task<ResultDTO> IsCompleteUpdateDetectionRun(DetectionRunDTO detectionRunDTO)
        {
            try
            {
                ResultDTO<DetectionRun?> resultGetEntity = await _detectionRunRepository.GetById(detectionRunDTO.Id.Value, track: true);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data is null)
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
                    return ResultDTO.Fail("Detection Run is null");

                if(detectionRunDTO.TrainedModelId is null)
                    return ResultDTO.Fail($"Trained Model Id is null for Detection Run with id {detectionRunDTO.Id}");

                if(string.IsNullOrEmpty(detectionRunDTO.TrainedModel.ModelConfigPath) || string.IsNullOrEmpty(detectionRunDTO.TrainedModel.ModelFilePath))
                    return ResultDTO.Fail($"Trained Model config or file is null or empty for Detection Run with id {detectionRunDTO.Id}");

                bool hasGPU = false;
                string hasGPUStrAppSetting = HasGPU;
                if (hasGPUStrAppSetting == "true")
                    hasGPU = true;

                string detectionCommand =
                    GeneratePythonDetectionCommandByType(
                        imageToRunDetectionOnPath: detectionRunDTO.InputImgPath,
                        trainedModelConfigPath: CommonHelper.PathToLinuxRegexSlashReplace(detectionRunDTO.TrainedModel.ModelConfigPath!),
                        trainedModelModelPath: CommonHelper.PathToLinuxRegexSlashReplace(detectionRunDTO.TrainedModel.ModelFilePath!),
                        detectionRunId: detectionRunDTO.Id!.Value,
                        isSmallImage: false, hasGPU: hasGPU);

                var powerShellResults = await Cli.Wrap(_MMDetectionConfiguration.GetCondaExeAbsPath())
                        .WithWorkingDirectory(_MMDetectionConfiguration.GetRootDirAbsPath())
                        .WithValidation(CommandResultValidation.None)
                        .WithArguments(detectionCommand.ToLower())
                        .WithStandardOutputPipe(PipeTarget.ToFile(
                            Path.Combine(_MMDetectionConfiguration.GetDetectionRunCliOutDirAbsPath(), $"succ_{detectionRunDTO.Id.Value}.txt")))
                        .WithStandardErrorPipe(PipeTarget.ToFile(
                            Path.Combine(_MMDetectionConfiguration.GetDetectionRunCliOutDirAbsPath(), $"error_{detectionRunDTO.Id.Value}.txt")))
                        .ExecuteBufferedAsync();

                if (powerShellResults.IsSuccess == false)
                    return ResultDTO.Fail($"Detection Run failed with error: {powerShellResults.StandardError}");

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> GetRawDetectionRunResultPathsByRunId(Guid detectionRunId)
        {
            string absDetectionRunResultsBBoxesFilePath = _MMDetectionConfiguration.GetDetectionRunOutputAnnotationsFileAbsPathByRunId(detectionRunId);

            if (File.Exists(absDetectionRunResultsBBoxesFilePath) == false)
                return ResultDTO<string>.Fail("No Polygonized Predictions Detection Run Results Found");

            return ResultDTO<string>.Ok(absDetectionRunResultsBBoxesFilePath);
        }

        public async Task<ResultDTO<DetectionRunFinishedResponse>> GetBBoxResultsDeserialized(string absBBoxResultsFilePath)
        {
            ResultDTO<DetectionRunFinishedResponse> resultJsonDeserialization =
                await NewtonsoftJsonHelper.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(absBBoxResultsFilePath);

            return resultJsonDeserialization;
        }

        public async Task<ResultDTO<DetectionRunFinishedResponse>> ConvertBBoxResultToImageProjection
            (string absoluteImagePath, DetectionRunFinishedResponse detectionRunFinishedResponse)
        {
            if (string.IsNullOrEmpty(absoluteImagePath))
                return ResultDTO<DetectionRunFinishedResponse>.Fail($"{nameof(absoluteImagePath)} is null or empty");

            if (detectionRunFinishedResponse is null)
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
            catch (Exception ex)
            {
                if (gdalDataset is not null)
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

        //Images
        #region DetectionInputImage
        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetAllImages()
        {
            try
            {
                ResultDTO<IEnumerable<DetectionInputImage>> resultGetEntities = await _detectionInputImageRepository.GetAll(includeProperties: "CreatedBy");
                if (resultGetEntities.IsSuccess is false && resultGetEntities.HandleError())
                {
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
                }
                List<DetectionInputImageDTO> dtos = _mapper.Map<List<DetectionInputImageDTO>>(resultGetEntities.Data);               
                return ResultDTO<List<DetectionInputImageDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionInputImageDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<DetectionInputImageDTO>>> GetSelectedInputImagesById(List<Guid> selectedImagesIds)
        {
            try
            {
                ResultDTO<IEnumerable<DetectionInputImage>> resultGetEntities = await _detectionInputImageRepository.GetAll(filter: x => selectedImagesIds.Contains(x.Id),includeProperties: "CreatedBy");
                if (resultGetEntities.IsSuccess is false && resultGetEntities.HandleError())
                {
                    return ResultDTO<List<DetectionInputImageDTO>>.Fail(resultGetEntities.ErrMsg!);
                }
                List<DetectionInputImageDTO> dtos = _mapper.Map<List<DetectionInputImageDTO>>(resultGetEntities.Data);
                return ResultDTO<List<DetectionInputImageDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionInputImageDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<DetectionInputImageDTO>> GetDetectionInputImageById(Guid detectionInputImageId)
        {
            try
            {
                ResultDTO<DetectionInputImage?> resultGetEntity = await _detectionInputImageRepository.GetById(detectionInputImageId, includeProperties: "CreatedBy");

                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO<DetectionInputImageDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                DetectionInputImageDTO dto = _mapper.Map<DetectionInputImageDTO>(resultGetEntity.Data);
                return ResultDTO<DetectionInputImageDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<DetectionInputImageDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<DetectionInputImageDTO>> CreateDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO)
        {
            try
            {
                DetectionInputImage detectionInputImageEntity = _mapper.Map<DetectionInputImage>(detectionInputImageDTO);

                ResultDTO<DetectionInputImage> resultCreate = await _detectionInputImageRepository.CreateAndReturnEntity(detectionInputImageEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO<DetectionInputImageDTO>.Fail(resultCreate.ErrMsg!);
                }
                DetectionInputImageDTO dto = _mapper.Map<DetectionInputImageDTO>(resultCreate.Data);
                return ResultDTO<DetectionInputImageDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<DetectionInputImageDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO)
        {
            try
            {
                DetectionInputImage detectionInputImageEntity = _mapper.Map<DetectionInputImage>(detectionInputImageDTO);

                ResultDTO resultCreate = await _detectionInputImageRepository.Delete(detectionInputImageEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> EditDetectionInputImage(DetectionInputImageDTO detectionInputImageDTO)
        {
            try
            {
                DetectionInputImage detectionInputImageEntity = _mapper.Map<DetectionInputImage>(detectionInputImageDTO);

                ResultDTO resultCreate = await _detectionInputImageRepository.Update(detectionInputImageEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<DetectionRunDTO>>> GetDetectionInputImageByDetectionRunId(Guid detectionInputImageId)
        {
            try
            {
                ResultDTO<IEnumerable<DetectionRun>> resultGetAllEntites = await _detectionRunRepository.GetAll(filter: x => x.DetectionInputImageId == detectionInputImageId);

                if (!resultGetAllEntites.IsSuccess && resultGetAllEntites.HandleError())
                {
                    return ResultDTO<List<DetectionRunDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                }

                List<DetectionRunDTO> dtos = _mapper.Map<List<DetectionRunDTO>>(resultGetAllEntites.Data);
                return ResultDTO<List<DetectionRunDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DetectionRunDTO>>.ExceptionFail(ex.Message, ex);
            }
        }



        #endregion
    }
}
