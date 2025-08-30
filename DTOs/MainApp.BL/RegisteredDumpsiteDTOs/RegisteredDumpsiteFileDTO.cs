namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;

public class RegisteredDumpsiteFileDTO
{
    public Guid Id { get; set; }
    public Guid RegisteredDumpsiteId { get; set; }
    public virtual RegisteredDumpsiteDTO RegisteredDumpsite { get; set; }

    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }
    public string FileExtension { get; set; }
    public string? Description { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual UserDTO? CreatedBy { get; set; }
}
