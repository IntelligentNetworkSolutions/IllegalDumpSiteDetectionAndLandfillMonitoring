using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.MapConfigurationDTOs
{
    public record MapConfigurationDTO
    {
        public Guid Id { get; init; }
        public string MapName { get; init; }
        public string Projection { get; init; }
        public string TileGridJs { get; init; }
        public double CenterX { get; init; }
        public double CenterY { get; init; }
        public double MinX { get; init; }
        public double MinY { get; init; }
        public double MaxX { get; init; }
        public double MaxY { get; init; }
        public string Resolutions { get; init; }
        public double DefaultResolution { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
        public virtual ICollection<MapLayerConfigurationDTO>? MapLayerConfigurations { get; init; }
        public virtual ICollection<MapLayerGroupConfigurationDTO>? MapLayerGroupConfigurations { get; init; }
    }
}
