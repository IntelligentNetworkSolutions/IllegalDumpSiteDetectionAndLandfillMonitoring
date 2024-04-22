using Entities.Intefaces;

namespace Entities.TrainingEntities
{
    public class TrainedModelStatistics : BaseEntity<Guid>
    {
        public Guid TrainedModelId { get; set; }
        public virtual TrainedModel? TrainedModel { get; set; }

        public TimeSpan? TrainingDuration { get; set; } = null;

        public int? TotalParameters { get; set; }

        public double? NumEpochs { get; set; }
        public double? LearningRate { get; set; }

        public double? AvgBoxLoss { get; set; }
        public double? mApp { get;set; }
    }
}
