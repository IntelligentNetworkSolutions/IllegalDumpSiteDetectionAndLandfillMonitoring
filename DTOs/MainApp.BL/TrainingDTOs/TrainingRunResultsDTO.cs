namespace DTOs.MainApp.BL.TrainingDTOs
{
    public class TrainingRunResultsDTO
    {
        public Dictionary<int, TrainingEpochMetricsDTO> EpochMetrics { get; set; }

        public TrainingEpochMetricsDTO BestEpochMetrics { get; set; }
    }
}
