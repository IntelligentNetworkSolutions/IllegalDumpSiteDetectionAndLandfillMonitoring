namespace DTOs.MainApp.BL.TrainingDTOs
{
    public class TrainingRunTrainParamsDTO
    {
        public Guid? Id { get; set; }

        public Guid? TrainingRunId { get; set; }
        public virtual TrainingRunDTO? TrainingRun { get; set; }

        public int? NumEpochs { get; set; }
        public int? BatchSize { get; set; }
        public int? NumFrozenStages { get; set; }
    }
}
