using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Entities;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SD;
using SD.Helpers;

namespace DAL.ApplicationStorage.SeedDatabase.ModulesConfigs.MMDetectionSetup
{
    internal class MMDetectionSetupService
    {
        private readonly MMDetectionConfiguration _mmConfiguration;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public MMDetectionSetupService(IConfiguration configuration, ApplicationDbContext db)
        {
            _configuration = configuration;
            _db = db;
            _mmConfiguration = GetMMDetectionConfigurationAppSettings();
        }

        private MMDetectionConfiguration GetMMDetectionConfigurationAppSettings()
        {
            IConfigurationSection section = _configuration.GetSection("MMDetectionConfiguration");

            MMDetectionConfiguration mmDetectionConfiguration =
                new MMDetectionConfiguration(
                    new MMDetectionBaseConfiguration(
                        section["Base:CondaExeAbsPath"],
                        section["Base:RootDirAbsPath"],
                        section["Base:ScriptsDirRelPath"],
                        section["Base:ResourcesDirRelPath"],
                        section["Base:OpenMMLabAbsPath"]
                    ),
                    new MMDetectionTrainingConfiguration(
                        section["Training:DatasetsDirRelPath"],
                        section["Training:ConfigsDirRelPath"],
                        section["Training:OutputDirRelPath"],
                        section["Training:BackboneCheckpointAbsPath"],
                        section["Training:CliLogsAbsPath"]
                    ),
                    new MMDetectionDetectionConfiguration(
                        section["Detection:OutputDirRelPath"],
                        section["Detection:CliLogsAbsPath"]
                    )
                );

            return mmDetectionConfiguration;
        }

        public ResultDTO<string> CreateTrainingAndDetectionProcessDirsInMMDetectionRoot()
        {
            string mmRootDir = _mmConfiguration.Base.RootDirAbsPath;
            if (string.IsNullOrEmpty(mmRootDir))
                return ResultDTO<string>.Fail("MMDetection root directory path is not configured");

            if (Directory.Exists(mmRootDir) == false)
                return ResultDTO<string>.Fail("MMDetection root directory path does not exist, or insufficient permissions");

            // Scripts
            string scriptsDirAbsPath = Path.Combine(mmRootDir, _mmConfiguration.Base.ScriptsDirRelPath);
            if (Directory.Exists(scriptsDirAbsPath) == false)
                Directory.CreateDirectory(scriptsDirAbsPath);

            // Resources
            string resourcesDirAbsPath = Path.Combine(mmRootDir, _mmConfiguration.Base.RootDirAbsPath);
            if (Directory.Exists(resourcesDirAbsPath) == false)
                Directory.CreateDirectory(resourcesDirAbsPath);

            // Training Output
            string trainingOutputDirAbsPath = Path.Combine(mmRootDir, _mmConfiguration.Training.OutputDirRelPath);
            if (Directory.Exists(trainingOutputDirAbsPath) == false)
                Directory.CreateDirectory(trainingOutputDirAbsPath);

            // Detection Output
            string detectionOutputsDirAbsPath = Path.Combine(mmRootDir, _mmConfiguration.Detection.OutputDirRelPath);
            if (Directory.Exists(detectionOutputsDirAbsPath) == false)
                Directory.CreateDirectory(detectionOutputsDirAbsPath);

            return ResultDTO<string>.Ok(scriptsDirAbsPath);
        }

        public record SeedTrainedModel(string Name, string ConfigDownloadUrl, string ModelFile);
        public ResultDTO GetAndSeedTrainedModelsFromSeedFile()
        {
            IDbContextTransaction? dbTransaction = null;
            try
            {
                if (_db.TrainedModels.Count() > 0)
                {
                    Console.WriteLine("Trained Models have Already been Seeded");
                    return ResultDTO.Ok();
                }

                string? seedTrainingAndDetectionProcessFile = _configuration["SeedDatabaseFilePaths:SeedTrainingAndDetectionProcess"];
                if (string.IsNullOrEmpty(seedTrainingAndDetectionProcessFile))
                    return ResultDTO.Fail("Null Seed File in Config");

                if (File.Exists(seedTrainingAndDetectionProcessFile) == false)
                    return ResultDTO.Fail($"JSON File does not exist: {seedTrainingAndDetectionProcessFile}");

                string seedTrainingAndDetectionProcessData = File.ReadAllText(seedTrainingAndDetectionProcessFile);
                JObject jsonObj = JObject.Parse(seedTrainingAndDetectionProcessData);
                JToken jsonTraining = jsonObj["Training"];
                List<SeedTrainedModel>? initialBaseModels =
                    JsonConvert.DeserializeObject<List<SeedTrainedModel>>(jsonTraining["InitialBaseModels"].ToString());
                if (initialBaseModels is null)
                    return ResultDTO.Fail("Failed to Deserialize Object Trained Models to <List<(string name, string config, string model)>");

                ApplicationUser? applicationUser = _db.Users.FirstOrDefault();
                if (applicationUser is null)
                    return ResultDTO.Fail("Failed to find a User");

                DatasetClass? datasetClassEntity = null;
                if (_db.DatasetClasses.Where(x => x.ClassName.ToLower() == "waste") != null
                    && _db.DatasetClasses.Where(x => x.ClassName.ToLower() == "waste").Count() > 0)
                {
                    datasetClassEntity = _db.DatasetClasses.Where(x => x.ClassName.ToLower() == "waste").First();
                }

                if (datasetClassEntity is null)
                {
                    datasetClassEntity = new DatasetClass
                    {
                        Id = Guid.NewGuid(),
                        ClassName = "waste",
                        CreatedById = applicationUser.Id,
                        CreatedOn = DateTime.UtcNow,
                    };
                    _db.DatasetClasses.Add(datasetClassEntity);
                }

                Console.WriteLine("Dataset Class: " + datasetClassEntity.Id);

                List<TrainedModel> trainedModels = new();
                dbTransaction = _db.Database.BeginTransaction();
                foreach (SeedTrainedModel model in initialBaseModels)
                {
                    Console.WriteLine("name: " + model.Name);
                    Console.WriteLine("config: " + model.ConfigDownloadUrl);
                    Console.WriteLine("model: " + model.ModelFile);
                    ResultDTO<string> downloadModelResult =
                        DownloadAndCopyTrainedModelToMMDetection(model.Name, model.ModelFile).GetAwaiter().GetResult();
                    if (downloadModelResult.IsSuccess == false && downloadModelResult.ExObj is null)
                        return ResultDTO.Fail(downloadModelResult.ErrMsg!);
                    if (downloadModelResult.IsSuccess == false && downloadModelResult.ExObj is not null)
                        return ResultDTO.Fail(((Exception)downloadModelResult.ExObj).Message);
                    if (string.IsNullOrEmpty(downloadModelResult.Data))
                        return ResultDTO.Fail($"Null path of downloaded model file {model.ModelFile}");

                    ResultDTO<string> downloadConfigResult =
                        DownloadAndCopyTrainedModelToMMDetection(model.Name, model.ConfigDownloadUrl).GetAwaiter().GetResult();
                    if (downloadConfigResult.IsSuccess == false && downloadConfigResult.ExObj is null)
                        return ResultDTO.Fail(downloadConfigResult.ErrMsg!);
                    if (downloadConfigResult.IsSuccess == false && downloadConfigResult.ExObj is not null)
                        return ResultDTO.Fail(((Exception)downloadConfigResult.ExObj).Message);
                    if (string.IsNullOrEmpty(downloadConfigResult.Data))
                        return ResultDTO.Fail($"Null path of downloaded config file {model.ConfigDownloadUrl}");

                    Console.WriteLine($"Config file Path: {downloadConfigResult.Data!}");
                    Console.WriteLine($"Model file Path: {downloadModelResult.Data!} ");
                    Console.WriteLine($"App User: {applicationUser} ");
                    Console.WriteLine($"App User Id: {applicationUser.Id} ");

                    Dataset dummyBaseTrainedModelDataset = new Dataset()
                    {
                        Id = Guid.NewGuid(),
                        Name = $"DummyDataset{model.Name}",
                        Description = $"Dummy Dataset for Trained Model: {model.Name}",
                        IsPublished = false,
                        CreatedById = applicationUser.Id,
                        CreatedBy = null,
                        CreatedOn = DateTime.UtcNow,
                    };
                    Console.WriteLine($"Added DatasetId: {dummyBaseTrainedModelDataset.Id}");
                    Dataset_DatasetClass dataset_DatasetClass = new Dataset_DatasetClass
                    {
                        Id = Guid.NewGuid(),
                        DatasetId = dummyBaseTrainedModelDataset.Id,
                        DatasetClassId = datasetClassEntity.Id,
                        DatasetClassValue = 1
                    };
                    Console.WriteLine($"Added Dataset Dataset Class Id: {dataset_DatasetClass.Id}");
                    dummyBaseTrainedModelDataset.DatasetClasses = [dataset_DatasetClass];
                    
                    

                    TrainedModel baseTrainedModel = new TrainedModel
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        ModelConfigPath = downloadConfigResult.Data!,
                        ModelFilePath = downloadModelResult.Data!,
                        IsPublished = true,
                        DatasetId = dummyBaseTrainedModelDataset.Id,
                        Dataset = dummyBaseTrainedModelDataset,
                        CreatedById = applicationUser.Id,
                        CreatedOn = DateTime.UtcNow,
                    };
                    trainedModels.Add(baseTrainedModel);
                    Console.WriteLine($"Added Model: {baseTrainedModel.Name}");
                }

                _db.TrainedModels.AddRange(trainedModels);
                int numRows = _db.SaveChanges();
                if (numRows <= 0)
                    throw new RowNotInTableException(nameof(numRows));

                dbTransaction.Commit();

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                if (dbTransaction is not null)
                    dbTransaction.Rollback();
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public ResultDTO<string> CreateTrainedModelDir(string modelName)
        {
            string mmRootDir = _mmConfiguration.Base.RootDirAbsPath;
            if (string.IsNullOrEmpty(mmRootDir))
                return ResultDTO<string>.Fail("MMDetection root directory path is not configured");

            if (Directory.Exists(mmRootDir) == false)
                return ResultDTO<string>.Fail("MMDetection root directory path does not exist, or insufficient permissions");

            string downloadedModelsDir = Path.Combine(mmRootDir, _mmConfiguration.Base.ResourcesDirRelPath, modelName.Replace(" ", "-"));
            if (Directory.Exists(downloadedModelsDir) == false)
                Directory.CreateDirectory(downloadedModelsDir);

            return ResultDTO<string>.Ok(downloadedModelsDir);
        }

        public async Task<ResultDTO<string>> DownloadAndCopyTrainedModelToMMDetection(string modelName, string fileUrl)
        {
            try
            {
                ResultDTO<string> createTrainedModelDirResult = CreateTrainedModelDir(modelName);
                if (createTrainedModelDirResult.IsSuccess == false)
                    return ResultDTO<string>.Fail(createTrainedModelDirResult.ErrMsg!);

                string fileDestPath = Path.Combine(createTrainedModelDirResult.Data, CommonHelper.GetFileNameFromUrl(fileUrl));

                // Handle custom trained model download
                //string insModelUrl = 
                //    "https://huggingface.co/INS-IntelligentNetworkSolutions/waste-detection-faster_rcnn-mmdetection/resolve/main/epoch_9.pth?download=true";

                if (File.Exists(fileDestPath) == false)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Configure client timeout for large files
                        client.Timeout = TimeSpan.FromMinutes(10);

                        // Download with progress tracking
                        using (HttpResponseMessage response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                        {
                            if (response.IsSuccessStatusCode == false)
                                return ResultDTO<string>.Fail($"Failed to download model. Status code: {response.StatusCode}");

                            using (Stream contentStream = response.Content.ReadAsStream())
                            using (FileStream fileStream = new FileStream(fileDestPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                                byte[] buffer = new byte[8192];
                                int bytesRead = 0;

                                while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fileStream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }

                if (File.Exists(fileDestPath) == false)
                    return ResultDTO<string>.Fail("Failed to download custom trained model");

                return ResultDTO<string>.Ok(fileDestPath);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail($"Failed to download and setup model files: {ex.Message}", ex);
            }
        }
    }
}
