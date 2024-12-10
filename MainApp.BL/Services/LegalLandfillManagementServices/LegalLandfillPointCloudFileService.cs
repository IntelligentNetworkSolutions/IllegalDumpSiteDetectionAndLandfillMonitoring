using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DAL.Repositories.LegalLandfillManagementRepositories;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.DetectionEntities;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSGeo.GDAL;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillPointCloudFileService : ILegalLandfillPointCloudFileService
    {
        private readonly ILegalLandfillPointCloudFileRepository _legalLandfillPointCloudFileRepository;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillPointCloudFileService> _logger;
        public LegalLandfillPointCloudFileService(ILegalLandfillPointCloudFileRepository legalLandfillPointCloudFileRepository, IAppSettingsAccessor appSettingsAccessor, IMapper mapper, ILogger<LegalLandfillPointCloudFileService> logger)
        {
            _legalLandfillPointCloudFileRepository = legalLandfillPointCloudFileRepository;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetAllLegalLandfillPointCloudFiles()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites = await _legalLandfillPointCloudFileRepository.GetAll(includeProperties: "LegalLandfill");
                if (!resultGetAllEntites.IsSuccess && resultGetAllEntites.HandleError())                
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Point cloud list not found");                

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);
                if(dtos == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Mapping point cloud list failed");

                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetFilteredLegalLandfillPointCloudFiles(List<Guid> selectedIds)
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites = await _legalLandfillPointCloudFileRepository.GetAll(filter: x => selectedIds.Contains(x.Id), includeProperties: "LegalLandfill");
                if (!resultGetAllEntites.IsSuccess && resultGetAllEntites.HandleError())                
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Point cloud list not found");

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);
                if (dtos == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Mapping point cloud list failed");

                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetLegalLandfillPointCloudFilesByLandfillId(Guid legalLandfillId)
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites = await _legalLandfillPointCloudFileRepository.GetAll(filter: x => x.LegalLandfillId == legalLandfillId);
                if (!resultGetAllEntites.IsSuccess && resultGetAllEntites.HandleError())                
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Point cloud list not found");

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);
                if (dtos == null)
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail("Mapping point cloud list failed");

                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> GetLegalLandfillPointCloudFilesById(Guid legalLandfillPointCloudFileId)
        {
            try
            {
                ResultDTO<LegalLandfillPointCloudFile?>? resultGetEntity = await _legalLandfillPointCloudFileRepository.GetById(legalLandfillPointCloudFileId, includeProperties: "LegalLandfill");
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Point cloud not found");

                LegalLandfillPointCloudFileDTO dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Mapping point cloud failed");

                return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> CreateLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO)
        {
            try
            {
                LegalLandfillPointCloudFile entity = _mapper.Map<LegalLandfillPointCloudFile>(legalLandfillPointCloudFileDTO);
                if (entity == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Mapping point cloud failed");

                ResultDTO<LegalLandfillPointCloudFile> resultCreateAndReturnEntity = await _legalLandfillPointCloudFileRepository.CreateAndReturnEntity(entity);
                if (!resultCreateAndReturnEntity.IsSuccess && resultCreateAndReturnEntity.HandleError())                
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultCreateAndReturnEntity.ErrMsg!);      
                if (resultCreateAndReturnEntity.Data is null)                
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Error Creating Detection Run");
                
                LegalLandfillPointCloudFileDTO dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultCreateAndReturnEntity.Data);
                if (dto == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Mapping point cloud failed");

                return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteLegalLandfillPointCloudFile(Guid legalLandfillPointCloudFileId)
        {
            try
            {
                ResultDTO<LegalLandfillPointCloudFile?> resultGetEntity = await _legalLandfillPointCloudFileRepository.GetFirstOrDefault(filter: x => x.Id == legalLandfillPointCloudFileId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);                
                if(resultGetEntity.Data == null)                
                    return ResultDTO.Fail("Data is null");
                
                ResultDTO resultDeleteEntity = await _legalLandfillPointCloudFileRepository.Delete(resultGetEntity.Data);
                if (!resultDeleteEntity.IsSuccess && resultDeleteEntity.HandleError())                
                    return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> EditLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO)
        {
            try
            {
                ResultDTO<LegalLandfillPointCloudFile?>? resultGetEntity = await _legalLandfillPointCloudFileRepository.GetById(legalLandfillPointCloudFileDTO.Id, track: true);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("File not exists in database!");

                if (legalLandfillPointCloudFileDTO.LegalLandfillId != resultGetEntity.Data.LegalLandfillId)
                {
                    ResultDTO<string?>? pointCloudUploadFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
                    if (string.IsNullOrEmpty(pointCloudUploadFolder.Data))
                        return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Uploads folder not found");

                    legalLandfillPointCloudFileDTO.FilePath = string.Format("{0}\\{1}\\", pointCloudUploadFolder.Data, legalLandfillPointCloudFileDTO.LegalLandfillId.ToString());
                }

                _mapper.Map(legalLandfillPointCloudFileDTO, resultGetEntity.Data);

                ResultDTO<LegalLandfillPointCloudFile> resultUpdateEntity = await _legalLandfillPointCloudFileRepository.UpdateAndReturnEntity(resultGetEntity.Data);
                if (!resultUpdateEntity.IsSuccess && resultUpdateEntity.HandleError())
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultUpdateEntity.ErrMsg!);
                if (resultUpdateEntity.Data == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Updated point clound not found");

                LegalLandfillPointCloudFileDTO returnDto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultUpdateEntity.Data);
                if (returnDto == null)
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Mapping point cloud failed");

                return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(returnDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> CreateDiffWasteVolumeComparisonFile(List<LegalLandfillPointCloudFileDTO> orderedList, string webRootPath)
        {
            try
            {
                LegalLandfillPointCloudFileDTO? firstElement = orderedList[0];
                LegalLandfillPointCloudFileDTO? secondElement = orderedList[1];
                if(firstElement.FilePath == null || secondElement.FilePath == null)                
                    return ResultDTO<string>.Fail("File path/s is/are null");
                
                string? inputA = Path.Combine(webRootPath, firstElement.FilePath, firstElement.Id.ToString() + "_dsm.tif");
                string? inputB = Path.Combine(webRootPath, secondElement.FilePath, secondElement.Id.ToString() + "_dsm.tif");
                if (string.IsNullOrEmpty(inputA) || string.IsNullOrEmpty(inputB))
                    return ResultDTO<string>.Fail("File path/s is/are null");

                ResultDTO<string?>? pythonExeAbsPath = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("PythonExeAbsPath", string.Empty);
                ResultDTO<string?>? gdalCalcAbsPath = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("GdalCalcAbsPath", string.Empty);
                ResultDTO<string?>? outputDiffFolderPath = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("OutputDiffFolderPath", "Uploads\\LegalLandfillUploads\\TempDiffWasteVolumeComparisonUploads");

                if ((!pythonExeAbsPath.IsSuccess && pythonExeAbsPath.HandleError()) || (!gdalCalcAbsPath.IsSuccess && gdalCalcAbsPath.HandleError()) || (!outputDiffFolderPath.IsSuccess && outputDiffFolderPath.HandleError()))                
                    return ResultDTO<string>.Fail("Can not get some of the application settings");                
                if(pythonExeAbsPath.Data == null || gdalCalcAbsPath.Data == null || outputDiffFolderPath.Data == null)                
                    return ResultDTO<string>.Fail("Some of the paths are null");                

                string? outputDiffFilePath = Path.Combine(webRootPath, outputDiffFolderPath.Data, $"A_{firstElement.Id}-B_{secondElement.Id}_diff.tif");
                if (string.IsNullOrEmpty(outputDiffFilePath))
                    return ResultDTO<string>.Fail("Output diff file path is null");

                string? outputFolderPath = Path.Combine(webRootPath, outputDiffFolderPath.Data);
                if (string.IsNullOrEmpty(outputFolderPath))
                    return ResultDTO<string>.Fail("Output folder path is null");

                if (!Directory.Exists(outputFolderPath))               
                    Directory.CreateDirectory(outputFolderPath);
                

                var gdalRes = await Cli.Wrap(pythonExeAbsPath.Data)
                    .WithArguments($"\"{gdalCalcAbsPath.Data}\" -A \"{inputA}\" -B \"{inputB}\" --outfile=\"{outputDiffFilePath}\" --calc=\"A-B\" --extent=intersect --overwrite")
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteBufferedAsync();

                if (!gdalRes.IsSuccess)                
                    return ResultDTO<string>.Fail("Creating file failed");                

                return ResultDTO<string>.Ok(outputDiffFilePath);

                //Gdal.AllRegister();
                //using (Dataset datasetA = Gdal.Open(inputA, Access.GA_ReadOnly))
                //using (Dataset datasetB = Gdal.Open(inputB, Access.GA_ReadOnly))
                //{
                //    if (datasetA == null || datasetB == null)
                //    {
                //        return ResultDTO<string>.Fail("Could not open the TIF file");
                //    }

                //    Band bandA = datasetA.GetRasterBand(1);
                //    Band bandB = datasetB.GetRasterBand(1);

                //    int xSize = Math.Min(datasetA.RasterXSize, datasetB.RasterXSize);
                //    int ySize = Math.Min(datasetA.RasterYSize, datasetB.RasterYSize);

                //    float[] dataA = new float[xSize * ySize];
                //    float[] dataB = new float[xSize * ySize];
                //    bandA.ReadRaster(0, 0, xSize, ySize, dataA, xSize, ySize, 0, 0);
                //    bandB.ReadRaster(0, 0, xSize, ySize, dataB, xSize, ySize, 0, 0);

                //    float[] diffData = new float[xSize * ySize];
                //    for (int i = 0; i < diffData.Length; i++)
                //    {
                //        diffData[i] = dataA[i] - dataB[i];
                //    }

                //    Driver driver = Gdal.GetDriverByName("GTiff");
                //    Dataset outputDataset = driver.Create(outputDiffFilePath, xSize, ySize, 1, DataType.GDT_Float32, null);

                //    double[] geoTransform = new double[6];
                //    datasetA.GetGeoTransform(geoTransform);
                //    outputDataset.SetGeoTransform(geoTransform);
                //    outputDataset.SetProjection(datasetA.GetProjection());

                //    Band outputBand = outputDataset.GetRasterBand(1);
                //    outputBand.WriteRaster(0, 0, xSize, ySize, diffData, xSize, ySize, 0, 0);
                //    outputBand.SetNoDataValue(-9999);

                //    outputBand.FlushCache();
                //    outputDataset.FlushCache();

                //    outputDataset.Dispose();
                //    return ResultDTO<string>.Ok(outputDiffFilePath);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }
        public async Task<ResultDTO<double?>> ReadAndDeleteDiffWasteVolumeComparisonFile(string outputDiffFilePath)
        {
            try
            {
                Gdal.AllRegister();
                GdalConfiguration.ConfigureGdal();

                if (!File.Exists(outputDiffFilePath))                
                    return ResultDTO<double?>.Fail("The TIF file does not exist");
                

                double? wasteVolumeDiffrenceFormula;
                using (Dataset dataset = Gdal.Open(outputDiffFilePath, Access.GA_ReadOnly))
                {
                    if (dataset == null)                    
                        return ResultDTO<double?>.Fail("Could not open the TIF file");
                    
                    Band band = dataset.GetRasterBand(1);

                    double min, max, mean, stdDev;
                    band.ComputeStatistics(false, out min, out max, out mean, out stdDev, null, null);

                    string statisticsValidPercent = band.GetMetadataItem("STATISTICS_VALID_PERCENT", null);
                    string statisticsMean = band.GetMetadataItem("STATISTICS_MEAN", null);

                    if (string.IsNullOrEmpty(statisticsValidPercent) || string.IsNullOrEmpty(statisticsMean))                    
                        return ResultDTO<double?>.Fail("Required statistics are missing from the dataset");
                    

                    double statisticsValidPercentValue = double.Parse(statisticsValidPercent);
                    double statisticsMeanValue = double.Parse(statisticsMean);

                    int width = dataset.RasterXSize;
                    int height = dataset.RasterYSize;

                    double[] geoTransform = new double[6];
                    dataset.GetGeoTransform(geoTransform);
                    double pixelWidth = geoTransform[1];

                    wasteVolumeDiffrenceFormula = ((width * pixelWidth * (statisticsValidPercentValue / 100)) *
                                                          (height * pixelWidth * (statisticsValidPercentValue / 100)) *
                                                          statisticsMeanValue) * -1;

                }

                var xmlFilePath = Path.Combine(outputDiffFilePath + ".aux.xml");
                File.Delete(outputDiffFilePath);

                if (File.Exists(xmlFilePath))                     
                    File.Delete(xmlFilePath);
                
                return ResultDTO<double?>.Ok(wasteVolumeDiffrenceFormula);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<double?>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> UploadFile(IFormFile file, string uploadFolder, string fileName)
        {
            try
            {
                string? filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return ResultDTO<string>.Ok(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }

        }

        public async Task<ResultDTO> ConvertToPointCloud(string potreeConverterFilePath, string uploadResultData, string convertsFolder, string filePath)
        {
            try
            {
                if (!Directory.Exists(convertsFolder))                
                    Directory.CreateDirectory(convertsFolder);
                
                CommandResult? result = await Cli.Wrap(potreeConverterFilePath)
                .WithArguments($"{filePath} -o {convertsFolder}")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

                if (!result.IsSuccess)                
                    return ResultDTO.Fail("Converting failed");
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }

        }

        public async Task<ResultDTO> CreateTifFile(string pipelineJsonTemplate, string pdalAbsPath, string filePath, string tifFilePath)
        {
            try
            {
                string? pipelineJson = pipelineJsonTemplate
                  .Replace("{INPUT_FILE}", filePath.Replace("\\", "\\\\"))
                  .Replace("{OUTPUT_FILE}", tifFilePath.Replace("\\", "\\\\"));

                if(string.IsNullOrEmpty(pipelineJson))
                        return ResultDTO.Fail("Failed to customize pipeline json file");

                BufferedCommandResult? res = await Cli.Wrap(pdalAbsPath)
                    .WithArguments("pipeline --stdin")
                    .WithStandardInputPipe(PipeSource.FromString(pipelineJson))
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteBufferedAsync();

                if (!res.IsSuccess)                
                    return ResultDTO.Fail("Tif file creating failed");
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }

        }

        public async Task<ResultDTO> CheckSupportingFiles(string fileUploadExtension)
        {
            try
            {
                ResultDTO<string?>? supportedExtensionsAppSetting = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", ".las, .laz");
                if (!supportedExtensionsAppSetting.IsSuccess && supportedExtensionsAppSetting.HandleError())                
                    return ResultDTO.Fail(supportedExtensionsAppSetting.ErrMsg!);                
                if (supportedExtensionsAppSetting.Data == null)                
                    return ResultDTO.Fail("No data for supported file extensions");
                
                List<string> listOfSupportedExtensions = new List<string>(supportedExtensionsAppSetting.Data.Split(','));
                for (int i = 0; i < listOfSupportedExtensions.Count; i++)
                {
                    listOfSupportedExtensions[i] = listOfSupportedExtensions[i].Trim();
                }

                if (!listOfSupportedExtensions.Contains(fileUploadExtension))                
                    return ResultDTO.Fail($"Not supported file extension. Supported extensions are {supportedExtensionsAppSetting.Data}");
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteFilesFromUploads(LegalLandfillPointCloudFileDTO dto, string webRootPath)
        {
            if (dto.FilePath == null)            
                return ResultDTO.Fail("File path is null");
            
            string fileName = dto.Id + Path.GetExtension(dto.FileName);
            string tiffFileName = dto.Id + "_dsm.tif";
            string currentFilePathUpload = Path.Combine(webRootPath, dto.FilePath, fileName);
            string currentTiffFilePath = Path.Combine(webRootPath, dto.FilePath, tiffFileName);
            string currentLandfillFilesDir = Path.Combine(webRootPath, dto.FilePath);

            if (!System.IO.Directory.Exists(currentLandfillFilesDir))            
                return ResultDTO.Fail($"{currentFilePathUpload} directory does not exist and files were not deleted");
            
            try
            {
                string[] files = Directory.GetFiles(currentLandfillFilesDir);
                if (files.Length <= 2)
                {                  
                    if (System.IO.File.Exists(currentFilePathUpload))                    
                        System.IO.File.Delete(currentFilePathUpload);
                    
                    if (System.IO.File.Exists(currentTiffFilePath))                    
                        System.IO.File.Delete(currentTiffFilePath);
                    
                    Directory.Delete(currentLandfillFilesDir, true);
                }
                else
                {
                    if (System.IO.File.Exists(currentFilePathUpload))                    
                        System.IO.File.Delete(currentFilePathUpload);
                    
                    if (System.IO.File.Exists(currentTiffFilePath))                    
                        System.IO.File.Delete(currentTiffFilePath);                    
                }
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteFilesFromConverts(LegalLandfillPointCloudFileDTO dto, string webRootPath)
        {
            ResultDTO<string?>? pointCloudConvertFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
            if (!pointCloudConvertFolder.IsSuccess && pointCloudConvertFolder.HandleError())            
                return ResultDTO.Fail(pointCloudConvertFolder.ErrMsg!);            
            if (pointCloudConvertFolder.Data == null)            
                return ResultDTO.Fail("Point cloud convert folder path is null");
            
            string currentFolderOfConvertedFilePath = Path.Combine(webRootPath, pointCloudConvertFolder.Data, dto.LegalLandfillId.ToString(), dto.Id.ToString());
            string currentLandfillFilesConvertedDir = Path.Combine(webRootPath, pointCloudConvertFolder.Data, dto.LegalLandfillId.ToString());

            if (!System.IO.Directory.Exists(currentFolderOfConvertedFilePath))            
                return ResultDTO.Fail($"{currentFolderOfConvertedFilePath} directory does not exist and files were not deleted");
            
            if (!System.IO.Directory.Exists(currentLandfillFilesConvertedDir))            
                return ResultDTO.Fail($"{currentLandfillFilesConvertedDir} directory does not exist and files were not deleted");
            
            try
            {
                string[] subDir = Directory.GetDirectories(currentLandfillFilesConvertedDir);
                if (subDir.Length == 1)
                {
                    string subDirName = new DirectoryInfo(subDir[0]).Name;
                    if (subDirName == dto.Id.ToString())                    
                        Directory.Delete(currentLandfillFilesConvertedDir, true);                    
                }
                else
                {
                    Directory.Delete(currentFolderOfConvertedFilePath, true);
                }
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> EditFileInUploads(string webRootPath ,string filePath, LegalLandfillPointCloudFileDTO dto)
        {
            ResultDTO<string?>? pointCloudUploadFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
            if (!pointCloudUploadFolder.IsSuccess && pointCloudUploadFolder.HandleError())            
                return ResultDTO.Fail(pointCloudUploadFolder.ErrMsg!);            
            if (pointCloudUploadFolder.Data == null)            
                return ResultDTO.Fail("Point cloud upload folder path is null");
            
            try
            {
                string? fileNameUpload = dto.Id + Path.GetExtension(dto.FileName);
                string? oldFileUploadPath = Path.Combine(webRootPath, filePath, fileNameUpload);

                string tifFileName = dto.Id + "_dsm.tif";
                string? oldTifFilePath = Path.Combine(webRootPath, filePath, tifFileName);

                string? newFolderPath = Path.Combine(webRootPath, pointCloudUploadFolder.Data, dto.LegalLandfillId.ToString());
                string? newFileUploadPath = Path.Combine(newFolderPath, fileNameUpload);

                string? newTifFilePath = Path.Combine(newFolderPath, tifFileName);

                if (!System.IO.Directory.Exists(newFolderPath))                
                    System.IO.Directory.CreateDirectory(newFolderPath);
                
                System.IO.File.Move(oldFileUploadPath, newFileUploadPath);
                System.IO.File.Move(oldTifFilePath, newTifFilePath);

                string oldDirectory = Path.Combine(webRootPath, filePath);
                if (System.IO.Directory.GetFiles(oldDirectory).Length == 0 && System.IO.Directory.GetDirectories(oldDirectory).Length == 0)                
                    System.IO.Directory.Delete(oldDirectory);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }           
        }

        public async Task<ResultDTO> EditFileConverts(string webRootPath, Guid oldLegalLandfillId, LegalLandfillPointCloudFileDTO dto)
        {
            ResultDTO<string?>? pointCloudConvertFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
            if (!pointCloudConvertFolder.IsSuccess && pointCloudConvertFolder.HandleError())            
                return ResultDTO.Fail(pointCloudConvertFolder.ErrMsg!);            
            if (pointCloudConvertFolder.Data == null)            
                return ResultDTO.Fail("Point cloud upload folder path is null");
            
            string oldConvertedLandfillSubfolderPath = Path.Combine(webRootPath, pointCloudConvertFolder.Data, oldLegalLandfillId.ToString(), dto.Id.ToString());
            string oldConvertedLandfillFolderPath = Path.Combine(webRootPath, pointCloudConvertFolder.Data, oldLegalLandfillId.ToString());

            string newConvertedLandfillSubfolderPath = Path.Combine(webRootPath, pointCloudConvertFolder.Data, dto.LegalLandfillId.ToString(), dto.Id.ToString());
            string newConvertedLandfillFolderPath = Path.Combine(webRootPath, pointCloudConvertFolder.Data, dto.LegalLandfillId.ToString());

            try
            {
                if (!Directory.Exists(newConvertedLandfillFolderPath))                
                    Directory.CreateDirectory(newConvertedLandfillFolderPath);
                
                if (!Directory.Exists(newConvertedLandfillSubfolderPath))                
                    Directory.CreateDirectory(newConvertedLandfillSubfolderPath);
                
                foreach (var file in Directory.GetFiles(oldConvertedLandfillSubfolderPath))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(newConvertedLandfillSubfolderPath, fileName);
                    System.IO.File.Move(file, destFile);
                }

                if (System.IO.Directory.GetFiles(oldConvertedLandfillSubfolderPath).Length == 0 && System.IO.Directory.GetDirectories(oldConvertedLandfillSubfolderPath).Length == 0)                
                    System.IO.Directory.Delete(oldConvertedLandfillSubfolderPath);
                
                if (System.IO.Directory.GetFiles(oldConvertedLandfillFolderPath).Length == 0 && System.IO.Directory.GetDirectories(oldConvertedLandfillFolderPath).Length == 0)                
                    System.IO.Directory.Delete(oldConvertedLandfillFolderPath);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
           
        }
    }
}
