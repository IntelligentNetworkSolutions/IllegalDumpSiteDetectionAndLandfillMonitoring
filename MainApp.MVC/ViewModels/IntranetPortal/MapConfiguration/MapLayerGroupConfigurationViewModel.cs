using DTOs.MainApp.BL;
using DTOs.MainApp.BL.MapConfigurationDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration
{
    public class MapLayerGroupConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string LayerGroupTitleJson { get; set; }
        public string LayerGroupDescriptionJson { get; set; }
        public int Order { get; set; }
        public double Opacity { get; set; }
        public string GroupJs { get; set; }
        public Guid? MapConfigurationId { get; set; }
        public virtual MapConfigurationDTO? MapConfiguration { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserDTO? UpdatedBy { get; set; }
        public virtual ICollection<MapLayerConfigurationViewModel>? MapLayerConfigurations { get; set; }
    }
}
