using Entities.Intefaces;

namespace Entities.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteInspectionFile : BaseEntity<Guid>, ICreatedByUser
{
    public Guid RegisteredDumpsiteInspectionId { get; set; }
    public virtual RegisteredDumpsiteInspection RegisteredDumpsiteInspection { get; set; }

    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }
    public string? Description { get; set; }
    public string FileExtension { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser? CreatedBy { get; set; }
}
