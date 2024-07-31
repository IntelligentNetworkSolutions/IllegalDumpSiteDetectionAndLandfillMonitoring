using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record DatasetImageDTO
    {
        public Guid Id { get; init; }
        [NotMapped]
        public int IdInt { get; set; } // MMDetection Id

        public string FileName { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; init; }
        public string? ThumbnailPath { get; init; }

        public bool IsEnabled { get; set; } = false;

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