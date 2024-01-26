namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record DatasetClassDTO
    {
        public Guid Id { get; init; }
        public int ClassId { get; init; }
        public string ClassName { get; init; }

        public Guid DatasetId { get; init; }
        public virtual DatasetDTO? Dataset { get; init; }

        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
    }
}
