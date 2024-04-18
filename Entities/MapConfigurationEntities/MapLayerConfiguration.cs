using Entities.Intefaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MapConfigurationEntities
{
    public class MapLayerConfiguration : BaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {       
        public string LayerName { get; set; }
        public string LayerTitleJson { get; set; }
        public string LayerDescriptionJson { get; set; }
        public int Order { get; set; }
        public string LayerJs { get; set; }
        public Guid? MapConfigurationId { get; set; }
        public virtual MapConfiguration? MapConfiguration { get; set; }
        public Guid? MapLayerGroupConfigurationId { get; set; }
        public virtual MapLayerGroupConfiguration? MapLayerGroupConfiguration { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
