using Entities.Intefaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MapConfigurationEntities
{
    public class MapLayerGroupConfiguration : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {        
        public string GroupName { get; set; }
        public string LayerGroupTitleJson { get; set; }
        public string LayerGroupDescriptionJson { get; set; }
        public int Order { get; set; }
        public double Opacity { get; set; }
        public string GroupJs { get; set; }
        public Guid? MapConfigurationId { get; set; }
        public virtual MapConfiguration? MapConfiguration { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public virtual ICollection<MapLayerConfiguration>? MapLayerConfigurations { get; set; }

    }
}
