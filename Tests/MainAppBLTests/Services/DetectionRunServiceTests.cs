using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.Helpers;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Services.DetectionServices;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class DetectionRunServiceTests
    {
        private readonly Mock<IDetectionRunsRepository> _mockDetectionRunsRepository;
        private readonly Mock<IDetectedDumpSitesRepository> _mockDetectedDumSiteRepositoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<DetectionRunService>> _mockLogger;
        private readonly DetectionRunService _service;
        private readonly Mock<IFileSystem> _mockFileSystem;

        public DetectionRunServiceTests()
        {
            _mockDetectionRunsRepository = new Mock<IDetectionRunsRepository>();
            _mockDetectedDumSiteRepositoryRepository = new Mock<IDetectedDumpSitesRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DetectionRunService>>();
            _mockFileSystem = new Mock<IFileSystem>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:CondaExeFileAbsPath"]).Returns("conda_exe_file_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:CliLogsDirectoryAbsPath"]).Returns("cli_logs_directory_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:OutputBaseDirectoryMMDetectionRelPath"]).Returns("output_base_directory_detection_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:WorkingMMDetectionDirectoryAbsPath"]).Returns("working_detection_directory_abs_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:TrainedModelConfigFileRelPath"]).Returns("trained_model_config_file_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:TrainedModelModelFileRelPath"]).Returns("trained_model_model_file_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:DetectionResultDummyDatasetClassId"]).Returns("detection_result_dummy_dataset_class_id");
           
            _service = new DetectionRunService(_mockDetectionRunsRepository.Object,
                                               _mockMapper.Object,
                                               _mockLogger.Object,
                                               _mockConfiguration.Object,
                                               _mockDetectedDumSiteRepositoryRepository.Object);
        }

        [Fact]
        public async Task GetAllDetectionRuns_ReturnsListOfDetectionRunDTO()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>();
            _mockDetectionRunsRepository.Setup(repo => repo.GetAll(null,null,false,null,null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            // Act
            var result = await _service.GetAllDetectionRuns();

            // Assert
            Assert.False(result.IsSuccess, "Operation should be successful");
        }

        [Fact]
        public async Task GetAllDetectionRunsIncludingDetectedDumpSites_ReturnsListOfDetectionRunDTO()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>();
            _mockDetectionRunsRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            // Act
            var result = await _service.GetAllDetectionRunsIncludingDetectedDumpSites();

            // Assert
            Assert.False(result.IsSuccess, "Operation should be successful");
        }

        [Fact]
        public async Task GetSelectedDetectionRunsIncludingDetectedDumpSites_ReturnsListOfDetectionRunDTO()
        {
            // Arrange
            var selectedDetectionRunsIds = new List<Guid>(); 
            var detectionRuns = new List<DetectionRun>(); 
            _mockDetectionRunsRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            // Act
            var result = await _service.GetSelectedDetectionRunsIncludingDetectedDumpSites(selectedDetectionRunsIds);

            // Assert
            Assert.False(result.IsSuccess, "Operation should be successful");
        }

        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateData_ReturnsList()
        {
            // Arrange
            var selectedDetectionRunsIds = new List<Guid>();
            var detectionRuns = new List<DetectionRun>();
            _mockDetectionRunsRepository.Setup(repo => repo.GetSelectedDetectionRunsWithClasses(selectedDetectionRunsIds))
                                        .ReturnsAsync(detectionRuns);

            // Act
            var result = await _service.GenerateAreaComparisonAvgConfidenceRateData(selectedDetectionRunsIds);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<AreaComparisonAvgConfidenceRateReportDTO>>(result);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsDetectionRunDTO()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var detectionRunEntity = new DetectionRun();
            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunId, false, "CreatedBy"))
                                        .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(null));

            // Act
            var result = await _service.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.IsType<ResultDTO<DetectionRunDTO>>(result);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetDetectionRunById_ReturnsFailOnInvalidId()
        {
            // Arrange
            var invalidDetectionRunId = Guid.Empty;
            _mockDetectionRunsRepository.Setup(repo => repo.GetById(invalidDetectionRunId, false, "CreatedBy"))
                                        .ReturnsAsync(ResultDTO<DetectionRun?>.Fail("Invalid detection run ID"));

            // Act
            var result = await _service.GetDetectionRunById(invalidDetectionRunId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.IsType<ResultDTO<DetectionRunDTO>>(result);
            Assert.Null(result.Data);
            Assert.Equal("Invalid detection run ID", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionRun_SuccessfullyCreated()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            var detectionRunEntity = new DetectionRun();
            _mockMapper.Setup(m => m.Map<DetectionRun>(detectionRunDTO)).Returns(detectionRunEntity);

            _mockDetectionRunsRepository.Setup(repo => repo.CreateAndReturnEntity(detectionRunEntity, true, default))
                                        .ReturnsAsync(ResultDTO<DetectionRun>.Ok(detectionRunEntity));

            // Act
            var result = await _service.CreateDetectionRun(detectionRunDTO);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateDetectionRun_FailedToCreate()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            var detectionRunEntity = new DetectionRun();
            _mockMapper.Setup(m => m.Map<DetectionRun>(detectionRunDTO)).Returns(detectionRunEntity);

            _mockDetectionRunsRepository.Setup(repo => repo.CreateAndReturnEntity(detectionRunEntity, true, default))
                                        .ReturnsAsync(ResultDTO<DetectionRun>.Fail("Failed to create detection run"));

            // Act
            var result = await _service.CreateDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to create detection run", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionRun_ExceptionThrown()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            var detectionRunEntity = new DetectionRun();
            _mockMapper.Setup(m => m.Map<DetectionRun>(detectionRunDTO)).Returns(detectionRunEntity);

            var exceptionMessage = "An exception occurred";
            _mockDetectionRunsRepository.Setup(repo => repo.CreateAndReturnEntity(detectionRunEntity, true, default))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.CreateDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }

        [Fact]
        public async Task IsCompleteUpdateDetectionRun_SuccessfullyUpdated()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            var detectionRunEntity = new DetectionRun
            {
                Id = detectionRunDTO.Id.Value
            };

            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunDTO.Id.Value, true, null))
                                        .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(detectionRunEntity));

            _mockDetectionRunsRepository.Setup(repo => repo.Update(detectionRunEntity, true, default))
                                        .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.IsCompleteUpdateDetectionRun(detectionRunDTO);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task IsCompleteUpdateDetectionRun_FailedToUpdate()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            var detectionRunEntity = new DetectionRun
            {
                Id = detectionRunDTO.Id.Value
            };

            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunDTO.Id.Value, true, null))
                                        .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(detectionRunEntity));

            _mockDetectionRunsRepository.Setup(repo => repo.Update(detectionRunEntity, true, default))
                                        .ReturnsAsync(ResultDTO.Fail("Failed to update detection run"));

            // Act
            var result = await _service.IsCompleteUpdateDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to update detection run", result.ErrMsg);
        }

        [Fact]
        public async Task IsCompleteUpdateDetectionRun_DetectionRunNotFound()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunDTO.Id.Value, true, null))
                                        .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(null));

            // Act
            var result = await _service.IsCompleteUpdateDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No Detection Run found with ID: {detectionRunDTO.Id}", result.ErrMsg);
        }

        [Fact]
        public async Task IsCompleteUpdateDetectionRun_ExceptionThrown()
        {
            // Arrange
            var detectionRunDTO = new DetectionRunDTO
            {
                Id = Guid.NewGuid()
            };

            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunDTO.Id.Value, true, null))
                                        .ThrowsAsync(new Exception("An exception occurred"));

            // Act
            var result = await _service.IsCompleteUpdateDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
        }


        [Fact]
        public async Task GetRawDetectionRunResultPathsByRunId_Fail()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var imgFileExtension = ".jpg";
            var relDetectionRunResultsDirPath = Path.Combine("output_base_directory_detection_rel_path", detectionRunId.ToString());
            var absDetectionRunResultsDirPath = Path.Combine("working_detection_directory_abs_path", relDetectionRunResultsDirPath);
            var absDetectionRunResultsVizualizedDirPath = Path.Combine(absDetectionRunResultsDirPath, "vis");
            var absDetectionRunResultsBBoxesDirPath = Path.Combine(absDetectionRunResultsDirPath, "preds");
            var absDetectionRunResultsVizualizedFilePath = Path.Combine(absDetectionRunResultsVizualizedDirPath, detectionRunId.ToString() + imgFileExtension);
            var absDetectionRunResultsBBoxesFilePath = Path.Combine(absDetectionRunResultsBBoxesDirPath, detectionRunId.ToString() + ".json");

            _mockFileSystem.Setup(x => x.FileExists(absDetectionRunResultsVizualizedFilePath)).Returns(false);
            _mockFileSystem.Setup(x => x.FileExists(absDetectionRunResultsBBoxesFilePath)).Returns(false);

            // Act
            var result = await _service.GetRawDetectionRunResultPathsByRunId(detectionRunId, imgFileExtension);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No Visualized Detection Run Results Found", result.ErrMsg);
        }

        [Fact]
        public async Task ConvertBBoxResultToImageProjection_NullInputs()
        {
            // Arrange
            string absoluteImagePath = null;
            DetectionRunFinishedResponse detectionRunFinishedResponse = null;

            // Act
            var result = await _service.ConvertBBoxResultToImageProjection(absoluteImagePath, detectionRunFinishedResponse);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"{nameof(absoluteImagePath)} is null or empty", result.ErrMsg);
        }


    }
}
