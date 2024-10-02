using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration
{
    public class MapLayerConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string LayerName { get; set; }
        public string LayerTitleJson { get; set; }
        public string LayerDescriptionJson { get; set; }
        public int Order { get; set; }
        public string LayerJs { get; set; }
        public Guid? MapConfigurationId { get; set; }
        public virtual MapConfigurationViewModel? MapConfiguration { get; set; }
        public Guid? MapLayerGroupConfigurationId { get; set; }
        public virtual MapLayerGroupConfigurationViewModel? MapLayerGroupConfiguration { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserDTO? UpdatedBy { get; set; }
    }
}
