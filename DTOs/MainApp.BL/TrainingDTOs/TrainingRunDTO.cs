using DTOs.MainApp.BL.DatasetDTOs;

namespace DTOs.MainApp.BL.TrainingDTOs
{
    public class TrainingRunDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }

        public bool IsCompleted { get; set; } = false;
        public string? Status { get; set; }

        public Guid? DatasetId { get; set; }
        public virtual DatasetDTO? Dataset { get; set; }

        public Guid? TrainedModelId { get; set; } = null;
        public virtual TrainedModelDTO? TrainedModel { get; set; }

        public Guid? BaseModelId { get; set; } = null;
        public virtual TrainedModelDTO? BaseModel { get; set; }

        public Guid? TrainParamsId { get; set; }
        public TrainingRunTrainParamsDTO? TrainParams { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }
    }
}
