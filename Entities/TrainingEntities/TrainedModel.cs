using Entities.DatasetEntities;
using Entities.Intefaces;

namespace Entities.TrainingEntities
{
    public class TrainedModel : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }
        public string ModelFilePath { get; set; }

        public bool IsPublished { get; set; } = false;

        public Guid DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public Guid? TrainingRunId { get; set; }
        public virtual TrainingRun? TrainingRun { get; set; }

        public Guid? BaseModelId { get; set; } = null;
        public virtual TrainedModel? BaseModel { get; set; }

        public Guid? TrainedModelStatisticsId { get; set; }
        public virtual TrainedModelStatistics? TrainedModelStatistics { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}
