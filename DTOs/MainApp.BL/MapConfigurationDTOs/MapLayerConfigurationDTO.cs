using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.MapConfigurationDTOs
{
    public record MapLayerConfigurationDTO
    {
        public Guid Id { get; init; }
        public string LayerName { get; init; }
        public string LayerTitleJson { get; init; }
        public string LayerDescriptionJson { get; init; }
        public int Order { get; init; }
        public string LayerJs { get; init; }
        public Guid? MapConfigurationId { get; init; }
        public virtual MapConfigurationDTO? MapConfiguration { get; init; }
        public Guid? MapLayerGroupConfigurationId { get; init; }
        public virtual MapLayerGroupConfigurationDTO? MapLayerGroupConfiguration { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
    }
}
