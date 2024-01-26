namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record DatasetImageDTO
    {
        public Guid Id { get; init; }
        public string FileName { get; init; }
        public string ImagePath { get; init; }

        public bool IsEnabled { get; init; } = false;

        public Guid? DatasetId { get; init; }
        public virtual DatasetDTO? Dataset { get; init; }

        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
    }
}
