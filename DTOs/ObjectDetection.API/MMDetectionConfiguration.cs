namespace DTOs.ObjectDetection.API
{
    public class MMDetectionConfiguration
    {
        public MMDetectionBaseConfiguration Base { get; init; }
        public MMDetectionTrainingConfiguration Training { get; init; }
        public MMDetectionDetectionConfiguration Detection { get; init; }

        public MMDetectionConfiguration(){}

        public MMDetectionConfiguration(MMDetectionBaseConfiguration baseConfig, MMDetectionTrainingConfiguration training, MMDetectionDetectionConfiguration detection)
        {
            Base = baseConfig;
            Training = training;
            Detection = detection;
        }
    }

    public record MMDetectionBaseConfiguration(string CondaExeAbsPath, string RootDirAbsPath, string ScriptsDirRelPath, string OpenMMLabAbsPath);

    public record MMDetectionTrainingConfiguration(string DatasetsDirRelPath, string ConfigsDirRelPath, 
        string OutputDirRelPath, string BackboneCheckpointAbsPath, string CliLogsAbsPath);

    public record MMDetectionDetectionConfiguration(string OutputDirRelPath, string CliLogsAbsPath);
}
