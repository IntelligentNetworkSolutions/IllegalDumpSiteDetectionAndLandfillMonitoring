namespace DTOs.MainApp.BL.TrainingDTOs
{
    public record TrainingEpochMetricsDTO
    {
        public int Epoch { get; set; }
        public List<TrainingEpochStepMetricsDTO> Steps { get; set; } = new List<TrainingEpochStepMetricsDTO>();
    }
}
