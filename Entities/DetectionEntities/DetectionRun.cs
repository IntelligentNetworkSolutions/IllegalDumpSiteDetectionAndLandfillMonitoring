﻿using Entities.Intefaces;
using Entities.TrainingEntities;

namespace Entities.DetectionEntities
{
    public class DetectionRun : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;
        public string? Status { get; set; }

        public Guid DetectionInputImageId { get; set; }
        public virtual DetectionInputImage? DetectionInputImage { get; set; }

        public Guid TrainedModelId { get; set; }
        public virtual TrainedModel? TrainedModel { get; set;}

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }

        public virtual ICollection<DetectedDumpSite>? DetectedDumpSites { get; set; }
    }
}
