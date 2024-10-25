using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ApplicationStorage.SeedDatabase.ModulesConfigs.MMDetectionSetup
{
    internal class MMDetectionConfiguration
    {
        internal MMDetectionBaseConfiguration Base { get; init; }
        internal MMDetectionTrainingConfiguration Training { get; init; }
        internal MMDetectionDetectionConfiguration Detection { get; init; }

        internal MMDetectionConfiguration() { }

        internal MMDetectionConfiguration(MMDetectionBaseConfiguration baseConfig, MMDetectionTrainingConfiguration training, MMDetectionDetectionConfiguration detection)
        {
            Base = baseConfig;
            Training = training;
            Detection = detection;
        }
    }

    internal record MMDetectionBaseConfiguration(string CondaExeAbsPath, string RootDirAbsPath, string ScriptsDirRelPath, string ResourcesDirRelPath, string OpenMMLabAbsPath);

    internal record MMDetectionTrainingConfiguration(string DatasetsDirRelPath, string ConfigsDirRelPath,
        string OutputDirRelPath, string BackboneCheckpointAbsPath, string CliLogsAbsPath);

    internal record MMDetectionDetectionConfiguration(string OutputDirRelPath, string CliLogsAbsPath);
}
