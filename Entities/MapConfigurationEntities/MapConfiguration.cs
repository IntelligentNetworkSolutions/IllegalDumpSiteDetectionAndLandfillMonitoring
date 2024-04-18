using Entities.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MapConfigurationEntities
{
    public class MapConfiguration : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
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
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public virtual ICollection<MapLayerConfiguration>? MapLayerConfigurations { get; set; }
        public virtual ICollection<MapLayerGroupConfiguration>? MapLayerGroupConfigurations { get; set; }
    }
}
