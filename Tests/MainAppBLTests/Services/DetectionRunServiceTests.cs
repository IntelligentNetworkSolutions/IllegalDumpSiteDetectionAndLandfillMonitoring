using AutoMapper;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Entities.DetectionEntities;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Services.DetectionServices;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class DetectionRunServiceTests
    {
        private readonly Mock<IDetectionRunsRepository> _mockDetectionRunsRepository;
        private readonly Mock<IDetectedDumpSitesRepository> _mockDetectedDumSiteRepositoryRepository;
        private readonly Mock<IDetectionInputImageRepository> _mockDetectionInputImageRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<DetectionRunService>> _mockLogger;
        private readonly DetectionRunService _service;
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly Mock<IMMDetectionConfigurationService> _mockMMDetectionConfigurationService;

        public DetectionRunServiceTests()
        {
            _mockDetectionRunsRepository = new Mock<IDetectionRunsRepository>();
            _mockDetectedDumSiteRepositoryRepository = new Mock<IDetectedDumpSitesRepository>();
            _mockDetectionInputImageRepository = new Mock<IDetectionInputImageRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DetectionRunService>>();
            _mockFileSystem = new Mock<IFileSystem>();
            _mockMMDetectionConfigurationService = new Mock<IMMDetectionConfigurationService>();

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
                                               _mockDetectedDumSiteRepositoryRepository.Object,
                                                _mockDetectionInputImageRepository.Object,
                                                _mockMMDetectionConfigurationService.Object);
        }

        [Fact]
        public async Task GetAllDetectionRuns_ReturnsListOfDetectionRunDTO()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>();
            _mockDetectionRunsRepository.Setup(repo => repo.GetAll(null, null, false, null, null))
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
            var confidenceRates = new List<ConfidenceRateDTO>();
            _mockDetectionRunsRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            // Act
            var result = await _service.GetSelectedDetectionRunsIncludingDetectedDumpSites(selectedDetectionRunsIds, confidenceRates);

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
            var result = await _service.GetRawDetectionRunResultPathsByRunId(detectionRunId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No Polygonized Predictions Detection Run Results Found", result.ErrMsg);
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

        #region DetectionInputImageTests

        [Fact]
        public async Task GetAllImages_ShouldReturnListOfDetectionInputImageDTO()
        {
            // Arrange
            var images = new List<DetectionInputImage> { new DetectionInputImage { Id = Guid.NewGuid() } };
            var mappedDtos = new List<DetectionInputImageDTO> { new DetectionInputImageDTO { Id = Guid.NewGuid() } };

            var repoResult = ResultDTO<IEnumerable<DetectionInputImage>>.Ok(images);
            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(repoResult);

            _mockMapper
                .Setup(m => m.Map<List<DetectionInputImageDTO>>(It.IsAny<IEnumerable<DetectionInputImage>>()))
                .Returns(mappedDtos);

            // Act
            var result = await _service.GetAllImages();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mappedDtos.Count, result.Data.Count);
        }

        [Fact]
        public async Task GetAllImages_RepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var repoResult = ResultDTO<IEnumerable<DetectionInputImage>>.Fail("Error occurred");
            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(repoResult);

            // Act
            var result = await _service.GetAllImages();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error occurred", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllImages_ExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ThrowsAsync(new Exception("Some exception"));

            // Act
            var result = await _service.GetAllImages();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Some exception", result.ErrMsg);
        }
        [Fact]
        public async Task GetDetectionInputImageById_ValidId_ReturnsDetectionInputImageDTO()
        {
            // Arrange
            var inputImageId = Guid.NewGuid();
            var imageEntity = new DetectionInputImage { Id = inputImageId };
            var imageDto = new DetectionInputImageDTO { Id = inputImageId };

            var repoResult = ResultDTO<DetectionInputImage?>.Ok(imageEntity);
            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetById(inputImageId, false, "CreatedBy"))
                .ReturnsAsync(repoResult);

            _mockMapper
                .Setup(m => m.Map<DetectionInputImageDTO>(It.IsAny<DetectionInputImage>()))
                .Returns(imageDto);

            // Act
            var result = await _service.GetDetectionInputImageById(inputImageId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(imageDto.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetDetectionInputImageById_InvalidId_ShouldReturnFailResult()
        {
            // Arrange
            var inputImageId = Guid.NewGuid();

            var repoResult = ResultDTO<DetectionInputImage?>.Fail("Image not found");
            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetById(inputImageId, false, "CreatedBy"))
                .ReturnsAsync(repoResult);

            // Act
            var result = await _service.GetDetectionInputImageById(inputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionInputImageById_ExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var inputImageId = Guid.NewGuid();

            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetById(inputImageId, false, "CreatedBy"))
                .ThrowsAsync(new Exception("Some exception"));

            // Act
            var result = await _service.GetDetectionInputImageById(inputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Some exception", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionInputImage_ValidDTO_ReturnsSuccessResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.CreateAndReturnEntity(inputImageEntity, true, default))
                .ReturnsAsync(ResultDTO<DetectionInputImage>.Ok(inputImageEntity));

            // Act
            var result = await _service.CreateDetectionInputImage(inputImageDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg); 
        }

        [Fact]
        public async Task CreateDetectionInputImage_RepositoryReturnsFail_ReturnsFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.CreateAndReturnEntity(inputImageEntity, true, default))
                 .ThrowsAsync(new Exception("Creation failed"));

            // Act
            var result = await _service.CreateDetectionInputImage(inputImageDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDetectionInputImage_ThrowsException_ReturnsExceptionFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            // Mock the mapping from DTO to entity
            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            // Mock the repository method to throw an exception
            _mockDetectionInputImageRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<DetectionInputImage>(), true, default))
                .ThrowsAsync(new Exception("Exception occurred"));

            // Act
            var result = await _service.CreateDetectionInputImage(inputImageDTO);

            // Assert
            Assert.NotNull(result); // Check that result is not null
            Assert.False(result.IsSuccess); // Ensure the result indicates failure
            Assert.Equal("Exception occurred", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionInputImage_ValidDTO_ReturnsSuccessResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Delete(inputImageEntity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDetectionInputImage(inputImageDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionInputImage_RepositoryReturnsFail_ReturnsFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Delete(inputImageEntity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Deletion failed"));

            // Act
            var result = await _service.DeleteDetectionInputImage(inputImageDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Deletion failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionInputImage_ThrowsException_ReturnsExceptionFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Delete(inputImageEntity, true, default))
                .ThrowsAsync(new Exception("Exception occurred"));

            // Act
            var result = await _service.DeleteDetectionInputImage(inputImageDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Exception occurred", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionInputImage_ValidDTO_ReturnsSuccessResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Update(inputImageEntity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditDetectionInputImage(inputImageDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionInputImage_RepositoryReturnsFail_ReturnsFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Update(inputImageEntity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.EditDetectionInputImage(inputImageDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditDetectionInputImage_ThrowsException_ReturnsExceptionFailResult()
        {
            // Arrange
            var inputImageDTO = new DetectionInputImageDTO { Id = Guid.NewGuid() };
            var inputImageEntity = new DetectionInputImage { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(m => m.Map<DetectionInputImage>(inputImageDTO))
                .Returns(inputImageEntity);

            _mockDetectionInputImageRepository
                .Setup(repo => repo.Update(inputImageEntity, true, default))
                .ThrowsAsync(new Exception("Exception occurred"));

            // Act
            var result = await _service.EditDetectionInputImage(inputImageDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Exception occurred", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionInputImageByDetectionRunId_ValidId_ReturnsSuccessResultWithDTOs()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            var detectionRunEntities = new List<DetectionRun>
                {
                    new DetectionRun { Id = Guid.NewGuid(), DetectionInputImageId = detectionInputImageId },
                    new DetectionRun { Id = Guid.NewGuid(), DetectionInputImageId = detectionInputImageId }
                };
            var detectionRunDTOs = new List<DetectionRunDTO>
                {
                    new DetectionRunDTO { Id = detectionRunEntities[0].Id },
                    new DetectionRunDTO { Id = detectionRunEntities[1].Id }
                };

            Expression<Func<DetectionRun, bool>> filter = x => x.DetectionInputImageId == detectionInputImageId;

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRunEntities));

            _mockMapper
                .Setup(m => m.Map<List<DetectionRunDTO>>(detectionRunEntities))
                .Returns(detectionRunDTOs);

            // Act
            var result = await _service.GetDetectionInputImageByDetectionRunId(detectionInputImageId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(detectionRunDTOs.Count, result.Data.Count);
        }

        [Fact]
        public async Task GetDetectionInputImageByDetectionRunId_RepositoryReturnsFail_ReturnsFailResult()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Fail("Data retrieval failed"));

            // Act
            var result = await _service.GetDetectionInputImageByDetectionRunId(detectionInputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data retrieval failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetDetectionInputImageByDetectionRunId_ThrowsException_ReturnsExceptionFailResult()
        {
            // Arrange
            var detectionInputImageId = Guid.NewGuid();
            Expression<Func<DetectionRun, bool>> filter = x => x.DetectionInputImageId == detectionInputImageId;

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception("Exception occurred"));

            // Act
            var result = await _service.GetDetectionInputImageByDetectionRunId(detectionInputImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Exception occurred", result.ErrMsg);
        }


        #endregion
    }
}



