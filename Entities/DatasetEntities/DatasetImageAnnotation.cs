using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Intefaces;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Entities.DatasetEntities
{
    public class DatasetImageAnnotation : IBaseEntity<Guid>, ICreatedByUser, IUpdatedByUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string AnnotationsGeoJson { get; set; }
        [JsonIgnore]
        public required LineString? Geom { get; set; }

        public bool IsEnabled { get; set; } = false;

        public Guid? DatasetImageId { get; set; }
        public virtual DatasetImage? DatasetImage { get; set; }

        public required string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
