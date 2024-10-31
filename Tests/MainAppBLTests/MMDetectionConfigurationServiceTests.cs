using System.Linq.Expressions;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Services.TrainingServices;
using Microsoft.Extensions.Configuration;
using Moq;
using SD;
using SD.Helpers;

namespace Tests.MainAppBLTests
{
    public class MMDetectionConfigurationServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly MMDetectionConfigurationService _service;

        public MMDetectionConfigurationServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            SetupMockConfiguration();
            _service = new MMDetectionConfigurationService(_mockConfiguration.Object);
        }

        private void SetupMockConfiguration()
        {
            var mockMMDetectionConfigSection = new Mock<IConfigurationSection>();
            _mockConfiguration.Setup(c => c.GetSection("MMDetectionConfiguration")).Returns(mockMMDetectionConfigSection.Object);

            SetupConfigurationSection(mockMMDetectionConfigSection, "Base:CondaExeAbsPath", "C:\\path\\to\\conda");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Base:RootDirAbsPath", "C:\\path\\to\\root");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Base:ScriptsDirRelPath", "scripts");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Base:OpenMMLabAbsPath", "C:\\path\\to\\openmmlab");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Training:DatasetsDirRelPath", "datasets");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Training:ConfigsDirRelPath", "configs");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Training:OutputDirRelPath", "output");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Training:BackboneCheckpointAbsPath", "C:\\path\\to\\backbone");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Training:CliLogsAbsPath", "C:\\path\\to\\training\\logs");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Detection:OutputDirRelPath", "detection_output");
            SetupConfigurationSection(mockMMDetectionConfigSection, "Detection:CliLogsAbsPath", "C:\\path\\to\\detection\\logs");
        }

        private void SetupConfigurationSection(Mock<IConfigurationSection> parentSection, string key, string value)
        {
            var mockChildSection = new Mock<IConfigurationSection>();
            mockChildSection.Setup(s => s.Value).Returns(value);
            parentSection.Setup(s => s[key]).Returns(value);
        }

        private string ExpectedPath(string windowsPath)
        {
            return Path.DirectorySeparatorChar == '/'
                ? "/" + windowsPath.Substring(3).Replace('\\', '/')
                : windowsPath.Replace('\\', '/');
        }

        [Fact]
        public void GetConfiguration_ReturnsCorrectConfiguration()
        {
            var config = _service.GetConfiguration();
            Assert.NotNull(config);
            Assert.Equal("C:\\path\\to\\conda", config.Base.CondaExeAbsPath);
            Assert.Equal("C:\\path\\to\\root", config.Base.RootDirAbsPath);
            Assert.Equal("scripts", config.Base.ScriptsDirRelPath);
        }

        [Fact]
        public void GetRootDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetRootDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root"), path);
        }

        [Fact]
        public void GetOpenMMLabAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetOpenMMLabAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\openmmlab"), path);
        }

        [Fact]
        public void GetCondaExeAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetCondaExeAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\conda"), path);
        }

        [Fact]
        public void GetScriptsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetScriptsDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root\\scripts"), path);
        }

        [Fact]
        public void GetBackboneCheckpointAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetBackboneCheckpointAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\backbone"), path);
        }

        [Fact]
        public void GetDatasetsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDatasetsDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root\\datasets"), path);
        }

        [Fact]
        public void GetTrainingRunDatasetDirAbsPath_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunDatasetDirAbsPath(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\datasets\\{guid}"), path);
        }

        [Fact]
        public void GetConfigsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetConfigsDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root\\configs"), path);
        }

        [Fact]
        public void GetTrainingRunConfigDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunConfigDirAbsPathByRunId(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\configs\\{guid}"), path);
        }

        [Fact]
        public void GetTrainingRunConfigFileAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunConfigFileAbsPathByRunId(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\configs\\{guid}\\{guid}.py"), path);
        }

        [Fact]
        public void GetTrainingRunsBaseOutDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetTrainingRunsBaseOutDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root\\output"), path);
        }

        [Fact]
        public void GetTrainingRunOutDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunOutDirAbsPathByRunId(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\output\\{guid}"), path);
        }

        [Trait("Category", "Integration")]
        [Fact]
        public void GetTrainingRunResultLogFileAbsPath_ReturnsSuccess_WhenDirectoryExists()
        {
            var guid = Guid.NewGuid();
            string rootPath = Path.GetTempPath();
            var mockService = new Mock<IMMDetectionConfigurationService>();
            mockService.Setup(x => x.GetTrainingRunOutDirAbsPathByRunId(guid))
                .Returns(rootPath);
            string runDirPath = mockService.Object.GetTrainingRunOutDirAbsPathByRunId(guid);
            string timeRunDirPath =
                CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(runDirPath, DateTime.UtcNow.ToString("ffffff")));
            string visDataPath =
                CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(timeRunDirPath, "vis_data"));
            string resultsFilePath =
                CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(visDataPath, "scalars.json"));

            Func<Guid, ResultDTO<string>> func = id => id == guid ? ResultDTO<string>.Ok(resultsFilePath) : ResultDTO<string>.Fail("Err");
            mockService.Setup(x => x.GetTrainingRunResultLogFileAbsPath(guid))
                .Returns(func);


            try
            {
                if (!Directory.Exists(runDirPath))
                    Directory.CreateDirectory(runDirPath);
                if (!Directory.Exists(timeRunDirPath))
                    Directory.CreateDirectory(timeRunDirPath);
                if (!Directory.Exists(visDataPath))
                    Directory.CreateDirectory(visDataPath);

                string scalarsJson = @"{
                    ""Training"": {
                        ""InitialBaseModels"": [
                            {
                                ""Name"": ""TestModel"",
                                ""ConfigDownloadUrl"": ""hdsdttp://test.com/config.py"",
                                ""ModelFile"": ""httsdsp://test.com/model.pth""
                            }
                        ]
                    }
                }";
                File.WriteAllText(resultsFilePath, scalarsJson);

                var result = mockService.Object.GetTrainingRunResultLogFileAbsPath(guid);
                Assert.True(result.IsSuccess);
            }
            finally
            {
                if (File.Exists(resultsFilePath))
                    File.Delete(resultsFilePath);
                if (Directory.Exists(visDataPath))
                    Directory.Delete(visDataPath);
                if (Directory.Exists(timeRunDirPath))
                    Directory.Delete(timeRunDirPath);
            }
        }

        [Fact]
        public void GetTrainingRunResultLogFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainingRunResultLogFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("", result.ErrMsg);
        }

        [Trait("Category", "Integration")]
        [Fact]
        public void GetTrainedModelConfigFileAbsPath_ReturnsSuccess_WhenDirectoryExists()
        {
            Guid guid = Guid.NewGuid();
            string rootPath = Path.Combine(Path.GetTempPath(), $"test_mmdetectionservice_{guid}");
            Mock<IMMDetectionConfigurationService> mockService = new Mock<IMMDetectionConfigurationService>();
            mockService.Setup(x => x.GetTrainingRunOutDirAbsPathByRunId(guid))
                .Returns(rootPath);
            string runDirPath = mockService.Object.GetTrainingRunOutDirAbsPathByRunId(guid);
            string configFilePath =
                CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(runDirPath, $"{guid}.py"));

            Func<Guid, ResultDTO<string>> func = id => id == guid ? ResultDTO<string>.Ok(configFilePath) : ResultDTO<string>.Fail("Err");
            mockService.Setup(x => x.GetTrainedModelConfigFileAbsPath(guid))
                .Returns(func);


            try
            {
                if (!Directory.Exists(runDirPath))
                    Directory.CreateDirectory(runDirPath);

                string configPy = @"#### Python Test File ####";
                File.WriteAllText(configFilePath, configPy);

                var result = mockService.Object.GetTrainedModelConfigFileAbsPath(guid);
                Assert.True(result.IsSuccess);
            }
            finally
            {
                if (File.Exists(configFilePath))
                    File.Delete(configFilePath);
            }
        }

        [Fact]
        public void GetTrainedModelConfigFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainedModelConfigFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist", result.ErrMsg);
        }

        [Trait("Category", "Integration")]
        [Fact]
        public void GetTrainedModelBestEpochFileAbsPath_NoEpochNumReturnsSuccess_WhenDirectoryExists()
        {
            Guid guid = Guid.NewGuid();
            string rootPath = Path.Combine(Path.GetTempPath(), $"test_mmdetectionservice_{guid}");
            Mock<IMMDetectionConfigurationService> mockService = new Mock<IMMDetectionConfigurationService>();

            mockService.Setup(x => x.GetTrainingRunOutDirAbsPathByRunId(guid))
                .Returns(rootPath);

            string runDirPath = mockService.Object.GetTrainingRunOutDirAbsPathByRunId(guid);
            string configFilePath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(runDirPath, $"epoch_1.pth"));

            Func<Guid, int?, ResultDTO<string>> func = (Guid idPar, int? epochPar = null) =>
                idPar == guid ? ResultDTO<string>.Ok(configFilePath) : ResultDTO<string>.Fail("Err");

            mockService.Setup(x => x.GetTrainedModelBestEpochFileAbsPath(guid, null))
                .Returns(() => func(guid, 1));

            try
            {
                if (!Directory.Exists(runDirPath))
                    Directory.CreateDirectory(runDirPath);

                string configPy = @"#### PTH Test File ####";
                File.WriteAllText(configFilePath, configPy);

                var result = mockService.Object.GetTrainedModelBestEpochFileAbsPath(guid, null);
                Assert.True(result.IsSuccess);
            }
            finally
            {
                if (File.Exists(configFilePath))
                    File.Delete(configFilePath);
            }
        }

        [Trait("Category", "Integration")]
        [Fact]
        public void GetTrainedModelBestEpochFileAbsPath_WithNumEpochReturnsSuccess_WhenDirectoryExists()
        {
            Guid guid = Guid.NewGuid();
            string rootPath = Path.Combine(Path.GetTempPath(), $"test_mmdetectionservice_{guid}");
            Mock<IMMDetectionConfigurationService> mockService = new Mock<IMMDetectionConfigurationService>();

            mockService.Setup(x => x.GetTrainingRunOutDirAbsPathByRunId(guid))
                .Returns(rootPath);

            string runDirPath = mockService.Object.GetTrainingRunOutDirAbsPathByRunId(guid);
            string configFilePath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(runDirPath, $"epoch_1.pth"));

            Func<Guid, int, ResultDTO<string>> func = (Guid idPar, int epochPar) =>
                idPar == guid ? ResultDTO<string>.Ok(configFilePath) : ResultDTO<string>.Fail("Err");

            mockService.Setup(x => x.GetTrainedModelBestEpochFileAbsPath(guid, 1))
                .Returns(() => func(guid, 1));

            try
            {
                if (!Directory.Exists(runDirPath))
                    Directory.CreateDirectory(runDirPath);

                string configPy = @"#### PTH Test File ####";
                File.WriteAllText(configFilePath, configPy);

                var result = mockService.Object.GetTrainedModelBestEpochFileAbsPath(guid, 1);
                Assert.True(result.IsSuccess);
            }
            finally
            {
                if (File.Exists(configFilePath))
                    File.Delete(configFilePath);
            }
        }

        [Fact]
        public void GetTrainedModelBestEpochFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainedModelBestEpochFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist", result.ErrMsg);
        }

        [Fact]
        public void GetTrainingRunCliOutDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetTrainingRunCliOutDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\training\\logs"), path);
        }

        [Fact]
        public void GetDetectionRunOutputDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDetectionRunOutputDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\root\\detection_output"), path);
        }

        [Fact]
        public void GetDetectionRunOutputDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetDetectionRunOutputDirAbsPathByRunId(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\detection_output\\{guid}"), path);
        }

        [Fact]
        public void GetDetectionRunOutputAnnotationsFileAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetDetectionRunOutputAnnotationsFileAbsPathByRunId(guid);
            Assert.Equal(ExpectedPath($"C:\\path\\to\\root\\detection_output\\{guid}\\detection_bboxes.json"), path);
        }

        [Fact]
        public void GetDetectionRunCliOutDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDetectionRunCliOutDirAbsPath();
            Assert.Equal(ExpectedPath("C:\\path\\to\\detection\\logs"), path);
        }
    }
}
