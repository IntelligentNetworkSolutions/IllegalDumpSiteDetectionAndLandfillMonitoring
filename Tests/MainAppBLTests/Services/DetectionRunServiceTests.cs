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
using SD.Helpers;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class DetectionRunServiceTests
    {
        private readonly Mock<IDetectionRunsRepository> _mockDetectionRunsRepository;
        private readonly Mock<IDetectedDumpSitesRepository> _mockDetectedDumSiteRepositoryRepository;
        private readonly Mock<IDetectionInputImageRepository> _mockDetectionInputImageRepository;
        private readonly Mock<IDetectionIgnoreZonesRepository> _mockDetectionIgnoreZoneRepository;
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
            _mockDetectionIgnoreZoneRepository = new Mock<IDetectionIgnoreZonesRepository>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:CondaExeFileAbsPath"]).Returns("conda_exe_file_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:CliLogsDirectoryAbsPath"]).Returns("cli_logs_directory_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:OutputBaseDirectoryMMDetectionRelPath"]).Returns("output_base_directory_detection_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:WorkingMMDetectionDirectoryAbsPath"]).Returns("working_detection_directory_abs_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:TrainedModelConfigFileRelPath"]).Returns("trained_model_config_file_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:TrainedModelModelFileRelPath"]).Returns("trained_model_model_file_rel_path");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:DetectionResultDummyDatasetClassId"]).Returns("detection_result_dummy_dataset_class_id");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:OpenMMLabAbsPath"]).Returns("OpenMMLabAbsPath");
            _mockConfiguration.Setup(x => x["AppSettings:MMDetection:HasGPU"]).Returns("false");

            _service = new DetectionRunService(_mockDetectionRunsRepository.Object,
                                               _mockMapper.Object,
                                               _mockLogger.Object,
                                               _mockConfiguration.Object,
                                               _mockDetectedDumSiteRepositoryRepository.Object,
                                                _mockDetectionInputImageRepository.Object,
                                                _mockMMDetectionConfigurationService.Object,
                                                _mockDetectionIgnoreZoneRepository.Object);
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
        public async Task GetDetectionRunById_ThrowsException_ReturnsExceptionFailResult()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var exceptionMessage = "An unexpected error occurred.";

            _mockDetectionRunsRepository.Setup(repo => repo.GetById(detectionRunId, false, "CreatedBy"))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetDetectionRunById(detectionRunId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.IsType<ResultDTO<DetectionRunDTO>>(result);
            Assert.Null(result.Data);
            Assert.Equal(exceptionMessage, result.ErrMsg);
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


        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateData_ShouldReturnFail_WhenNoDetectionRunsFound()
        {
            // Arrange
            var selectedIds = new List<Guid> { Guid.NewGuid() };
            int selectedConfidenceRate = 70;

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetSelectedDetectionRunsWithClasses(selectedIds))
                .ReturnsAsync(new List<DetectionRun>());

            // Act
            var result = await _service.GenerateAreaComparisonAvgConfidenceRateData(selectedIds, selectedConfidenceRate);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No detection runs found.", result.ErrMsg);
        }

        [Fact]
        public async Task GenerateAreaComparisonAvgConfidenceRateData_ShouldReturnFail_WhenIgnoreZonesNotFound()
        {
            // Arrange
            var selectedIds = new List<Guid> { Guid.NewGuid() };
            int selectedConfidenceRate = 70;
            var detectionRuns = new List<DetectionRun>
            {
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run1", DetectedDumpSites = new List<DetectedDumpSite>() }
            };

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetSelectedDetectionRunsWithClasses(selectedIds))
                .ReturnsAsync(detectionRuns);

            _mockDetectionIgnoreZoneRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionIgnoreZone>>.Fail("Ignore zones retrieval failed"));

            // Act
            var result = await _service.GenerateAreaComparisonAvgConfidenceRateData(selectedIds, selectedConfidenceRate);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Ignore zones retrieval failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedInputImagesById_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var selectedImagesIds = new List<Guid> { Guid.NewGuid() };
            string exceptionMessage = "Repository exception";

            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionInputImage, bool>>>(), null, false, It.IsAny<string>(), null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetSelectedInputImagesById(selectedImagesIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedInputImagesById_ShouldReturnFail_WhenRepositoryReturnsError()
        {
            // Arrange
            var selectedImagesIds = new List<Guid> { Guid.NewGuid() };
            string errorMessage = "Repository error";

            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionInputImage, bool>>>(), null, false, It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionInputImage>>.Fail(errorMessage));

            // Act
            var result = await _service.GetSelectedInputImagesById(selectedImagesIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedInputImagesById_ShouldReturnOk_WhenImagesAreRetrievedSuccessfully()
        {
            // Arrange
            var selectedImagesIds = new List<Guid> { Guid.NewGuid() };
            var detectionImages = new List<DetectionInputImage>
            {
                new DetectionInputImage
                {
                    Id = selectedImagesIds.First(),
                    ImageFileName = "Image1.jpg",
                    CreatedById = "test-user-id"
                }
            };

            _mockDetectionInputImageRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionInputImage, bool>>>(), null, false, It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionInputImage>>.Ok(detectionImages));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionInputImageDTO>>(detectionImages))
                .Returns(new List<DetectionInputImageDTO>
                {
            new DetectionInputImageDTO { Id = selectedImagesIds.First(), ImageFileName = "Image1.jpg" }
                });

            // Act
            var result = await _service.GetSelectedInputImagesById(selectedImagesIds);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Image1.jpg", result.Data.First().ImageFileName);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var status = "Completed";
            var detectionRun = new DetectionRun { Id = detectionRunId, Status = "Pending" };

            // Setup repository to return the detection run for GetById
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetById(detectionRunId, true, null))
                .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(detectionRun));

            _mockDetectionRunsRepository
                .Setup(repo => repo.Update(detectionRun, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.UpdateStatus(detectionRunId, status);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnFail_WhenDetectionRunNotFound()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var status = "Completed";

            // Setup repository to return failure for GetById
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetById(detectionRunId, true, null))
                .ReturnsAsync(ResultDTO<DetectionRun?>.Fail("Detection run not found"));

            // Act
            var result = await _service.UpdateStatus(detectionRunId, status);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Detection run not found", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnFail_WhenUpdateFails()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var status = "Completed";
            var detectionRun = new DetectionRun { Id = detectionRunId, Status = "Pending" };

            // Setup repository to return the detection run for GetById
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetById(detectionRunId, true, null))
                .ReturnsAsync(ResultDTO<DetectionRun?>.Ok(detectionRun));

            // Setup repository to return failure for Update
            _mockDetectionRunsRepository
                .Setup(repo => repo.Update(detectionRun, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.UpdateStatus(detectionRunId, status);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var status = "Completed";
            string exceptionMessage = "Repository exception";

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetById(detectionRunId, true, null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.UpdateStatus(detectionRunId, status);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);

        }

        [Fact]
        public async Task GetDetectionRunsWithClasses_ShouldReturnListOfDTOs_WhenSuccessful()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>
            {
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 1" },
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 2" }
            };

            var expectedDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = detectionRuns[0].Id, Name = "Run 1" },
                new DetectionRunDTO { Id = detectionRuns[1].Id, Name = "Run 2" }
            };

            // Setup repository to return detection runs
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRuns);

            // Setup mapper to return mapped DTOs
            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(detectionRuns))
                .Returns(expectedDTOs);

            // Act
            var result = await _service.GetDetectionRunsWithClasses();

            // Assert
            Assert.Equal(expectedDTOs.Count, result.Count);
            Assert.Equal(expectedDTOs[0].Name, result[0].Name);
            Assert.Equal(expectedDTOs[1].Name, result[1].Name);
        }

        [Fact]
        public async Task GetDetectionRunsWithClasses_ShouldThrowException_WhenRepositoryReturnsNull()
        {
            // Arrange
            string exceptionMessage = "Object not found";

            // Setup repository to return null
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetDetectionRunsWithClasses())
                .ReturnsAsync((List<DetectionRun>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetDetectionRunsWithClasses());
            Assert.Equal(exceptionMessage, exception.Message);

        }

        [Fact]
        public async Task GetDetectionRunsWithClasses_ShouldThrowException_WhenMapperReturnsNull()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>
            {
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 1" }
            };
            string exceptionMessage = "Object not found";

            // Setup repository to return detection runs
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetDetectionRunsWithClasses())
                .ReturnsAsync(detectionRuns);

            // Setup mapper to return null
            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(detectionRuns))
                .Returns((List<DetectionRunDTO>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetDetectionRunsWithClasses());
            Assert.Equal(exceptionMessage, exception.Message);

        }

        [Fact]
        public async Task DeleteDetectionRun_ShouldReturnOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();

            var detectionRunDTO = new DetectionRunDTO { Id = detectionRunId, Name = "Test Run" };
            var detectionRun = new DetectionRun { Id = detectionRunId, Name = detectionRunDTO.Name };

            // Set up the mapper to map from DTO to entity
            _mockMapper
                .Setup(mapper => mapper.Map<DetectionRun>(detectionRunDTO))
                .Returns(detectionRun);

            // Set up the repository to return a successful result on deletion
            _mockDetectionRunsRepository
                .Setup(repo => repo.Delete(detectionRun, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDetectionRun(detectionRunDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionRun_ShouldReturnFail_WhenDeletionFails()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();

            var detectionRunDTO = new DetectionRunDTO { Id = detectionRunId, Name = "Test Run" };
            var detectionRun = new DetectionRun { Id = detectionRunId, Name = detectionRunDTO.Name };
            string errorMessage = "Deletion failed due to repository error.";

            // Set up the mapper to map from DTO to entity
            _mockMapper
                .Setup(mapper => mapper.Map<DetectionRun>(detectionRunDTO))
                .Returns(detectionRun);

            // Set up the repository to return a failure result on deletion
            _mockDetectionRunsRepository
                .Setup(repo => repo.Delete(detectionRun, true, default))
                .ReturnsAsync(ResultDTO.Fail(errorMessage));

            // Act
            var result = await _service.DeleteDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDetectionRun_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var detectionRunDTO = new DetectionRunDTO { Id = detectionRunId, Name = "Test Run" };
            var detectionRun = new DetectionRun { Id = detectionRunId, Name = detectionRunDTO.Name };
            string exceptionMessage = "An error occurred during deletion.";

            // Set up the mapper to map from DTO to entity
            _mockMapper
                .Setup(mapper => mapper.Map<DetectionRun>(detectionRunDTO))
                .Returns(detectionRun);

            // Set up the repository to throw an exception
            _mockDetectionRunsRepository
                .Setup(repo => repo.Delete(detectionRun, true, default))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.DeleteDetectionRun(detectionRunDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);

        }

        [Fact]
        public async Task GetAllDetectionRuns_ShouldReturnFail_WhenGetAllFails()
        {
            // Arrange
            string errorMessage = "Failed to retrieve Detection Runs";

            // Set up the repository to return a failure result
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Fail(errorMessage));

            // Act
            var result = await _service.GetAllDetectionRuns();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDetectionRuns_ShouldReturnOkWithMappedDTOs_WhenGetAllIsSuccessful()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>
            {
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 1" },
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 2" }
            };
            var detectionRunDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = detectionRuns[0].Id, Name = detectionRuns[0].Name },
                new DetectionRunDTO { Id = detectionRuns[1].Id, Name = detectionRuns[1].Name }
            };

            // Set up the repository to return a successful result with detection runs
            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            // Set up the mapper to map detection runs to DTOs
            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(detectionRuns))
                .Returns(detectionRunDTOs);

            // Act
            var result = await _service.GetAllDetectionRuns();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
            Assert.Equal(detectionRunDTOs.Count, result.Data.Count);
            Assert.Equal(detectionRunDTOs[0].Name, result.Data[0].Name);
            Assert.Equal(detectionRunDTOs[1].Name, result.Data[1].Name);
        }

        [Fact]
        public async Task GetAllDetectionRunsIncludingDetectedDumpSites_ShouldReturnFail_WhenGetAllFails()
        {
            // Arrange
            string errorMessage = "Failed to retrieve Detection Runs with Detected Dump Sites";

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, "CreatedBy,DetectedDumpSites", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Fail(errorMessage));

            // Act
            var result = await _service.GetAllDetectionRunsIncludingDetectedDumpSites();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDetectionRunsIncludingDetectedDumpSites_ShouldReturnOkWithMappedDTOs_WhenGetAllIsSuccessful()
        {
            // Arrange
            var detectionRuns = new List<DetectionRun>
            {
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 1", DetectedDumpSites = new List<DetectedDumpSite> { /* example detected sites */ }},
                new DetectionRun { Id = Guid.NewGuid(), Name = "Run 2", DetectedDumpSites = new List<DetectedDumpSite> { /* example detected sites */ }}
            };
            var detectionRunDTOs = new List<DetectionRunDTO>
            {
                new DetectionRunDTO { Id = detectionRuns[0].Id, Name = detectionRuns[0].Name },
                new DetectionRunDTO { Id = detectionRuns[1].Id, Name = detectionRuns[1].Name }
            };

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, "CreatedBy,DetectedDumpSites", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(detectionRuns))
                .Returns(detectionRunDTOs);

            // Act
            var result = await _service.GetAllDetectionRunsIncludingDetectedDumpSites();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
            Assert.Equal(detectionRunDTOs.Count, result.Data.Count);
            Assert.Equal(detectionRunDTOs[0].Name, result.Data[0].Name);
            Assert.Equal(detectionRunDTOs[1].Name, result.Data[1].Name);
        }

        [Fact]
        public async Task GetSelectedDetectionRunsIncludingDetectedDumpSites_ShouldReturnFail_WhenGetAllFails()
        {
            // Arrange
            string errorMessage = "Failed to retrieve selected Detection Runs with Detected Dump Sites";

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, "CreatedBy,DetectedDumpSites,DetectionInputImage", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Fail(errorMessage));

            // Act
            var result = await _service.GetSelectedDetectionRunsIncludingDetectedDumpSites(new List<Guid> { Guid.NewGuid() }, new List<ConfidenceRateDTO>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetSelectedDetectionRunsIncludingDetectedDumpSites_ShouldReturnFilteredDTOs_WhenConfidenceThresholdIsApplied()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var selectedDetectionRunIds = new List<Guid> { detectionRunId };
            var selectedConfidenceRates = new List<ConfidenceRateDTO>
    {
        new ConfidenceRateDTO { detectionRunId = detectionRunId, confidenceRate = 80 }
    };

            var detectionRuns = new List<DetectionRun>
    {
        new DetectionRun
        {
            Id = detectionRunId,
            DetectedDumpSites = new List<DetectedDumpSite>
            {
                new DetectedDumpSite { ConfidenceRate = 0.9 }, // Should be included
                new DetectedDumpSite { ConfidenceRate = 0.7 }  // Should be filtered out
            }
        }
    };

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, "CreatedBy,DetectedDumpSites,DetectionInputImage", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(It.Is<List<DetectionRun>>(dr => dr[0].DetectedDumpSites.Count == 1)))
                .Returns(new List<DetectionRunDTO> { new DetectionRunDTO { Id = detectionRunId } });

            // Act
            var result = await _service.GetSelectedDetectionRunsIncludingDetectedDumpSites(selectedDetectionRunIds, selectedConfidenceRates);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
            Assert.Single(result.Data); // Only one DTO expected
            Assert.Equal(detectionRunId, result.Data[0].Id);
        }

        [Fact]
        public async Task GetSelectedDetectionRunsIncludingDetectedDumpSites_ShouldReturnOkWithMappedDTOs_WhenGetAllIsSuccessfulWithoutFiltering()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var selectedDetectionRunIds = new List<Guid> { detectionRunId };
            var selectedConfidenceRates = new List<ConfidenceRateDTO>(); // No filtering applied

            var detectionRuns = new List<DetectionRun>
    {
        new DetectionRun
        {
            Id = detectionRunId,
            DetectedDumpSites = new List<DetectedDumpSite>
            {
                new DetectedDumpSite { ConfidenceRate = 0.9 },
                new DetectedDumpSite { ConfidenceRate = 0.7 }
            }
        }
    };

            _mockDetectionRunsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DetectionRun, bool>>>(), null, false, "CreatedBy,DetectedDumpSites,DetectionInputImage", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DetectionRun>>.Ok(detectionRuns));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DetectionRunDTO>>(detectionRuns))
                .Returns(new List<DetectionRunDTO> { new DetectionRunDTO { Id = detectionRunId } });

            // Act
            var result = await _service.GetSelectedDetectionRunsIncludingDetectedDumpSites(selectedDetectionRunIds, selectedConfidenceRates);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
            Assert.Single(result.Data);
            Assert.Equal(detectionRunId, result.Data[0].Id);
        }

        [Fact]
        public void GeneratePythonDetectionCommandByType_SmallImage_WithGPU_GeneratesCorrectCommand()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var imagePath = @"C:\images\test.jpg";
            var configPath = @"C:\models\config.py";
            var modelPath = @"C:\models\model.pth";

            var expectedImagePath = CommonHelper.PathToLinuxRegexSlashReplace(imagePath);
            var expectedConfigPath = CommonHelper.PathToLinuxRegexSlashReplace(configPath);
            var expectedModelPath = CommonHelper.PathToLinuxRegexSlashReplace(modelPath);
            var expectedOutDir = CommonHelper.PathToLinuxRegexSlashReplace("/path/to/output");
            var expectedScript = CommonHelper.PathToLinuxRegexSlashReplace("demo\\image_demo.py");

            var expectedCommand = $"run -p /path/to/openmmlab python {expectedScript} " +
                $"\"{expectedImagePath}\" {expectedConfigPath} --weights {expectedModelPath} " +
                $"--out-dir {expectedOutDir}  ";

            _mockMMDetectionConfigurationService.Setup(x => x.GetOpenMMLabAbsPath())
                .Returns("/path/to/openmmlab");
            _mockMMDetectionConfigurationService.Setup(x => x.GetDetectionRunOutputDirAbsPathByRunId(detectionRunId))
                .Returns(expectedOutDir);

            // Act
            var result = _service.GeneratePythonDetectionCommandByType(
                imagePath,
                configPath,
                modelPath,
                detectionRunId,
                isSmallImage: true,
                hasGPU: true
            );

            // Assert
            Assert.Equal(expectedCommand, result);
        }

        [Fact]
        public void GeneratePythonDetectionCommandByType_LargeImage_WithoutGPU_GeneratesCorrectCommand()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var imagePath = @"C:\images\large.jpg";
            var configPath = @"C:\models\config.py";
            var modelPath = @"C:\models\model.pth";

            var expectedScript = CommonHelper.PathToLinuxRegexSlashReplace("ins_development\\scripts\\large_image_annotated_inference.py");
            var expectedImagePath = CommonHelper.PathToLinuxRegexSlashReplace(imagePath);
            var expectedConfigPath = CommonHelper.PathToLinuxRegexSlashReplace(configPath);
            var expectedModelPath = CommonHelper.PathToLinuxRegexSlashReplace(modelPath);
            var expectedOutDir = CommonHelper.PathToLinuxRegexSlashReplace("/path/to/output");

            var expectedCommand = $"run -p /path/to/openmmlab python {expectedScript} " +
                $"\"{expectedImagePath}\" {expectedConfigPath}  {expectedModelPath} " +
                $"--out-dir {expectedOutDir} --device cpu --patch-size 1280";

            _mockMMDetectionConfigurationService.Setup(x => x.GetOpenMMLabAbsPath())
                .Returns("/path/to/openmmlab");
            _mockMMDetectionConfigurationService.Setup(x => x.GetDetectionRunOutputDirAbsPathByRunId(detectionRunId))
                .Returns(expectedOutDir);

            // Act
            var result = _service.GeneratePythonDetectionCommandByType(
                imagePath,
                configPath,
                modelPath,
                detectionRunId,
                isSmallImage: false,
                hasGPU: false
            );

            // Assert
            Assert.Equal(expectedCommand, result);
        }

        [Fact]
        public void GeneratePythonDetectionCommandByType_VerifiesPathsAreConvertedToLinuxFormat()
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var imagePath = @"C:\path\with\windows\slashes\image.jpg";
            var configPath = @"D:\another\path\config.py";
            var modelPath = @"E:\model\path\model.pth";

            // Act
            var result = _service.GeneratePythonDetectionCommandByType(
                imagePath,
                configPath,
                modelPath,
                detectionRunId,
                isSmallImage: false,
                hasGPU: true
            );

            // Assert
            Assert.Contains(CommonHelper.PathToLinuxRegexSlashReplace(imagePath), result);
            Assert.Contains(CommonHelper.PathToLinuxRegexSlashReplace(configPath), result);
            Assert.Contains(CommonHelper.PathToLinuxRegexSlashReplace(modelPath), result);
        }

        [Theory]
        [InlineData(true, true)]   // Small image with GPU
        [InlineData(true, false)]  // Small image without GPU
        [InlineData(false, true)]  // Large image with GPU
        [InlineData(false, false)] // Large image without GPU
        public void GeneratePythonDetectionCommandByType_VariousConfigurations_GeneratesValidCommands(
            bool isSmallImage, bool hasGPU)
        {
            // Arrange
            var detectionRunId = Guid.NewGuid();
            var imagePath = @"C:\test\image.jpg";
            var configPath = @"C:\test\config.py";
            var modelPath = @"C:\test\model.pth";

            // Act
            var result = _service.GeneratePythonDetectionCommandByType(
                imagePath,
                configPath,
                modelPath,
                detectionRunId,
                isSmallImage,
                hasGPU
            );

            // Assert
            if (isSmallImage)
            {
                Assert.Contains("demo/image_demo.py", result);
                Assert.Contains("--weights", result);
                Assert.DoesNotContain("--patch-size", result);
            }
            else
            {
                Assert.Contains("large_image_annotated_inference.py", result);
                Assert.DoesNotContain("--weights", result);
                Assert.Contains("--patch-size 1280", result);
            }

            if (!hasGPU)
            {
                Assert.Contains("--device cpu", result);
            }
            else
            {
                Assert.DoesNotContain("--device cpu", result);
            }

            // Verify essential command parts
            Assert.Contains("run -p", result);
            Assert.Contains("python", result);
            Assert.Contains("--out-dir", result);
        }

    }
}



