using SD.Enums;

namespace DTOs.MainApp.BL
{
    public class AppSettingDTO
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string? Description { get; set; }
        public string? Module { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }
    }
}
