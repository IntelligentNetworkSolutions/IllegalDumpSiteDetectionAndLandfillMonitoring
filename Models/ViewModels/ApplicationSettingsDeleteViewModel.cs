using SD;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ApplicationSettingsDeleteViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string? InsertedModule { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }

    }
}
