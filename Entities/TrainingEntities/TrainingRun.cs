using Entities.DatasetEntities;
using Entities.Intefaces;

namespace Entities.TrainingEntities
{
    public class TrainingRun : BaseEntity<Guid>, ICreatedByUser
    {
        public string Name { get; set; }

        public bool IsCompleted { get; set; } = false;
        public string? Status { get; set; }

        public Guid DatasetId { get; set; }
        public virtual Dataset? Dataset { get; set; }

        public Guid? TrainedModelId { get; set; } = null;
        public virtual TrainedModel? TrainedModel { get; set; }

        public Guid? BaseModelId { get; set; } = null;
        public virtual TrainedModel? BaseModel { get; set; }

        public Guid? TrainParamsId { get; set; }
        public TrainingRunTrainParams? TrainParams { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}
