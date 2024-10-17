namespace Entities.TrainingEntities
{
    public class TrainingRunTrainParams : BaseEntity<Guid>
    {
        public Guid TrainingRunId { get; set; }
        public virtual TrainingRun TrainingRun { get; set; }

        public int? NumEpochs { get; set; }
        public int? BatchSize { get; set; }
        public int? NumFrozenStages { get; set; }
    }
}
