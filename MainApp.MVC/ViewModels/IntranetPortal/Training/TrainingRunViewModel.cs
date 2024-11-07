namespace MainApp.MVC.ViewModels.IntranetPortal.Training
{
    public class TrainingRunViewModel
    {
        public string Name { get; set; }
        public Guid TrainedModelId { get; set; }
        public Guid DatasetId { get; set; }
        public int NumEpochs { get; set; }
        public int BatchSize { get; set; }
        public int NumFrozenStages { get; set; }
    }
}
