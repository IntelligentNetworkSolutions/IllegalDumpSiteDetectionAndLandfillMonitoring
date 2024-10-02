using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration
{
    public class MapConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string Projection { get; set; }
        public string TileGridJs { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public string Resolutions { get; set; }
        public double DefaultResolution { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserDTO? UpdatedBy { get; set; }
        public virtual ICollection<MapLayerConfigurationViewModel>? MapLayerConfigurations { get; set; }
        public virtual ICollection<MapLayerGroupConfigurationViewModel>? MapLayerGroupConfigurations { get; set; }
    }
}
