using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        //TODO: Maybe add function return claims for role name
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


    } 
}


     
