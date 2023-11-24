using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ApplicationSettingsViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string? Module { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }
    }
}
