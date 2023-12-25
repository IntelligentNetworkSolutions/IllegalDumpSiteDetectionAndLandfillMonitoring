using SD;
using SD.Enums;

namespace MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings
{
    public class ApplicationSettingsEditViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string? InsertedModule { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }
        public ApplicationSettingsEditViewModel()
        {
            Modules = new List<Module>();

        }
    }
}
