using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Moq;
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
        /*
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
            Assert.Equal("C:/path/to/root", path);
        }

        [Fact]
        public void GetCondaExeAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetCondaExeAbsPath();
            Assert.Equal("C:/path/to/conda", path);
        }

        [Fact]
        public void GetScriptsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetScriptsDirAbsPath();
            Assert.Equal("C:/path/to/root/scripts", path);
        }

        [Fact]
        public void GetBackboneCheckpointAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetBackboneCheckpointAbsPath();
            Assert.Equal("C:/path/to/backbone", path);
        }

        [Fact]
        public void GetDatasetsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDatasetsDirAbsPath();
            Assert.Equal("C:/path/to/root/datasets", path);
        }

        [Fact]
        public void GetTrainingRunDatasetDirAbsPath_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunDatasetDirAbsPath(guid);
            Assert.Equal($"C:/path/to/root/datasets/{guid}", path);
        }

        [Fact]
        public void GetConfigsDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetConfigsDirAbsPath();
            Assert.Equal("C:/path/to/root/configs", path);
        }

        [Fact]
        public void GetTrainingRunConfigDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunConfigDirAbsPathByRunId(guid);
            Assert.Equal($"C:/path/to/root/configs/{guid}", path);
        }

        [Fact]
        public void GetTrainingRunConfigFileAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunConfigFileAbsPathByRunId(guid);
            Assert.Equal($"C:/path/to/root/configs/{guid}/{guid}.py", path);
        }

        [Fact]
        public void GetTrainingRunsBaseOutDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetTrainingRunsBaseOutDirAbsPath();
            Assert.Equal("C:/path/to/root/output", path);
        }

        [Fact]
        public void GetTrainingRunOutDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetTrainingRunOutDirAbsPathByRunId(guid);
            Assert.Equal($"C:/path/to/root/output/{guid}", path);
        }

        [Fact]
        public void GetTrainingRunResultLogFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainingRunResultLogFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("", result.ErrMsg);
        }

        [Fact]
        public void GetTrainedModelConfigFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainedModelConfigFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist", result.ErrMsg);
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
            Assert.Equal("C:/path/to/training/logs", path);
        }

        [Fact]
        public void GetDetectionRunOutputDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDetectionRunOutputDirAbsPath();
            Assert.Equal("C:/path/to/root/detection_output", path);
        }

        [Fact]
        public void GetDetectionRunOutputDirAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetDetectionRunOutputDirAbsPathByRunId(guid);
            Assert.Equal($"C:/path/to/root/detection_output/{guid}", path);
        }

        [Fact]
        public void GetDetectionRunOutputAnnotationsFileAbsPathByRunId_ReturnsCorrectPath()
        {
            var guid = Guid.NewGuid();
            var path = _service.GetDetectionRunOutputAnnotationsFileAbsPathByRunId(guid);
            Assert.Equal($"C:/path/to/root/detection_output/{guid}/detection_bboxes.json", path);
        }

        [Fact]
        public void GetDetectionRunCliOutDirAbsPath_ReturnsCorrectPath()
        {
            var path = _service.GetDetectionRunCliOutDirAbsPath();
            Assert.Equal("C:/path/to/detection/logs", path);
        }
        */
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

        // TODO: As Integration Test
        //[Fact]
        //public void GetTrainingRunResultLogFileAbsPath_ReturnsSuccess_WhenDirectoryExists()
        //{
        //    var guid = Guid.NewGuid();
        //    _mockConfiguration.Setup(x => x["Base:RootDirAbsPath"]).Returns(Path.GetTempPath());
        //    _mockConfiguration.Object["MMDetectionConfiguration:Base:RootDirAbsPath"] = Path.GetTempPath();
        //    string runDirPath = _service.GetTrainingRunOutDirAbsPathByRunId(guid);
        //    string timeRunDirPath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(runDirPath, DateTime.UtcNow.ToString("ffffff")));
        //    string visDataPath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(timeRunDirPath, "vis_data"));
        //    string resultsFilePath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(visDataPath, "scalars.json"));

        //    try
        //    {
        //        if (!Directory.Exists(runDirPath))
        //            Directory.Delete(runDirPath);
        //        if (!Directory.Exists(timeRunDirPath))
        //            Directory.Delete(timeRunDirPath);
        //        if (!Directory.Exists(visDataPath))
        //            Directory.Delete(visDataPath);
        //        if (!File.Exists(resultsFilePath))
        //            File.Delete(resultsFilePath);

        //        var result = _service.GetTrainingRunResultLogFileAbsPath(guid);
        //        Assert.True(result.IsSuccess);
        //    }
        //    finally
        //    {
        //        if(File.Exists(resultsFilePath))
        //            File.Delete(resultsFilePath);
        //        if (Directory.Exists(visDataPath))
        //            Directory.Delete(visDataPath);
        //        if (Directory.Exists(timeRunDirPath))
        //            Directory.Delete(timeRunDirPath);
        //        if (Directory.Exists(runDirPath))
        //            Directory.Delete(runDirPath);

        //        _mockConfiguration.Setup(x => x["Base:RootDirAbsPath"]).Returns("C:\\path\\to\\root");
        //    }
        //}

        [Fact]
        public void GetTrainingRunResultLogFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainingRunResultLogFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("", result.ErrMsg);
        }

        [Fact]
        public void GetTrainedModelConfigFileAbsPath_ReturnsFailure_WhenDirectoryDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var result = _service.GetTrainedModelConfigFileAbsPath(guid);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist", result.ErrMsg);
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
