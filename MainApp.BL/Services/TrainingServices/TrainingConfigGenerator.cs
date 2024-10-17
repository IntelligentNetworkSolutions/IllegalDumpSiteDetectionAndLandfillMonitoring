using SD.Helpers;

namespace MainApp.BL.Services.TrainingServices
{
    public static class TrainingConfigGenerator
    {
        public static string GenerateConfigOverrideStr(string backboneCheckpointAbsPath,
            string dataRootAbsPath, string[] classNames,
            string baseModelConfigFilePath, string baseModelFileAbsPath,
            int numDatasetClasses, int? numBatchSize = null, int? numEpochs = null, int? numFrozenStages = null)
        {
            string configStr =
                $"{GenerateConfigVariablesOverrideStr(numDatasetClasses, numBatchSize, numEpochs, numFrozenStages)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigBaseModelOverrideStr(baseModelConfigFilePath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigDataRootOverrideStr(dataRootAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigMetaInfoOverrideStr(classNames)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigModelOverrideStr(backboneCheckpointAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigTrainCfgOverrideStr}\r\n" +
                $"{GenerateConfigTrainDataloaderOverrideStr(dataRootAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigValCfgOverrideStr}\r\n" +
                $"{GenerateConfigValDataloaderOverrideStr(dataRootAbsPath)}\r\n" +
                $"{GenerateConfigValEvaluatorOverrideStr(dataRootAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigTestCfgOverrideStr}\r\n" +
                $"{GenerateConfigTestDataloaderOverrideStr(dataRootAbsPath)}\r\n" +
                $"{GenerateConfigTestEvaluatorOverrideStr(dataRootAbsPath)}\r\n" +
                $"\r\n" +
                $"{GenerateConfigLoadFromOverrideStr(baseModelFileAbsPath)}\r\n" +
                $"\r\n";

            return configStr;
        }

        public static string GenerateConfigVariablesOverrideStr(int numDatasetClasses,
            int? numBatchSize = null, int? numEpochs = null, int? numFrozenStages = null)
        {
            string configVariablesOverrideStr =
                $"num_dataset_classes = {numDatasetClasses}\r\n" +
                $"num_batch_size = {(numBatchSize.HasValue ? numBatchSize.Value : 2)}\r\n" +
                $"num_epochs = {(numEpochs.HasValue ? numEpochs.Value : 15)}\r\n" +
                $"num_frozen_stages = {(numFrozenStages.HasValue ? numFrozenStages.Value : 2)}\r\n";

            return configVariablesOverrideStr;
        }

        public static string GenerateConfigBaseModelOverrideStr(string baseModelConfigFilePath)
            => $"_base_ = ['{CommonHelper.ConvertWindowsPathToLinuxPathReplaceAllDashes(baseModelConfigFilePath)}']";

        public static string GenerateConfigDataRootOverrideStr(string dataRootAbsPath)
            => $"data_root = '{CommonHelper.ConvertWindowsPathToLinuxPathReplaceAllDashes(dataRootAbsPath)}'\r\n";

        public static string GenerateConfigMetaInfoOverrideStr(string[] classNames)
        {
            string classesListOverrideStr = "";
            string paletteArrOverrideStr = "";
            const int redVal = 220;
            const int greenVal = 20;
            const int blueVal = 60;
            for (int i = 0; i < classNames.Length; i++)
            {
                string className = classNames[i];
                classesListOverrideStr += $"'{className}', ";
                paletteArrOverrideStr += $"({redVal + i * -10}, {greenVal + i * 10}, {blueVal + i * 10}), ";
            }

            string metaInfoOverrideStr = $"metainfo = dict(classes=({classesListOverrideStr}), palette=[{paletteArrOverrideStr}])\r\n";

            return metaInfoOverrideStr;
        }

        public static string GenerateConfigModelOverrideStr(string backboneCheckpointAbsPath)
        {
            backboneCheckpointAbsPath = CommonHelper.ConvertWindowsPathToLinuxPathReplaceAllDashes(backboneCheckpointAbsPath);

            string configModelOverrideStr =
                $"model = dict(\r\n" +
                $"\tbackbone=dict(\r\n" +
                $"\t\tfrozen_stages=num_frozen_stages,\r\n" +
                $"\t\tinit_cfg=dict(checkpoint='{backboneCheckpointAbsPath}', type='Pretrained'),\r\n" +
                $"\t\tnum_stages=4),\r\n" +
                $"\troi_head=dict(\r\n" +
                $"\t\tbbox_head=dict(\r\n" +
                $"\t\t\tnum_classes=num_dataset_classes\r\n" +
                $"\t\t\t),\r\n" +
                $"\t\t),\r\n" +
                $"\ttype='FasterRCNN')\r\n";

            return configModelOverrideStr;
        }

        public const string GenerateConfigTrainCfgOverrideStr =
            "train_cfg = dict(max_epochs=num_epochs, type='EpochBasedTrainLoop', val_interval=1)\t\n";
        public static string GenerateConfigTrainDataloaderOverrideStr(string dataRootAbsPath) 
            => "train_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                $"\t\tann_file='train/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='train/'),),\r\n" +
                "\tnum_workers=2,)\r\n";

        public const string GenerateConfigValCfgOverrideStr =
            "val_cfg = dict(type='ValLoop')\r\n";
        public static string GenerateConfigValDataloaderOverrideStr(string dataRootAbsPath) 
            => "val_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                $"\t\tann_file='valid/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='valid/'),),\r\n" +
                "\tnum_workers=2,)\r\n";
        public static string GenerateConfigValEvaluatorOverrideStr(string dataRootAbsPath) 
            => $"val_evaluator = dict(ann_file=data_root + '/valid/annotations_coco.json',)\r\n";

        public const string GenerateConfigTestCfgOverrideStr =
            "test_cfg = dict(type='TestLoop')\r\n";
        public static string GenerateConfigTestDataloaderOverrideStr(string dataRootAbsPath)
            => "test_dataloader = dict(\r\n" +
                "\tbatch_size=num_batch_size,\r\n" +
                "\tdataset=dict(\r\n" +
                "\t\tdata_root=data_root,\r\n" +
                "\t\tmetainfo=metainfo,\r\n" +
                $"\t\tann_file='test/annotations_coco.json',\r\n" +
                "\t\tdata_prefix=dict(img='test/'),),\r\n" +
                "\tnum_workers=2,)\r\n";
        public static string GenerateConfigTestEvaluatorOverrideStr(string dataRootAbsPath)
            => $"test_evaluator = dict(ann_file=data_root + '/test/annotations_coco.json',)\r\n";

        public static string GenerateConfigLoadFromOverrideStr(string baseModelFileAbsPath)
            => $"load_from = '{CommonHelper.ConvertWindowsPathToLinuxPathReplaceAllDashes(baseModelFileAbsPath)}'\r\n";
    }
}
