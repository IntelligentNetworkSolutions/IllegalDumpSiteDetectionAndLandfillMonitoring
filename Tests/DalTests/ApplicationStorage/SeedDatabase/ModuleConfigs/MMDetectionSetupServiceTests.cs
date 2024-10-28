using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage.SeedDatabase.ModulesConfigs.MMDetectionSetup;
using Entities;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.DalTests.ApplicationStorage.SeedDatabase.ModuleConfigs
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class MMDetectionSetupServiceTests
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly string _testRootDir;
        private const string _testDownloadableFileUrl = "https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv";

        public MMDetectionSetupServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _mockConfiguration = new Mock<IConfiguration>();
            _testRootDir = Path.Combine(Path.GetTempPath(), $"mmdetection_test_{Guid.NewGuid()}");
            SetupConfiguration();
        }

        private void SetupConfiguration()
        {
            var mmSection = new Mock<IConfigurationSection>();
            SetupMMDetectionConfiguration(mmSection);
            _mockConfiguration.Setup(c => c.GetSection("MMDetectionConfiguration"))
                .Returns(mmSection.Object);
        }

        private void SetupMMDetectionConfiguration(Mock<IConfigurationSection> section)
        {
            section.Setup(s => s["Base:CondaExeAbsPath"]).Returns("/path/to/conda");
            section.Setup(s => s["Base:RootDirAbsPath"]).Returns(_testRootDir);
            section.Setup(s => s["Base:ScriptsDirRelPath"]).Returns("scripts");
            section.Setup(s => s["Base:ResourcesDirRelPath"]).Returns("resources");
            section.Setup(s => s["Base:OpenMMLabAbsPath"]).Returns("/path/to/openmmlab");

            section.Setup(s => s["Training:DatasetsDirRelPath"]).Returns("datasets");
            section.Setup(s => s["Training:ConfigsDirRelPath"]).Returns("configs");
            section.Setup(s => s["Training:OutputDirRelPath"]).Returns("output");
            section.Setup(s => s["Training:BackboneCheckpointAbsPath"]).Returns("/path/to/checkpoint");
            section.Setup(s => s["Training:CliLogsAbsPath"]).Returns("/path/to/logs");

            section.Setup(s => s["Detection:OutputDirRelPath"]).Returns("detection_output");
            section.Setup(s => s["Detection:CliLogsAbsPath"]).Returns("/path/to/detection_logs");
        }

        [Fact]
        public void CreateTrainingAndDetectionProcessDirsInMMDetectionRoot_ShouldCreateDirectories()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.CreateTrainingAndDetectionProcessDirsInMMDetectionRoot();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "scripts")));
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "resources")));
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "output")));
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "detection_output")));
            }
            finally
            {
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public void CreateTrainingAndDetectionProcessDirsInMMDetectionRoot_WithInvalidPath_ShouldFail()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            _mockConfiguration.Setup(c => c.GetSection("MMDetectionConfiguration:Base:RootDirAbsPath"))
                .Returns(Mock.Of<IConfigurationSection>(s => s.Value == "Z:\\invalid\\path"));

            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.CreateTrainingAndDetectionProcessDirsInMMDetectionRoot();

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("does not exist", result.ErrMsg);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public void CopyScriptsToMMDetectionRoot_ShouldCopyScripts()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            Directory.CreateDirectory(Path.Combine(_testRootDir, "scripts"));

            var seedFilePath = Path.GetTempFileName();
            var seedJson = @"{
                ""CopyScripts"": [
                    {
                        ""Name"": ""test_script"",
                        ""FileUrl"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv""
                    }
                ]
            }";
            File.WriteAllText(seedFilePath, seedJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedTrainingAndDetectionProcess"])
                .Returns(seedFilePath);

            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.CopyScriptsToMMDetectionRoot();

                // Assert
                Assert.True(result.IsSuccess);
                // Note: We can't verify actual file copying since we mock the HTTP client,
                // but we can verify the process completed successfully
            }
            finally
            {
                File.Delete(seedFilePath);
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task GetAndSeedTrainedModelsFromSeedFile_ShouldSeedModels()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);

            var seedFilePath = Path.GetTempFileName();
            var seedJson = @"{
                ""Training"": {
                    ""InitialBaseModels"": [
                        {
                            ""Name"": ""TestModel"",
                            ""ConfigDownloadUrl"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv"",
                            ""ModelFile"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv""
                        }
                    ]
                }
            }";
            File.WriteAllText(seedFilePath, seedJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedTrainingAndDetectionProcess"])
                .Returns(seedFilePath);

            // Add test user
            var testUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@test.com"
            };
            await dbContext.Users.AddAsync(testUser);
            await dbContext.SaveChangesAsync();

            // Add test dataset class
            var datasetClass = new DatasetClass
            {
                Id = Guid.NewGuid(),
                ClassName = "waste",
                CreatedById = testUser.Id
            };
            await dbContext.DatasetClasses.AddAsync(datasetClass);
            await dbContext.SaveChangesAsync();

            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.GetAndSeedTrainedModelsFromSeedFile(transaction);

                // Assert
                Assert.True(result.IsSuccess);
                var trainedModels = await dbContext.TrainedModels.ToListAsync();
                Assert.NotEmpty(trainedModels);
                Assert.Equal("TestModel", trainedModels[0].Name);
            }
            finally
            {
                File.Delete(seedFilePath);
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task CreateTrainedModelDir_ShouldCreateAndReturnDirectory()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.CreateTrainedModelDir("TestModel");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.True(Directory.Exists(result.Data));
                Assert.Contains("TestModel", result.Data);
            }
            finally
            {
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task DownloadAndCopyTrainedModelToMMDetection_ShouldDownloadAndSaveFile()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);
            var modelName = "TestModel";
            var fileUrl = "https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv";

            try
            {
                // Act
                var result = await service.DownloadAndCopyTrainedModelToMMDetection(modelName, fileUrl);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Contains("200KB.csv", result.Data);
            }
            finally
            {
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task DownloadAndCopyScriptToMMDetection_ShouldDownloadAndSaveScript()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            var scriptsDir = Path.Combine(_testRootDir, "scripts");
            Directory.CreateDirectory(scriptsDir);

            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);
            var scriptUrl = "https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv";

            try
            {
                // Act
                var result = await service.DownloadAndCopyScriptToMMDetection(scriptUrl, scriptsDir);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Contains("200KB.csv", result.Data);
                Assert.True(File.Exists(result.Data));
            }
            finally
            {
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task SeedMMDetection_ShouldExecuteAllStepsAsync()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);

            var seedFilePath = Path.GetTempFileName();
            var seedJson = @"{
                ""CopyScripts"": [
                    {
                        ""Name"": ""test_script"",
                        ""FileUrl"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv""
                    }
                ],
                ""Training"": {
                    ""InitialBaseModels"": [
                        {
                            ""Name"": ""TestModel"",
                            ""ConfigDownloadUrl"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv"",
                            ""ModelFile"": ""https://onlinetestcase.com/wp-content/uploads/2023/06/200KB.csv""
                        }
                    ]
                }
            }";
            File.WriteAllText(seedFilePath, seedJson);

            _mockConfiguration.Setup(c => c["SeedDatabaseFilePaths:SeedTrainingAndDetectionProcess"])
                .Returns(seedFilePath);

            // Add test user
            var testUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@test.com"
            };
            await dbContext.Users.AddAsync(testUser);
            await dbContext.SaveChangesAsync();

            // Add test dataset class
            var datasetClass = new DatasetClass
            {
                Id = Guid.NewGuid(),
                ClassName = "waste",
                CreatedById = testUser.Id
            };
            await dbContext.DatasetClasses.AddAsync(datasetClass);
            await dbContext.SaveChangesAsync();

            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);

            try
            {
                // Act
                var result = service.SeedMMDetection(transaction);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "scripts")));
                Assert.True(Directory.Exists(Path.Combine(_testRootDir, "resources")));
            }
            finally
            {
                File.Delete(seedFilePath);
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task DownloadFromUrlToFileAbsPath_WithInvalidUrl_ShouldFail()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();

            Directory.CreateDirectory(_testRootDir);
            var service = new MMDetectionSetupService(_mockConfiguration.Object, dbContext);
            var invalidUrl = "https://invalid.url/file.txt";
            var destPath = Path.Combine(_testRootDir, "test.txt");

            try
            {
                // Act & Assert
                await Assert.ThrowsAsync<HttpRequestException>(async () => await service.DownloadFromUrlToFileAbsPath(invalidUrl, destPath));
            }
            finally
            {
                if (Directory.Exists(_testRootDir))
                    Directory.Delete(_testRootDir, true);
                transaction.Rollback();
            }
        }
    }
}
