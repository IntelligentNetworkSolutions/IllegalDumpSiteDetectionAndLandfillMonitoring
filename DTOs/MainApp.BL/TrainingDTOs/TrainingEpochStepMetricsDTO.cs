namespace DTOs.MainApp.BL.TrainingDTOs
{
    public record TrainingEpochStepMetricsDTO
    {
        public int Step { get; set; }
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>();
    }
}
