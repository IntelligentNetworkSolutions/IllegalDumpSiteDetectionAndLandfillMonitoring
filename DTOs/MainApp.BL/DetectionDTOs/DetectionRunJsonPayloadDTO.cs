using DTOs.MainApp.BL.TrainingDTOs;

namespace DTOs.MainApp.BL.DetectionDTOs
{
    public record DetectionRunJsonPayloadDTO
    {
        public Guid Id { get; } = Guid.NewGuid();

        public required string DetectionImagePath { get; init; }
        public required string DetectionImageFileName { get; init; }

        public required string TrainedModelName { get; init; }
        public required string TrainedModelPath { get; init; }

        public Guid DetectionRunId { get; init; }
        public virtual DetectionRunDTO? DetectionRun { get; init; }

        public Guid TrainedModelId { get; init; }
        public virtual TrainedModelDTO? TrainedModel { get; init; }
    }
}
