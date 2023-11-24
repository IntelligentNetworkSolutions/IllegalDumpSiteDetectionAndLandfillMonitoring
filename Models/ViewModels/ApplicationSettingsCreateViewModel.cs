using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD;

namespace Models.ViewModels
{
    public class ApplicationSettingsCreateViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string? InsertedModule { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }
        public List<string> AllApplicationSettingsKeys { get; set; }
        public ApplicationSettingsCreateViewModel()
        {
            Modules = new List<Module>();
            AllApplicationSettingsKeys= new List<string>();

        }
        
    }
}
