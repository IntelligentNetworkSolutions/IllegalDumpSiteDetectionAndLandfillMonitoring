using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainApp.BL.Services.TrainingServices;
using SD.Helpers;

namespace Tests.MainAppBLTests.Services.TrainingServices
{
    public class TrainingConfigGeneratorTests
    {
        [Fact]
        public void GenerateConfigOverrideStr_ShouldGenerateCompleteConfig()
        {
            // Arrange
            string backboneCheckpointPath = @"C:\Models\backbone.pth";
            string dataRootPath = @"C:\Data\dataset";
            string[] classNames = ["waste", "recycling"];
            string baseModelConfigPath = @"C:\Config\base_model.py";
            string baseModelFilePath = @"C:\Models\base_model.pth";
            int numDatasetClasses = 2;
            int batchSize = 4;
            int epochs = 20;
            int frozenStages = 2;

            // Act
            string result = TrainingConfigGenerator.GenerateConfigOverrideStr(
                backboneCheckpointPath,
                dataRootPath,
                classNames,
                baseModelConfigPath,
                baseModelFilePath,
                numDatasetClasses,
                batchSize,
                epochs,
                frozenStages
            );

            // Assert
            Assert.Contains($"num_dataset_classes = {numDatasetClasses}", result);
            Assert.Contains($"num_batch_size = {batchSize}", result);
            Assert.Contains($"num_epochs = {epochs}", result);
            Assert.Contains($"num_frozen_stages = {frozenStages}", result);
            Assert.Contains($"_base_ = ['{CommonHelper.PathToLinuxRegexSlashReplace(baseModelConfigPath)}']", result);
            Assert.Contains($"data_root = '{CommonHelper.PathToLinuxRegexSlashReplace(dataRootPath)}'", result);
            Assert.Contains("'waste', 'recycling'", result);
            Assert.Contains($"load_from = '{CommonHelper.PathToLinuxRegexSlashReplace(baseModelFilePath)}'", result);
        }

        [Fact]
        public void GenerateConfigOverrideStr_WithNullOptionalParameters_ShouldGenerateCompleteConfig()
        {
            // Arrange
            string backboneCheckpointPath = @"C:\Models\backbone.pth";
            string dataRootPath = @"C:\Data\dataset";
            string[] classNames = ["waste"];
            string baseModelConfigPath = @"C:\Config\base_model.py";
            string baseModelFilePath = @"C:\Models\base_model.pth";
            int numDatasetClasses = 1;

            // Act
            string result = TrainingConfigGenerator.GenerateConfigOverrideStr(
                backboneCheckpointPath,
                dataRootPath,
                classNames,
                baseModelConfigPath,
                baseModelFilePath,
                numDatasetClasses,
                null,
                null,
                null
            );

            // Assert
            Assert.Contains("num_dataset_classes = 1", result);
            Assert.Contains("num_batch_size = 2", result); // Default value
            Assert.Contains("num_epochs = 15", result); // Default value
            Assert.Contains("num_frozen_stages = 2", result); // Default value
        }

        [Fact]
        public void GenerateConfigOverrideStr_WithEmptyClassNames_ShouldGenerateConfig()
        {
            // Arrange
            string backboneCheckpointPath = @"C:\Models\backbone.pth";
            string dataRootPath = @"C:\Data\dataset";
            string[] classNames = [];
            string baseModelConfigPath = @"C:\Config\base_model.py";
            string baseModelFilePath = @"C:\Models\base_model.pth";
            int numDatasetClasses = 0;

            // Act
            string result = TrainingConfigGenerator.GenerateConfigOverrideStr(
                backboneCheckpointPath,
                dataRootPath,
                classNames,
                baseModelConfigPath,
                baseModelFilePath,
                numDatasetClasses
            );

            // Assert
            Assert.Contains("metainfo = dict(classes=(), palette=[])", result);
        }

        [Fact]
        public void GenerateConfigVariablesOverrideStr_WithDefaultValues_ShouldGenerateCorrectly()
        {
            // Arrange
            int numDatasetClasses = 1;

            // Act
            string result = TrainingConfigGenerator.GenerateConfigVariablesOverrideStr(numDatasetClasses);

            // Assert
            Assert.Contains("num_dataset_classes = 1", result);
            Assert.Contains("num_batch_size = 2", result); // Default value
            Assert.Contains("num_epochs = 15", result); // Default value
            Assert.Contains("num_frozen_stages = 2", result); // Default value
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void GenerateConfigVariablesOverrideStr_WithEdgeCases_ShouldGenerateCorrectly(int numDatasetClasses)
        {
            // Act
            string result = TrainingConfigGenerator.GenerateConfigVariablesOverrideStr(numDatasetClasses);

            // Assert
            Assert.Contains($"num_dataset_classes = {numDatasetClasses}", result);
        }

        [Fact]
        public void GenerateConfigVariablesOverrideStr_WithCustomValues_ShouldGenerateCorrectly()
        {
            // Arrange
            int numDatasetClasses = 3;
            int batchSize = 8;
            int epochs = 25;
            int frozenStages = 4;

            // Act
            string result = TrainingConfigGenerator.GenerateConfigVariablesOverrideStr(
                numDatasetClasses, batchSize, epochs, frozenStages);

            // Assert
            Assert.Contains("num_dataset_classes = 3", result);
            Assert.Contains("num_batch_size = 8", result);
            Assert.Contains("num_epochs = 25", result);
            Assert.Contains("num_frozen_stages = 4", result);
        }

        [Theory]
        [InlineData(@"C:\Config\base_model.py")]
        [InlineData(@"\\network\share\base_model.py")]
        [InlineData(@"./local/path/base_model.py")]
        [InlineData(@"base_model.py")]
        public void GenerateConfigBaseModelOverrideStr_ShouldHandleVariousPathFormats(string baseModelConfigPath)
        {
            // Arrange
            string expectedLinuxPath = CommonHelper.PathToLinuxRegexSlashReplace(baseModelConfigPath);

            // Act
            string result = TrainingConfigGenerator.GenerateConfigBaseModelOverrideStr(baseModelConfigPath);

            // Assert
            Assert.Equal($"_base_ = ['{expectedLinuxPath}']", result);
        }

        [Theory]
        [InlineData(@"C:\Data\dataset")]
        [InlineData(@"\\network\share\dataset")]
        [InlineData(@"./local/path/dataset")]
        [InlineData(@"dataset")]
        public void GenerateConfigDataRootOverrideStr_ShouldHandleVariousPathFormats(string dataRootPath)
        {
            // Arrange
            string expectedLinuxPath = CommonHelper.PathToLinuxRegexSlashReplace(dataRootPath);

            // Act
            string result = TrainingConfigGenerator.GenerateConfigDataRootOverrideStr(dataRootPath);

            // Assert
            Assert.Equal($"data_root = '{expectedLinuxPath}'\r\n", result);
        }

        [Fact]
        public void GenerateConfigMetaInfoOverrideStr_SingleClass_ShouldGenerateCorrectly()
        {
            // Arrange
            string[] classNames = ["waste"];

            // Act
            string result = TrainingConfigGenerator.GenerateConfigMetaInfoOverrideStr(classNames);

            // Assert
            Assert.Contains("'waste'", result);
            Assert.Contains("(220, 20, 60)", result);
        }

        [Fact]
        public void GenerateConfigMetaInfoOverrideStr_MultipleClasses_ShouldGenerateCorrectly()
        {
            // Arrange
            string[] classNames = ["waste", "recycling", "hazardous"];

            // Act
            string result = TrainingConfigGenerator.GenerateConfigMetaInfoOverrideStr(classNames);

            // Assert
            Assert.Contains("'waste', 'recycling', 'hazardous'", result);
            Assert.Contains("(220, 20, 60)", result); // First class
            Assert.Contains("(210, 30, 70)", result); // Second class
            Assert.Contains("(200, 40, 80)", result); // Third class
        }

        [Fact]
        public void GenerateConfigMetaInfoOverrideStr_WithEmptyArray_ShouldGenerateEmptyConfig()
        {
            // Arrange
            string[] classNames = [];

            // Act
            string result = TrainingConfigGenerator.GenerateConfigMetaInfoOverrideStr(classNames);

            // Assert
            Assert.Contains("metainfo = dict(classes=(), palette=[])", result);
        }

        [Theory]
        [InlineData(@"C:\Models\backbone.pth")]
        [InlineData(@"\\network\share\backbone.pth")]
        [InlineData(@"./local/path/backbone.pth")]
        [InlineData(@"backbone.pth")]
        public void GenerateConfigModelOverrideStr_ShouldHandleVariousPathFormats(string backboneCheckpointPath)
        {
            // Arrange
            string expectedLinuxPath = CommonHelper.PathToLinuxRegexSlashReplace(backboneCheckpointPath);

            // Act
            string result = TrainingConfigGenerator.GenerateConfigModelOverrideStr(backboneCheckpointPath);

            // Assert
            Assert.Contains("model = dict(", result);
            Assert.Contains("backbone=dict(", result);
            Assert.Contains("frozen_stages=num_frozen_stages", result);
            Assert.Contains($"init_cfg=dict(checkpoint='{expectedLinuxPath}', type='Pretrained')", result);
            Assert.Contains("num_stages=4", result);
            Assert.Contains("roi_head=dict(", result);
            Assert.Contains("bbox_head=dict(", result);
            Assert.Contains("num_classes=num_dataset_classes", result);
            Assert.Contains("type='FasterRCNN'", result);
        }

        [Theory]
        [InlineData(@"C:\Models\base_model.pth")]
        [InlineData(@"\\network\share\base_model.pth")]
        [InlineData(@"./local/path/base_model.pth")]
        [InlineData(@"base_model.pth")]
        public void GenerateConfigLoadFromOverrideStr_ShouldHandleVariousPathFormats(string baseModelFilePath)
        {
            // Arrange
            string expectedLinuxPath = CommonHelper.PathToLinuxRegexSlashReplace(baseModelFilePath);

            // Act
            string result = TrainingConfigGenerator.GenerateConfigLoadFromOverrideStr(baseModelFilePath);

            // Assert
            Assert.Equal($"load_from = '{expectedLinuxPath}'\r\n", result);
        }

        [Fact]
        public void ConstantConfigStrings_ShouldContainRequiredElements()
        {
            // Train config constants
            Assert.Contains("max_epochs=num_epochs", TrainingConfigGenerator.GenerateConfigTrainCfgOverrideStr);
            Assert.Contains("train_dataloader = dict(", TrainingConfigGenerator.GenerateConfigTrainDataloaderOverrideStr);
            Assert.Contains("batch_size=num_batch_size", TrainingConfigGenerator.GenerateConfigTrainDataloaderOverrideStr);
            Assert.Contains("train/annotations_coco.json", TrainingConfigGenerator.GenerateConfigTrainDataloaderOverrideStr);

            // Val config constants
            Assert.Contains("val_cfg = dict(", TrainingConfigGenerator.GenerateConfigValCfgOverrideStr);
            Assert.Contains("val_dataloader = dict(", TrainingConfigGenerator.GenerateConfigValDataloaderOverrideStr);
            Assert.Contains("val_evaluator = dict(", TrainingConfigGenerator.GenerateConfigValEvaluatorOverrideStr);
            Assert.Contains("valid/annotations_coco.json", TrainingConfigGenerator.GenerateConfigValDataloaderOverrideStr);

            // Test config constants
            Assert.Contains("test_cfg = dict(", TrainingConfigGenerator.GenerateConfigTestCfgOverrideStr);
            Assert.Contains("test_dataloader = dict(", TrainingConfigGenerator.GenerateConfigTestDataloaderOverrideStr);
            Assert.Contains("test_evaluator = dict(", TrainingConfigGenerator.GenerateConfigTestEvaluatorOverrideStr);
            Assert.Contains("test/annotations_coco.json", TrainingConfigGenerator.GenerateConfigTestDataloaderOverrideStr);
        }

        [Fact]
        public void AllConfigPaths_ShouldProperlyFormatPaths()
        {
            // Arrange
            string windowsPath = @"C:\Path\With\Windows\Style";
            string networkPath = @"\\network\share\path";
            string unixPath = "/unix/style/path";
            string relativePath = @"relative\path";

            // Act & Assert
            foreach (string path in new[] { windowsPath, networkPath, unixPath, relativePath })
            {
                string formattedPath = CommonHelper.PathToLinuxRegexSlashReplace(path);

                // Test in each config generation method
                Assert.Contains(formattedPath,
                    TrainingConfigGenerator.GenerateConfigBaseModelOverrideStr(path));
                Assert.Contains(formattedPath,
                    TrainingConfigGenerator.GenerateConfigDataRootOverrideStr(path));
                Assert.Contains(formattedPath,
                    TrainingConfigGenerator.GenerateConfigModelOverrideStr(path));
                Assert.Contains(formattedPath,
                    TrainingConfigGenerator.GenerateConfigLoadFromOverrideStr(path));
            }
        }
    }
}
