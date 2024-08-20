using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.LegalLandfillManagementDTOs
{
    public class WasteVolumeComparisonDTO
    {
        public string FileAName { get; set; }
        public string FileBName { get; set; }
        public double? Difference { get; set; }
        public string ScanDateFileA { get; set; }
        public string ScanDateFileB { get; set; }

    }
}
