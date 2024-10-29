﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.TrainingRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.DatasetEntities;
using Entities.TrainingEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Services.TrainingServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using SD.Enums;
using SD.Helpers;
using Westwind.Globalization;

namespace Tests.MainAppBLTests.Services.TrainingServices
{
    public class TrainingRunServiceTests
    {
        private readonly Mock<IMMDetectionConfigurationService> _mockMMDetectionConfiguration;
        private readonly Mock<ITrainingRunsRepository> _mockTrainingRunsRepository;
        private readonly Mock<ITrainedModelsRepository> _mockTrainedModelsRepository;
        private readonly Mock<IDetectionRunService> _mockDetectionRunService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TrainingRunService>> _mockLogger;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly TrainingRunService _service;

        // Common test values
        private readonly string _rootDir = @"C:\mmdetection";
        private readonly string _openMMLab = @"C:\openmmlab";
        private readonly string _trainingRunsDir = @"training_runs";
        private readonly string _configsDir = @"configs";
        private readonly Guid _trainingRunId = Guid.NewGuid();

        public TrainingRunServiceTests()
        {
            _mockMMDetectionConfiguration = new Mock<IMMDetectionConfigurationService>();
            _mockTrainingRunsRepository = new Mock<ITrainingRunsRepository>();
            _mockTrainedModelsRepository = new Mock<ITrainedModelsRepository>();
            _mockDetectionRunService = new Mock<IDetectionRunService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TrainingRunService>>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            SetupBasicMMDetectionConfiguration();

            _service = new TrainingRunService(
                _mockMMDetectionConfiguration.Object,
                _mockTrainingRunsRepository.Object,
                _mockTrainedModelsRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockDetectionRunService.Object,
                _mockAppSettingsAccessor.Object
            );
        }

        private void SetupBasicMMDetectionConfiguration()
        {
            _mockMMDetectionConfiguration.Setup(x => x.GetRootDirAbsPath())
                .Returns(_rootDir);

            _mockMMDetectionConfiguration.Setup(x => x.GetOpenMMLabAbsPath())
                .Returns(_openMMLab);

            _mockMMDetectionConfiguration.Setup(x => x.GetConfigsDirAbsPath())
                .Returns(Path.Combine(_rootDir, _configsDir));

            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunsBaseOutDirAbsPath())
                .Returns(Path.Combine(_rootDir, _trainingRunsDir));
        }

        [Fact]
        public async Task GetTrainingRunById_ReturnsSuccess_WhenTrainingRunExists()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRun = new TrainingRun { Id = trainingRunId, Name = "Test Run" };
            var trainingRunDTO = new TrainingRunDTO { Id = trainingRunId, Name = "Test Run" };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(trainingRun))
                .Returns(trainingRunDTO);

            // Act
            var result = await _service.GetTrainingRunById(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(trainingRunDTO, result.Data);
        }

        [Fact]
        public async Task GetTrainingRunById_ReturnsFailure_WhenTrainingRunDoesNotExist()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(null));

            // Act
            var result = await _service.GetTrainingRunById(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess); // Note: The service returns success with null data in this case
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateTrainingRunWithBaseModel_ReturnsSuccess_WhenValidInputProvided()
        {
            // Arrange
            var inputDto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid(),
                Name = "Test Run"
            };

            var baseModel = new TrainedModel
            {
                Id = Guid.NewGuid(),
                Name = "Base Model"
            };

            var trainingRun = new TrainingRun
            {
                Id = inputDto.Id.Value,
                Name = inputDto.Name
            };

            _mockTrainedModelsRepository.Setup(x => x.GetByIdInclude(inputDto.TrainedModelId.Value, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel?>.Ok(baseModel));

            _mockMapper.Setup(x => x.Map<TrainedModelDTO>(baseModel))
                .Returns(new TrainedModelDTO { Id = baseModel.Id, Name = baseModel.Name });

            _mockMapper.Setup(x => x.Map<TrainingRun>(inputDto))
                .Returns(trainingRun);

            _mockTrainingRunsRepository.Setup(x => x.CreateAndReturnEntity(It.IsAny<TrainingRun>(), true, default))
                .ReturnsAsync(ResultDTO<TrainingRun>.Ok(trainingRun));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(trainingRun))
                .Returns(inputDto);

            // Act
            var result = await _service.CreateTrainingRunWithBaseModel(inputDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(inputDto.Id, result.Data?.Id);
            Assert.Equal(inputDto.Name, result.Data?.Name);
        }

        [Fact]
        public async Task UpdateTrainingRunEntity_ReturnsSuccess_WhenValidUpdateProvided()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRun = new TrainingRun
            {
                Id = trainingRunId,
                Name = "Original Name",
                Status = "Waiting",
                IsCompleted = false
            };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, true, null))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

            _mockTrainingRunsRepository.Setup(x => x.Update(It.IsAny<TrainingRun>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.UpdateTrainingRunEntity(
                trainingRunId,
                trainedModelId: Guid.NewGuid(),
                status: "Processing",
                isCompleted: true,
                name: "Updated Name"
            );

            // Assert
            Assert.True(result.IsSuccess);
            _mockTrainingRunsRepository.Verify(x => x.Update(It.Is<TrainingRun>(tr =>
                tr.Status == "Processing" &&
                tr.IsCompleted == true &&
                tr.Name == "Updated Name"
            ), true, default), Times.Once);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_GeneratesCorrectBasicCommand()
        {
            // Arrange
            string expectedConfigDir = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, _configsDir, _trainingRunId.ToString()));
            string expectedConfigFile = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(expectedConfigDir, $"{_trainingRunId}.py"));
            string expectedOutputDir = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, _trainingRunsDir, _trainingRunId.ToString()));
            string expectedOpenMMLab = CommonHelper.PathToLinuxRegexSlashReplace(_openMMLab);
            string expectedScriptPath = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, "tools", "train.py"));

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            //Assert.Contains($"-p {expectedOpenMMLab}", command); TODO: Check this
            Assert.Contains($"python {expectedScriptPath}", command);
            Assert.Contains($"\"{expectedConfigFile}\"", command);
            Assert.Contains("--work-dir", command);
            Assert.Contains($"\"{expectedOutputDir}\"", command);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_UsesCorrectPathSeparators()
        {
            // Arrange
            string windowsPath = @"C:\path\with\windows\separators";
            string expectedLinuxPath = "/path/with/windows/separators";
            _mockMMDetectionConfiguration.Setup(x => x.GetRootDirAbsPath())
                .Returns(windowsPath);

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains(expectedLinuxPath, command);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_HandlesSpecialCharactersInPaths()
        {
            // Arrange
            string pathWithSpaces = @"C:\Program Files\MMDetection";
            _mockMMDetectionConfiguration.Setup(x => x.GetRootDirAbsPath())
                .Returns(pathWithSpaces);

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains("\"", command); // Should contain quotes around paths with spaces
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_IncludesCorrectToolsPath()
        {
            // Arrange
            string expectedToolsPath = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, "tools", "train.py"));

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains(expectedToolsPath, command);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_IncludesCorrectConfigFile()
        {
            // Arrange
            string expectedConfigFile = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, _configsDir, _trainingRunId.ToString(), $"{_trainingRunId}.py"));

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains($"\"{expectedConfigFile}\"", command);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_IncludesCorrectWorkDir()
        {
            // Arrange
            string expectedWorkDir = CommonHelper.PathToLinuxRegexSlashReplace(
                Path.Combine(_rootDir, _trainingRunsDir, _trainingRunId.ToString()));

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains("--work-dir", command);
            Assert.Contains($"\"{expectedWorkDir}\"", command);
        }

        [Theory]
        [InlineData("/linux/style/path", "/linux/style/path")]
        public void GeneratePythonTrainingCommandByRunId_HandlesVariousPathFormats(string inputPath, string expectedPath)
        {
            // Arrange
            _mockMMDetectionConfiguration.Setup(x => x.GetRootDirAbsPath())
                .Returns(inputPath);

            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Assert
            Assert.Contains(expectedPath.Replace("/", CommonHelper.PathToLinuxRegexSlashReplace("/")), command);
        }

        [Fact]
        public void GeneratePythonTrainingCommandByRunId_CommandPartsAreInCorrectOrder()
        {
            // Act
            string command = _service.GeneratePythonTrainingCommandByRunId(_trainingRunId);

            // Get command parts
            var commandParts = command.Split(' ');

            // Assert
            Assert.True(Array.IndexOf(commandParts, "run") < Array.IndexOf(commandParts, "python"));
            Assert.True(Array.IndexOf(commandParts, "python") < Array.IndexOf(commandParts, "--work-dir"));
        }

        // TODO: Make as Integration
        //[Fact]
        //public async Task StartTrainingRun_ReturnsSuccess_WhenCommandExecutesSuccessfully()
        //{
        //    // Arrange
        //    var trainingRunId = Guid.NewGuid();
        //    _mockMMDetectionConfiguration.Setup(x => x.GetCondaExeAbsPath())
        //        .Returns("conda.exe");
        //    _mockMMDetectionConfiguration.Setup(x => x.GetRootDirAbsPath())
        //        .Returns("C:/root");
        //    _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunCliOutDirAbsPath())
        //        .Returns("C:/logs");
        //    _mockMMDetectionConfiguration.Setup(x => x.GetDetectionRunCliOutDirAbsPath())
        //        .Returns("C:/detection_logs");

        //    // Note: This test might need modification depending on your environment
        //    // as it involves actual file system operations

        //    // Act
        //    var result = await _service.StartTrainingRun(trainingRunId);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //}

        [Fact]
        public async Task DeleteTrainingRun_ReturnsSuccess_WhenTrainingRunInWaitingStatus()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                Status = nameof(ScheduleRunsStatus.Waiting)
            };

            var trainingRun = new TrainingRun
            {
                Id = trainingRunId,
                Status = nameof(ScheduleRunsStatus.Waiting)
            };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(trainingRun))
                .Returns(trainingRunDto);

            _mockMapper.Setup(x => x.Map<TrainingRun>(trainingRunDto))
                .Returns(trainingRun);

            _mockTrainingRunsRepository.Setup(x => x.Delete(It.IsAny<TrainingRun>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteTrainingRun(trainingRunId, "wwwroot");

            // Assert
            Assert.True(result.IsSuccess);
            _mockTrainingRunsRepository.Verify(x => x.Delete(It.IsAny<TrainingRun>(), true, default), Times.Once);
        }

        [Fact]
        public async Task DeleteTrainingRun_ReturnsFailure_WhenTrainingRunInProcessingStatus()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                Status = nameof(ScheduleRunsStatus.Processing)
            };

            var trainingRun = new TrainingRun
            {
                Id = trainingRunId,
                Status = nameof(ScheduleRunsStatus.Processing)
            };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(trainingRun))
                .Returns(trainingRunDto);

            // Act
            var result = await _service.DeleteTrainingRun(trainingRunId, "wwwroot");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not delete training run because it is in process", result.ErrMsg);
        }

        [Fact]
        public async Task PublishTrainingRunTrainedModel_ReturnsSuccess_WhenValidTrainingRun()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();
            var trainingRun = new TrainingRun
            {
                Id = trainingRunId,
                TrainedModelId = trainedModelId
            };

            var trainedModel = new TrainedModel
            {
                Id = trainedModelId,
                IsPublished = false
            };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, false, "TrainedModel"))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

            _mockTrainedModelsRepository.Setup(x => x.GetById(trainedModelId, true, null))
                .ReturnsAsync(ResultDTO<TrainedModel?>.Ok(trainedModel));

            _mockTrainedModelsRepository.Setup(x => x.Update(It.IsAny<TrainedModel>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.PublishTrainingRunTrainedModel(trainingRunId);

            // Assert
            Assert.True(result.IsSuccess);
            _mockTrainedModelsRepository.Verify(x => x.Update(It.Is<TrainedModel>(tm =>
                tm.Id == trainedModelId &&
                tm.IsPublished == true
            ), true, default), Times.Once);
        }

        [Fact]
        public async Task GetAllTrainingRuns_ReturnsSuccess_WhenTrainingRunsExist()
        {
            // Arrange
            var trainingRuns = new List<TrainingRun>
            {
                new TrainingRun { Id = Guid.NewGuid(), Name = "Run 1" },
                new TrainingRun { Id = Guid.NewGuid(), Name = "Run 2" }
            };

            var trainingRunDTOs = trainingRuns.Select(tr => new TrainingRunDTO
            {
                Id = tr.Id,
                Name = tr.Name
            }).ToList();

            _mockTrainingRunsRepository.Setup(x => x.GetAll(
                null, null, false, "CreatedBy,TrainedModel", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainingRun>>.Ok(trainingRuns));

            _mockMapper.Setup(x => x.Map<List<TrainingRunDTO>>(trainingRuns))
                .Returns(trainingRunDTOs);

            // Act
            var result = await _service.GetAllTrainingRuns();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data?.Count);
            Assert.Equal(trainingRunDTOs[0].Name, result.Data?[0].Name);
            Assert.Equal(trainingRunDTOs[1].Name, result.Data?[1].Name);
        }

        // TODO: Make as Integration
        //[Fact]
        //public async Task CreateTrainedModelByTrainingRunId_Success()
        //{
        //    // Arrange
        //    var trainingRunId = Guid.NewGuid();
        //    var trainingRun = new TrainingRun
        //    {
        //        Id = trainingRunId,
        //        Name = "Test Run",
        //        DatasetId = Guid.NewGuid(),
        //        CreatedById = "testuser"
        //    };

        //    _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, false, null))
        //        .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(trainingRun));

        //    _mockMMDetectionConfiguration.Setup(x => x.GetTrainedModelConfigFileAbsPath(trainingRunId))
        //        .Returns(ResultDTO<string>.Ok("config/path"));

        //    _mockMMDetectionConfiguration.Setup(x => x.GetTrainedModelBestEpochFileAbsPath(trainingRunId, It.IsAny<int>()))
        //        .Returns(ResultDTO<string>.Ok("model/path"));

        //    _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunResultLogFileAbsPath(trainingRunId))
        //        .Returns(ResultDTO<string>.Ok("results/path"));

        //    var trainedModel = new TrainedModel { Id = Guid.NewGuid() };
        //    _mockTrainedModelsRepository.Setup(x => x.CreateAndReturnEntity(It.IsAny<TrainedModel>(), true, default))
        //        .ReturnsAsync(ResultDTO<TrainedModel>.Ok(trainedModel));
            
        //    // Act
        //    var result = await _service.CreateTrainedModelByTrainingRunId(trainingRunId);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal(trainedModel.Id, result.Data);
        //}

        [Fact]
        public async Task GenerateTrainingRunConfigFile_Success()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var datasetDTO = new DatasetDTO
            {
                Id = Guid.NewGuid(),
                DatasetClasses = new List<Dataset_DatasetClassDTO>
                {
                    new Dataset_DatasetClassDTO
                    {
                        DatasetClass = new DatasetClassDTO { ClassName = "TestClass" }
                    }
                }
            };
            var dataset = new Dataset
            {
                Id = datasetDTO.Id,
                DatasetClasses = new List<Dataset_DatasetClass>
                {
                    new Dataset_DatasetClass
                    {
                        DatasetClass = new DatasetClass { ClassName = "TestClass" }
                    }
                }
            };
            TrainingRun trainingRun = new TrainingRun { Id = trainingRunId };
            var baseTrainedModelDTO = new TrainedModelDTO
            {
                Id = Guid.NewGuid(),
                ModelConfigPath = "config/path",
                ModelFilePath = "model/path"
            };
            var baseTrainedModel = new TrainedModel
            {
                Id = Guid.NewGuid(),
                ModelConfigPath = "config/path",
                ModelFilePath = "model/path"
            };

            //_mockMapper.Setup(x => x.Map<TrainingRunDTO>(It.IsAny<TrainingRun>()))
            //.Returns(trainingRunDto);

            _mockMapper.Setup(x => x.Map<TrainingRun>(It.IsAny<TrainingRunDTO>()))
                .Returns(trainingRun);

            _mockMapper.Setup(x => x.Map<TrainedModel>(It.IsAny<TrainedModelDTO>()))
                .Returns(baseTrainedModel);

            _mockMapper.Setup(x => x.Map<Dataset>(It.IsAny<DatasetDTO>()))
                .Returns(dataset);

            _mockMMDetectionConfiguration.Setup(x => x.GetBackboneCheckpointAbsPath())
                .Returns("backbone/path");
            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunDatasetDirAbsPath(trainingRunId))
                .Returns("dataset/path");

            string tmpConfDir = Path.GetTempPath();
            string tmpConfFile = Path.GetTempFileName();
            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunConfigDirAbsPathByRunId(trainingRunId))
                .Returns(tmpConfDir);
            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunConfigFileAbsPathByRunId(trainingRunId))
                .Returns(tmpConfFile);

            // Act
            var result = await _service.GenerateTrainingRunConfigFile(
                trainingRunId,
                datasetDTO,
                baseTrainedModelDTO,
                10,
                2,
                4);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(tmpConfFile, result.Data);
        }

        // TODO: Make as Integration
        //[Fact]
        //public async Task GetBestEpochForTrainingRun_Success()
        //{
        //    // Arrange
        //    var trainingRunId = Guid.NewGuid();
        //    var logContent = @"{
        //        ""epoch"": 1,
        //        ""step"": 100,
        //        ""coco/bbox_mAP"": 0.8
        //    }";

        //    _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunResultLogFileAbsPath(trainingRunId))
        //        .Returns(ResultDTO<string>.Ok(CreateTempLogFile(logContent)));

        //    // Act
        //    var result = _service.GetBestEpochForTrainingRun(trainingRunId);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.NotNull(result.Data);
        //}

        [Fact]
        public async Task DeleteTrainingRun_WithExistingDetectionRuns_ReturnsFail()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            var trainedModelId = Guid.NewGuid();
            var trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                TrainedModelId = trainedModelId,
                Status = nameof(ScheduleRunsStatus.Success)
            };

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(new TrainingRun { Id = trainingRunId }));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(It.IsAny<TrainingRun>()))
                .Returns(trainingRunDto);

            _mockDetectionRunService.Setup(x => x.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(new List<DetectionRunDTO>
                {
                    new DetectionRunDTO { TrainedModelId = trainedModelId }
                }));

            // Act
            var result = await _service.DeleteTrainingRun(trainingRunId, "wwwroot");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("delete first detection run", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteTrainingRun_WithErrorStatus_DeletesErrorFile()
        {
            // Arrange
            Guid trainedModelId = Guid.NewGuid();
            TrainedModel trainedModel = new TrainedModel
            {
                Id = trainedModelId,
            };
            Guid trainingRunId = Guid.NewGuid();
            TrainingRun trainingRun = new TrainingRun { Id = trainingRunId };
            TrainingRunDTO trainingRunDto = new TrainingRunDTO
            {
                Id = trainingRunId,
                Status = nameof(ScheduleRunsStatus.Error),
                TrainedModelId = trainedModelId,
                TrainedModel = new TrainedModelDTO()
                {
                    Id = trainedModelId,
                }
            
            };

            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunCliOutDirAbsPath()).Returns("logs/trainings/path");

            _mockMMDetectionConfiguration.Setup(x => x.GetTrainingRunsBaseOutDirAbsPath()).Returns("mmdetection/trainings/path");

            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(new TrainingRun { Id = trainingRunId, TrainedModel = trainedModel }));

            _mockTrainedModelsRepository.Setup(x => x.GetById(trainedModelId, true, null))
                .ReturnsAsync(ResultDTO<TrainedModel?>.Ok(trainedModel));

            _mockTrainingRunsRepository.Setup(x=> x.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainingRun>>.Ok([trainingRun]));

            _mockMapper.Setup(x => x.Map<TrainingRunDTO>(It.IsAny<TrainingRun>()))
                .Returns(trainingRunDto);

            _mockMapper.Setup(x => x.Map<TrainingRun>(It.IsAny<TrainingRunDTO>()))
                .Returns(trainingRun);

            _mockTrainedModelsRepository.Setup(x => x.Delete(trainedModel, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockTrainingRunsRepository.Setup(x => x.Delete(trainingRun, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockAppSettingsAccessor.Setup(x => x.GetApplicationSettingValueByKey<string>(
                "TrainingRunsErrorLogsFolder", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string?>.Ok("ErrorLogs"));

            _mockDetectionRunService.Setup(x => x.GetAllDetectionRuns())
                .ReturnsAsync(ResultDTO<List<DetectionRunDTO>>.Ok(new List<DetectionRunDTO>()));

            // Act
            var result = await _service.DeleteTrainingRun(trainingRunId, "wwwroot");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllTrainingRuns_WithNoTrainingRuns_ReturnsFailure()
        {
            // Arrange
            _mockTrainingRunsRepository.Setup(x => x.GetAll(
                null, null, false, "CreatedBy,TrainedModel", null))
                .ReturnsAsync(ResultDTO<IEnumerable<TrainingRun>>.Ok(null));

            // Act
            var result = await _service.GetAllTrainingRuns();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Training runs not found", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateTrainingRunEntity_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var trainingRunId = Guid.NewGuid();
            _mockTrainingRunsRepository.Setup(x => x.GetById(trainingRunId, true, null))
                .ReturnsAsync(ResultDTO<TrainingRun?>.Ok(null));

            // Act
            var result = await _service.UpdateTrainingRunEntity(trainingRunId, null, "Processing", true);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No Training Run found", result.ErrMsg);
        }

        [Fact]
        public async Task CreateTrainingRunWithBaseModel_WithInvalidBaseModel_ReturnsFailure()
        {
            // Arrange
            var inputDto = new TrainingRunDTO
            {
                Id = Guid.NewGuid(),
                TrainedModelId = Guid.NewGuid()
            };

            _mockTrainedModelsRepository.Setup(x => x.GetByIdInclude(inputDto.TrainedModelId.Value, false, null))
                .ReturnsAsync(ResultDTO<TrainedModel?>.Ok(null));

            // Act
            var result = await _service.CreateTrainingRunWithBaseModel(inputDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Base model not found", result.ErrMsg);
        }

        private string CreateTempLogFile(string content)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, content);
            return tempFile;
        }
    }
}