using SD.Enums;

namespace MainApp.MVC.ViewModels.IntranetPortal.ApplicationSettings
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
