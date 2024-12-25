using System.Collections.ObjectModel;
using System.Reflection;

namespace SD
{
    public class AuthClaim
    {
        /// <summary>
        /// Values must be unique across all claims
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This will be shown to the end admin user when configuring access right to users/roles
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Do not provide value for common functionalities that are not part of a module     
        /// </summary>
        public Module FromModule { get; set; }

    }

    /// <summary>
    /// All Authorization claims are defined here
    /// </summary>
    public static class AuthClaims
    {
        public static ICollection<AuthClaim> GetAll()
        {
            Type t = typeof(AuthClaims);
            FieldInfo[] fields = t.GetFields();
            Collection<AuthClaim> claims = new Collection<AuthClaim>();
            foreach (FieldInfo fi in fields)
            {
                claims.Add((AuthClaim)fi.GetValue(null));
            }
            return claims;
        }

        public static void CheckAuthClaimsValuesForDuplicates()
        {
            var valuesList = GetAll().Select(s => s.Value);
            if (valuesList.Count() != valuesList.Distinct().Count())
            {
                throw new Exception("SD.AuthClaims have value duplicates. Rename the duplicated values!");
            }
        }

        public static void CheckAuthClaimsForInvalidCharacters()
        {
            var valuesList = GetAll().Select(s => s.Value);
            if (valuesList.Where(s => s.Contains(',')).Count() > 0)
            {
                throw new Exception("There are SD.AuthClaims with invalid characters in the Value param!");
            }
        }

        public static readonly AuthClaim UserManagement = new AuthClaim
        {
            Value = "1:1",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Manage all intranet portal users"
        };
        public static readonly AuthClaim UserManagementEditUsersAndRoles = new AuthClaim
        {
            Value = "1:2",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Edit users and roles"
        };
        public static readonly AuthClaim UserManagementAddUsersAndRoles = new AuthClaim
        {
            Value = "1:3",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Add users and roles"
        };
        public static readonly AuthClaim UserManagementDeleteUsersAndRoles = new AuthClaim
        {
            Value = "1:4",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Delete users and roles"
        };
        public static readonly AuthClaim UserManagementViewUserClaims = new AuthClaim
        {
            Value = "1:5",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View user claims"
        };
        public static readonly AuthClaim UserManagementViewUserRoles = new AuthClaim
        {
            Value = "1:6",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View user roles"
        };
        public static readonly AuthClaim UserManagementViewRoleClaims = new AuthClaim
        {
            Value = "1:7",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View role claims"
        };
        public static readonly AuthClaim AuditLog = new AuthClaim
        {
            Value = "2:1",
            FromModule = Modules.AuditLog,
            Description = "Audit Log: View Audit Log"
        };
        public static readonly AuthClaim Admin = new AuthClaim
        {
            Value = "3:1",
            FromModule = Modules.Admin,
            Description = "Admin: Admin access"
        };
        public static readonly AuthClaim SpecialActions = new AuthClaim
        {
            Value = "4:1",
            FromModule = Modules.SpecialActions,
            Description = "SpecialActions: Special Actions"
        };
        public static readonly AuthClaim SpecialActionsResetTranslationCache = new AuthClaim
        {
            Value = "4:2",
            FromModule = Modules.SpecialActions,
            Description = "SpecialActions: Reset translation cache"
        };
        public static readonly AuthClaim AddDataset = new AuthClaim
        {
            Value = "5:1",
            FromModule = Modules.Datasets,
            Description = "Datasets: Add dataset"
        };
        public static readonly AuthClaim ManageDataset = new AuthClaim
        {
            Value = "5:2",
            FromModule = Modules.Datasets,
            Description = "Datasets: Manage dataset"
        };
        public static readonly AuthClaim DeleteDataset = new AuthClaim
        {
            Value = "5:3",
            FromModule = Modules.Datasets,
            Description = "Datasets: Delete dataset"
        };
        public static readonly AuthClaim DeleteDatasetClass = new AuthClaim
        {
            Value = "5:4",
            FromModule = Modules.Datasets,
            Description = "Datasets: Delete dataset class"
        };
        public static readonly AuthClaim EditDatasetClass = new AuthClaim
        {
            Value = "5:5",
            FromModule = Modules.Datasets,
            Description = "Datasets: Edit dataset class"
        };
        public static readonly AuthClaim AddDatasetClass = new AuthClaim
        {
            Value = "5:6",
            FromModule = Modules.Datasets,
            Description = "Datasets: Add dataset class"
        };
        public static readonly AuthClaim ViewDatasetClasses = new AuthClaim
        {
            Value = "5:7",
            FromModule = Modules.Datasets,
            Description = "Datasets: View dataset classes"
        };
        public static readonly AuthClaim ViewDatasets = new AuthClaim
        {
            Value = "5:8",
            FromModule = Modules.Datasets,
            Description = "Datasets: View datasets"
        };
        public static readonly AuthClaim ChooseDatasetClassType = new AuthClaim
        {
            Value = "5:9",
            FromModule = Modules.Datasets,
            Description = "Datasets: Choose dataset class type"
        };
        public static readonly AuthClaim PublishDataset = new AuthClaim
        {
            Value = "5:10",
            FromModule = Modules.Datasets,
            Description = "Datasets: Publish dataset"
        };
        public static readonly AuthClaim AddDatasetImage = new AuthClaim
        {
            Value = "5:11",
            FromModule = Modules.Datasets,
            Description = "Datasets: Add dataset image"
        };
        public static readonly AuthClaim DeleteDatasetImage = new AuthClaim
        {
            Value = "5:12",
            FromModule = Modules.Datasets,
            Description = "Datasets: Delete dataset image"
        };
        public static readonly AuthClaim EditDatasetImage = new AuthClaim
        {
            Value = "5:13",
            FromModule = Modules.Datasets,
            Description = "Datasets: Edit dataset image"
        };
        public static readonly AuthClaim EditDatasetImageAnnotations = new AuthClaim
        {
            Value = "5:14",
            FromModule = Modules.Datasets,
            Description = "Datasets: Edit dataset image annotations"
        };
        public static readonly AuthClaim ViewDatasetImageAnnotations = new AuthClaim
        {
            Value = "5:15",
            FromModule = Modules.Datasets,
            Description = "Datasets: View dataset image annotations"
        };
        public static readonly AuthClaim MapToolMeasureLength = new AuthClaim
        {
            Value = "6:1",
            FromModule = Modules.MapToolMeasureLength,
            Description = "Map Tools: Measure length map tool"
        };
        public static readonly AuthClaim MapToolMeasureArea = new AuthClaim
        {
            Value = "7:1",
            FromModule = Modules.MapToolMeasureArea,
            Description = "Map Tools: Measure area map tool"
        };
        public static readonly AuthClaim MapToolLastExtent = new AuthClaim
        {
            Value = "8:1",
            FromModule = Modules.MapToolLastExtent,
            Description = "Map Tools: Last extent map tool"
        };
        public static readonly AuthClaim MapToolZoomToExtent = new AuthClaim
        {
            Value = "9:1",
            FromModule = Modules.MapToolZoomToExtent,
            Description = "Map Tools: Zoom to extent"
        };
        public static readonly AuthClaim GoToCoordinatesTool = new AuthClaim
        {
            Value = "10:1",
            FromModule = Modules.GoToCoordinatesTool,
            Description = "Go To Coordinates Tool"
        };
        public static readonly AuthClaim ViewHistoricData = new AuthClaim
        {
            Value = "11:1",
            FromModule = Modules.HistoricData,
            Description = "View Historic Data"
        };
        public static readonly AuthClaim ViewDetectionRuns = new AuthClaim
        {
            Value = "12:1",
            FromModule = Modules.Detection,
            Description = "View Detection Runs"
        };
        public static readonly AuthClaim DeleteDetectionRun = new AuthClaim
        {
            Value = "12:2",
            FromModule = Modules.Detection,
            Description = "Delete Detection Run"
        };
        public static readonly AuthClaim CreateDetectionRun = new AuthClaim
        {
            Value = "12:3",
            FromModule = Modules.Detection,
            Description = "Create Detection Run"
        };
        public static readonly AuthClaim ManageDetectionRuns = new AuthClaim
        {
            Value = "12:4",
            FromModule = Modules.Detection,
            Description = "Manage Detection Runs"
        };
        public static readonly AuthClaim StartDetectionRun = new AuthClaim
        {
            Value = "12:5",
            FromModule = Modules.Detection,
            Description = "Start Detection Run"
        };
        public static readonly AuthClaim ScheduleDetectionRun = new AuthClaim
        {
            Value = "12:6",
            FromModule = Modules.Detection,
            Description = "Schedule Detection Run"
        };
        public static readonly AuthClaim ViewDetectedZones = new AuthClaim
        {
            Value = "12:7",
            FromModule = Modules.Detection,
            Description = "View Detected Zones"
        };
        public static readonly AuthClaim PreviewDetectionInputImages = new AuthClaim
        {
            Value = "12:8",
            FromModule = Modules.Detection,
            Description = "Preview Detection Input Images"
        };
        public static readonly AuthClaim AddDetectionInputImage = new AuthClaim
        {
            Value = "12:9",
            FromModule = Modules.Detection,
            Description = "Add Detection Input Image"
        };
        public static readonly AuthClaim EditDetectionInputImage = new AuthClaim
        {
            Value = "12:10",
            FromModule = Modules.Detection,
            Description = "Edit Detection Input Image"
        };
        public static readonly AuthClaim DeleteDetectionInputImage = new AuthClaim
        {
            Value = "12:11",
            FromModule = Modules.Detection,
            Description = "Delete Detection Input Image"
        };

        public static readonly AuthClaim AddLegalLandfill = new AuthClaim
        {
            Value = "13:1",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Add Legal Landfill"
        };
        public static readonly AuthClaim EditLegalLandfill = new AuthClaim
        {
            Value = "13:2",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Edit Legal Landfill"
        };
        public static readonly AuthClaim DeleteLegalLandfill = new AuthClaim
        {
            Value = "13:3",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Delete Legal Landfill"
        };
        public static readonly AuthClaim ViewLegalLandfillsManagement = new AuthClaim
        {
            Value = "13:4",
            FromModule = Modules.LegalLandfillManagement,
            Description = "View Legal Landfills Management"
        };
        public static readonly AuthClaim ViewLegalLandfills = new AuthClaim
        {
            Value = "13:5",
            FromModule = Modules.LegalLandfillManagement,
            Description = "View Legal Landfills"
        };
        public static readonly AuthClaim ViewLegalLandfillPointCloudFiles = new AuthClaim
        {
            Value = "13:6",
            FromModule = Modules.LegalLandfillManagement,
            Description = "View Legal Landfill Point Cloud Files"
        };
        public static readonly AuthClaim UploadConvertLegalLandfillPointCloudFile = new AuthClaim
        {
            Value = "13:7",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Upload And Convert Legal Landfill Point Cloud File"
        };
        public static readonly AuthClaim EditLegalLandfillPointCloudFile = new AuthClaim
        {
            Value = "13:8",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Edit Legal Landfill Point Cloud File"
        };
        public static readonly AuthClaim DeleteLegalLandfillPointCloudFile = new AuthClaim
        {
            Value = "13:9",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Delete Legal Landfill Point Cloud File"
        };
        public static readonly AuthClaim PreviewLegalLandfillPointCloudFiles = new AuthClaim
        {
            Value = "13:10",
            FromModule = Modules.LegalLandfillManagement,
            Description = "Preview Legal Landfill Point Cloud Files"
        };
        public static readonly AuthClaim ViewWasteVolumeDiffAnalysis = new AuthClaim
        {
            Value = "13:11",
            FromModule = Modules.LegalLandfillManagement,
            Description = "View Waste Volume Differential Analysis"
        };
        public static readonly AuthClaim AddLegalLandfillTruck = new AuthClaim
        {
            Value = "14:1",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Add Legal Landfill Truck"
        };
        public static readonly AuthClaim EditLegalLandfillTruck = new AuthClaim
        {
            Value = "14:2",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Edit Legal Landfill Truck"
        };
        public static readonly AuthClaim DeleteLegalLandfillTruck = new AuthClaim
        {
            Value = "14:3",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Delete Legal Landfill Truck"
        };
        public static readonly AuthClaim ViewLegalLandfillTrucks = new AuthClaim
        {
            Value = "14:4",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "View Legal Landfill Trucks"
        };
        public static readonly AuthClaim AddLegalLandfillWasteType = new AuthClaim
        {
            Value = "14:5",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Add Legal Landfill Waste Type"
        };
        public static readonly AuthClaim EditLegalLandfillWasteType = new AuthClaim
        {
            Value = "14:6",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Edit Legal Landfill Waste Type"
        };
        public static readonly AuthClaim DeleteLegalLandfillWasteType = new AuthClaim
        {
            Value = "14:7",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Delete Legal Landfill Waste Type"
        };
        public static readonly AuthClaim ViewLegalLandfillWasteTypes = new AuthClaim
        {
            Value = "14:8",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "View Legal Landfill Waste Types"
        };
        public static readonly AuthClaim ViewLegalLandfillWasteImports = new AuthClaim
        {
            Value = "14:9",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "View Legal Landfill Waste Imports"
        };
        public static readonly AuthClaim AddLegalLandfillWasteImports = new AuthClaim
        {
            Value = "14:10",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Add Legal Landfill Waste Imports"
        };
        public static readonly AuthClaim DeleteLegalLandfillWasteImports = new AuthClaim
        {
            Value = "14:11",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Delete Legal Landfill Waste Imports"
        };
        public static readonly AuthClaim EditLegalLandfillWasteImports = new AuthClaim
        {
            Value = "14:12",
            FromModule = Modules.LegalLandfillWasteManagement,
            Description = "Edit Legal Landfill Waste Imports"
        };
        public static readonly AuthClaim ViewMapConfigurations = new AuthClaim
        {
            Value = "15:1",
            FromModule = Modules.MapConfiguration,
            Description = "View Map Configurations"
        };
        public static readonly AuthClaim EditMapConfigurations = new AuthClaim
        {
            Value = "15:2",
            FromModule = Modules.MapConfiguration,
            Description = "Edit Map Configurations"
        };
        public static readonly AuthClaim DeleteMapConfigurations = new AuthClaim
        {
            Value = "15:3",
            FromModule = Modules.MapConfiguration,
            Description = "Delete Map Configurations"
        };
        public static readonly AuthClaim AddMapConfigurations = new AuthClaim
        {
            Value = "15:4",
            FromModule = Modules.MapConfiguration,
            Description = "Add Map Configurations"
        };
        public static readonly AuthClaim ViewMapLayerConfigurations = new AuthClaim
        {
            Value = "15:5",
            FromModule = Modules.MapConfiguration,
            Description = "View Map Layer Configurations"
        };
        public static readonly AuthClaim EditMapLayerConfigurations = new AuthClaim
        {
            Value = "15:6",
            FromModule = Modules.MapConfiguration,
            Description = "Edit Map Layer Configurations"
        };
        public static readonly AuthClaim DeleteMapLayerConfigurations = new AuthClaim
        {
            Value = "15:7",
            FromModule = Modules.MapConfiguration,
            Description = "Delete Map Layer Configurations"
        };
        public static readonly AuthClaim AddMapLayerConfigurations = new AuthClaim
        {
            Value = "15:8",
            FromModule = Modules.MapConfiguration,
            Description = "Add Map Layer Configurations"
        };
        public static readonly AuthClaim ViewMapLayerGroupConfigurations = new AuthClaim
        {
            Value = "15:9",
            FromModule = Modules.MapConfiguration,
            Description = "View Map Layer Group Configurations"
        };
        public static readonly AuthClaim EditMapLayerGroupConfigurations = new AuthClaim
        {
            Value = "15:10",
            FromModule = Modules.MapConfiguration,
            Description = "Edit Map Layer Group Configurations"
        };
        public static readonly AuthClaim DeleteMapLayerGroupConfigurations = new AuthClaim
        {
            Value = "15:11",
            FromModule = Modules.MapConfiguration,
            Description = "Delete Map Layer Group Configurations"
        };
        public static readonly AuthClaim AddMapLayerGroupConfigurations = new AuthClaim
        {
            Value = "15:12",
            FromModule = Modules.MapConfiguration,
            Description = "Add Map Layer Group Configurations"
        };

        public static readonly AuthClaim ViewDetectionInputImages = new AuthClaim
        {
            Value = "16:1",
            FromModule = Modules.MapToolDetectionInputImages,
            Description = "View Detection Input Images"
        };

        public static readonly AuthClaim ManageDetectionIgnoreZones = new AuthClaim
        {
            Value = "17:1",
            FromModule = Modules.MapToolDetectionIgnoreZones,
            Description = "Manage Detection Ignore Zones"
        };
        public static readonly AuthClaim ManageScheduledRuns = new AuthClaim
        {
            Value = "18:1",
            FromModule = Modules.ScheduleDetectionAndTrainingRuns,
            Description = "Manage Scheduled Detection And Training Runs"
        };

        public static readonly AuthClaim CreateTrainingRun = new AuthClaim
        {
            Value = "19:1",
            FromModule = Modules.Training,
            Description = "Create Training Run"
        };
        public static readonly AuthClaim ViewTrainingRuns = new AuthClaim
        {
            Value = "19:2",
            FromModule = Modules.Training,
            Description = "View Training Run"
        };
        public static readonly AuthClaim ScheduleTrainingRun = new AuthClaim
        {
            Value = "19:3",
            FromModule = Modules.Training,
            Description = "Schedule Training Run"
        };
        public static readonly AuthClaim DeleteTrainingRun = new AuthClaim
        {
            Value = "19:4",
            FromModule = Modules.Training,
            Description = "Delete Training Run"
        };
        public static readonly AuthClaim EditTrainingRun = new AuthClaim
        {
            Value = "19:5",
            FromModule = Modules.Training,
            Description = "Edit Training Run"
        };
        public static readonly AuthClaim PublishTrainingRunTrainedModel = new AuthClaim
        {
            Value = "19:6",
            FromModule = Modules.Training,
            Description = "Publish Training Run Trained Model"
        };
        public static readonly AuthClaim ViewTrainingRunsStatistics = new AuthClaim
        {
            Value = "19:7",
            FromModule = Modules.Training,
            Description = "View Training Runs Statistics"
        };
        public static readonly AuthClaim ViewTrainedModelStatistics = new AuthClaim
        {
            Value = "19:8",
            FromModule = Modules.Training,
            Description = "View Trained Model Statistics"
        };
        public static readonly AuthClaim ViewApplicationSettings = new AuthClaim
        {
            Value = "20:1",
            FromModule = Modules.ApplicationSettings,
            Description = "View Application Settings"
        };
        public static readonly AuthClaim CreateApplicationSettings = new AuthClaim
        {
            Value = "20:2",
            FromModule = Modules.ApplicationSettings,
            Description = "Create Application Settings"
        };
        public static readonly AuthClaim UpdateApplicationSettings = new AuthClaim
        {
            Value = "20:3",
            FromModule = Modules.ApplicationSettings,
            Description = "Update Application Settings"
        };
        public static readonly AuthClaim DeleteApplicationSettings = new AuthClaim
        {
            Value = "20:4",
            FromModule = Modules.ApplicationSettings,
            Description = "Delete Application Settings"
        };



    }
}



