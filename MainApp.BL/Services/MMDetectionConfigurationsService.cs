using DTOs.ObjectDetection.API;
using MainApp.BL.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SD;
using SD.Helpers;

public class MMDetectionConfigurationService : IMMDetectionConfigurationService
{
    private readonly MMDetectionConfiguration _MMDetectionConfiguration;
    private readonly IConfiguration _configuration;

    public MMDetectionConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;

        var section = _configuration.GetSection("MMDetectionConfiguration");

        _MMDetectionConfiguration =
            new MMDetectionConfiguration(
                new MMDetectionBaseConfiguration(
                    section["Base:CondaExeAbsPath"],
                    section["Base:RootDirAbsPath"],
                    section["Base:ScriptsDirRelPath"],
                    section["Base:OpenMMLabAbsPath"], 
                    section["Base:HasGPU"] 
                ),
                new MMDetectionTrainingConfiguration(
                    section["Training:DatasetsDirRelPath"],
                    section["Training:ConfigsDirRelPath"],
                    section["Training:OutputDirRelPath"],
                    section["Training:BackboneCheckpointAbsPath"],
                    section["Training:CliLogsAbsPath"]
                ),
                new MMDetectionDetectionConfiguration(
                    section["Detection:OutputDirRelPath"],
                    section["Detection:CliLogsAbsPath"]
                )
            );
    }

    public MMDetectionConfiguration GetConfiguration()
        => _MMDetectionConfiguration;

    public string GetRootDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Base.RootDirAbsPath);

    public string GetOpenMMLabAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Base.OpenMMLabAbsPath);

    public string GetHasGPU()
    =>  _MMDetectionConfiguration.Base.HasGPU;

    public string GetCondaExeAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Base.CondaExeAbsPath);

    public string GetScriptsDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(_MMDetectionConfiguration.Base.RootDirAbsPath, _MMDetectionConfiguration.Base.ScriptsDirRelPath));


    public string GetBackboneCheckpointAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Training.BackboneCheckpointAbsPath);

    public string GetDatasetsDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(_MMDetectionConfiguration.Base.RootDirAbsPath, _MMDetectionConfiguration.Training.DatasetsDirRelPath));

    public string GetTrainingRunDatasetDirAbsPath(Guid trainingRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(GetDatasetsDirAbsPath(), trainingRunId.ToString()));

    public string GetConfigsDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(_MMDetectionConfiguration.Base.RootDirAbsPath, _MMDetectionConfiguration.Training.ConfigsDirRelPath));

    public string GetTrainingRunConfigDirAbsPathByRunId(Guid trainingRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(GetConfigsDirAbsPath(), trainingRunId.ToString()));

    public string GetTrainingRunConfigFileAbsPathByRunId(Guid trainingRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(GetTrainingRunConfigDirAbsPathByRunId(trainingRunId), $"{trainingRunId}.py"));

    public string GetTrainingRunsBaseOutDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(_MMDetectionConfiguration.Base.RootDirAbsPath, _MMDetectionConfiguration.Training.OutputDirRelPath));

    public string GetTrainingRunOutDirAbsPathByRunId(Guid trainingRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(GetTrainingRunsBaseOutDirAbsPath(), trainingRunId.ToString()));

    public ResultDTO<string> GetTrainingRunResultLogFileAbsPath(Guid trainingRunId)
    {
        string trainRunOutDirAbsPath = GetTrainingRunOutDirAbsPathByRunId(trainingRunId);
        if (string.IsNullOrEmpty(trainRunOutDirAbsPath))
            return ResultDTO<string>.Fail("");

        if (Directory.Exists(trainRunOutDirAbsPath) == false)
            return ResultDTO<string>.Fail("");

        string[] dirs = Directory.GetDirectories(trainRunOutDirAbsPath);
        if (dirs.Length == 0)
            return ResultDTO<string>.Fail("No Directories found in Training Run Output Directory, there must be at least one direcotry present");

        string logDir = dirs[0];
        string? visDataDir = Directory.GetDirectories(logDir).FirstOrDefault(d => d.EndsWith("vis_data"));
        if (string.IsNullOrEmpty(visDataDir))
            return ResultDTO<string>.Fail("No Directory named vis_data found in Training Run Output Directory, there must be at least one directory named vis_data present");

        string[] visDataFiles = Directory.GetFiles(visDataDir);
        if (visDataFiles.Length == 0)
            return ResultDTO<string>.Fail("No Files found in Training Run Output vis_data Directory");

        string? resultLogFile = visDataFiles.FirstOrDefault(f => f.EndsWith("scalars.json"));
        if (string.IsNullOrEmpty(resultLogFile))
            return ResultDTO<string>.Fail($"No Result Log File found in Training Run Output vis_data Directory, no file named scalars.json");

        resultLogFile = CommonHelper.PathToLinuxRegexSlashReplace(resultLogFile);

        return ResultDTO<string>.Ok(resultLogFile);
    }

    public ResultDTO<string> GetTrainedModelConfigFileAbsPath(Guid trainingRunId)
    {
        string trainRunOutDirAbsPath = GetTrainingRunOutDirAbsPathByRunId(trainingRunId);
        if (string.IsNullOrEmpty(trainRunOutDirAbsPath))
            return ResultDTO<string>.Fail("Failed to Get Training Run Output Directory");

        if (Directory.Exists(trainRunOutDirAbsPath) == false)
            return ResultDTO<string>.Fail("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist");

        string[] files = Directory.GetFiles(trainRunOutDirAbsPath);
        if (files.Length == 0)
            return ResultDTO<string>.Fail("No Files found in Training Run Output Directory");

        string? configFile = files.FirstOrDefault(f => f.Contains($"{trainingRunId}.py"));
        if (string.IsNullOrEmpty(configFile))
            return ResultDTO<string>.Fail($"No Trained Model Config File found in Training Run Output Directory, no file named {trainingRunId}.py");
        if (File.Exists(configFile) == false)
            return ResultDTO<string>.Fail("Failed to Access Trained Model Config File found in Training Run Output Directory, No Permissions or File does Not Exist");

        configFile = CommonHelper.PathToLinuxRegexSlashReplace(configFile);

        return ResultDTO<string>.Ok(configFile);
    }

    public ResultDTO<string> GetTrainedModelBestEpochFileAbsPath(Guid trainingRunId, int? bestEpochNum = null)
    {
        string trainRunOutDirAbsPath = GetTrainingRunOutDirAbsPathByRunId(trainingRunId);
        if (string.IsNullOrEmpty(trainRunOutDirAbsPath))
            return ResultDTO<string>.Fail("Failed to Get Training Run Output Directory");

        if (Directory.Exists(trainRunOutDirAbsPath) == false)
            return ResultDTO<string>.Fail("Failed to Access Training Run Output Directory, No Permissions or Directory does Not Exist");

        string[] files = Directory.GetFiles(trainRunOutDirAbsPath);
        if (files.Length == 0)
            return ResultDTO<string>.Fail("No Files found in Training Run Output Directory");

        string[] epochFiles = files.Where(f => f.EndsWith(".pth")).ToArray();
        if (epochFiles.Length == 0)
            return ResultDTO<string>.Fail("No Epoch Files found in Training Run Output Directory, no files with .pth extension");

        string? bestEpoch = null;
        if (bestEpochNum.HasValue == false)
        {
            if (epochFiles.Length > 1)
                return ResultDTO<string>.Fail("Too Many Epoch Files found in Training Run Output Directory, too many files with .pth extension, only one Epoch(.pth) file should be present in Directory");

            bestEpoch = epochFiles[0];
        }
        else
        {
            bestEpoch = files.FirstOrDefault(f => f.EndsWith($"epoch_{bestEpochNum}.pth"));
            if (string.IsNullOrEmpty(bestEpoch))
                return ResultDTO<string>.Fail($"No Trained Model Config File found in Training Run Output Directory, no file named epoch_{bestEpochNum}.pth");
        }

        if (File.Exists(bestEpoch) == false)
            return ResultDTO<string>.Fail("Failed to Access Epoch File found in Training Run Output Directory, No Permissions or File does Not Exist");

        bestEpoch = CommonHelper.PathToLinuxRegexSlashReplace(bestEpoch);

        return ResultDTO<string>.Ok(bestEpoch);
    }

    public string GetTrainingRunCliOutDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Training.CliLogsAbsPath);


    public string GetDetectionRunOutputDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(_MMDetectionConfiguration.Base.RootDirAbsPath, _MMDetectionConfiguration.Detection.OutputDirRelPath));

    public string GetDetectionRunOutputDirAbsPathByRunId(Guid detectionRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(GetDetectionRunOutputDirAbsPath(), detectionRunId.ToString()));

    public string GetDetectionRunOutputAnnotationsFileAbsPathByRunId(Guid detectionRunId)
        => CommonHelper.PathToLinuxRegexSlashReplace(
            Path.Combine(GetDetectionRunOutputDirAbsPathByRunId(detectionRunId), "detection_bboxes.json"));

    public string GetDetectionRunCliOutDirAbsPath()
        => CommonHelper.PathToLinuxRegexSlashReplace(
            _MMDetectionConfiguration.Detection.CliLogsAbsPath);
}
