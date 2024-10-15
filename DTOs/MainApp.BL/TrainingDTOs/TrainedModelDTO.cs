using DTOs.MainApp.BL.DatasetDTOs;

namespace DTOs.MainApp.BL.TrainingDTOs
{
    public class TrainedModelDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? ModelFilePath { get; set; }
        public string? ModelConfigPath { get; set; }

        public bool IsPublished { get; set; } = false;

        public Guid? DatasetId { get; set; }
        public virtual DatasetDTO? Dataset { get; set; }

        public Guid? TrainingRunId { get; set; }
        public virtual TrainingRunDTO? TrainingRun { get; set; }

        public Guid? BaseModelId { get; set; } = null;
        public virtual TrainedModelDTO? BaseModel { get; set; }

        public Guid? TrainedModelStatisticsId { get; set; }
        public virtual TrainedModelStatisticsDTO? TrainedModelStatistics { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }
    }
}
