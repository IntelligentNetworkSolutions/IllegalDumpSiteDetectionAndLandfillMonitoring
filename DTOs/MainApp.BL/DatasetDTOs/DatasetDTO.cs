using System.ComponentModel.DataAnnotations;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record DatasetDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }

        public bool IsPublished { get; init; } = false;

        public Guid? ParentDatasetId { get; init; }
        public virtual DatasetDTO? ParentDataset { get; init; }

        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }

        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
        public bool? AnnotationsPerSubclass { get; set; }
    }
}
