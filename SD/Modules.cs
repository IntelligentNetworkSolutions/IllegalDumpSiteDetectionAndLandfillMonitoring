using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SD.Helpers;

namespace SD
{
    public class Module
    {
        /// <summary>
        /// Unique module code (used in DB config for tenants access to modules)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Can be used as a display name/title of the tool
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Module description for internal user (INS only)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// All module codes
    /// </summary>
    public static class Modules
    {
        public static ICollection<Module> GetAll()
        {
            Type t = typeof(Modules);
            FieldInfo[] fields = t.GetFields();
            var modules = new Collection<Module>();
            foreach (FieldInfo fi in fields)
            {
                modules.Add((Module)fi.GetValue(null));
            }
            return modules;
        }

        public static void CheckModuleValuesForDuplicates(ICollection<Module>? modules = null)
        {
            if (modules is null)
            {
                IEnumerable<string> valuesList = Modules.GetAll().Select(s => s.Value);
                if (CommonHelper.EnumerableHasDuplicatesByProperty(valuesList))
                    throw new Exception("SD.Modules have value duplicates. Rename the duplicated values!");

                return;
            }

            if (CommonHelper.EnumerableHasDuplicatesByProperty(modules, x => x.Value))
                throw new Exception($"{nameof(modules)} has value duplicates. Rename the duplicated values!");
        }

        public static readonly Module UserManagement = new Module { Value = "UserManagement", Description = "Management of users" };
        public static readonly Module AuditLog = new Module { Value = "AuditLog", Description = "Audit Log" };
        public static readonly Module Admin = new Module { Value = "Admin", Description = "Admin" };
        public static readonly Module SpecialActions = new Module { Value = "SpecialActions", Description = "Special Actions" };
        public static readonly Module Datasets = new Module { Value = "Datasets", Description = "Datasets" };
        public static readonly Module MapToolMeasureLength = new Module { Value = "MapToolMeasureLength", Title = "Measure Length", Description = "Measure length map tool" };
        public static readonly Module MapToolMeasureArea = new Module { Value = "MapToolMeasureArea", Title = "Measure Area", Description = "Measure area map tool" };
        public static readonly Module MapToolLastExtent = new Module { Value = "MapToolLastExtent", Title = "Last Extent", Description = "Last extent map tool" };
        public static readonly Module MapToolZoomToExtent = new Module { Value = "MapToolZoomToExtent", Title = "Zoom To Area", Description = "ZoomToExtent map tool" };
        public static readonly Module GoToCoordinatesTool = new Module { Value = "GoToCoordinatesTool", Title = "Go To Coordinates", Description = "Go To Coordinates Tool" };
        public static readonly Module HistoricData = new Module { Value = "HistoricData", Title = "Historic Data", Description = "Historic Data" };
        public static readonly Module Detection = new Module { Value = "Detection", Title = "Detection", Description = "Detection" };
        public static readonly Module LegalLandfillManagement = new Module { Value = "LegalLandfillManagement", Title = "Legal Landfill Management", Description = "Legal Landfill Management" };
        public static readonly Module MapToolDetectionIgnoreZones = new Module { Value = "MapToolDetectionIgnoreZones", Title = "Map Tool Detection Ignore Zones", Description = "Map Tool Detection Ignore Zones" };
        public static readonly Module LegalLandfillWasteManagement = new Module { Value = "LegalLandfillWasteManagement", Title = "Legal Landfill Waste Management", Description = "Legal Landfill Waste Management" };
        public static readonly Module MapConfiguration = new Module { Value = "MapConfiguration", Title = "Map Configuration", Description = "Map Configuration" };
        public static readonly Module MapToolDetectionInputImages = new Module { Value = "MapToolDetectionInputImages", Title = "Map Tool Detection Input Images", Description = "Map Tool Detection Input Images" };
        public static readonly Module ScheduleDetectionAndTrainingRuns = new Module { Value = "ScheduleDetectionAndTrainingRuns", Title = "Schedule Detection And Training Runs", Description = "Schedule Detection And Training Runs" };
        public static readonly Module Training = new Module { Value = "Training", Title = "Training", Description = "Training" };
        


    }
}
