using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.MapConfigurationDTOs
{
    public record MapLayerGroupConfigurationDTO
    {
        public Guid Id { get; init; }
        public string GroupName { get; init; }
        public string LayerGroupTitleJson { get; init; }
        public string LayerGroupDescriptionJson { get; init; }
        public int Order { get; init; }
        public double Opacity { get; init; }
        public string GroupJs { get; init; }
        public Guid? MapConfigurationId { get; init; }
        public virtual MapConfigurationDTO? MapConfiguration { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
        public virtual ICollection<MapLayerConfigurationDTO>? MapLayerConfigurations { get; init; }
    }
}
