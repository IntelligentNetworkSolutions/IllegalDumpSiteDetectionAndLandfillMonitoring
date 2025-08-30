using SD.Enums;

namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
public class RegisteredDumpsiteInspectionDTO
{
    public Guid? Id { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Guid? RegisteredDumpsiteId { get; set; }
    public virtual RegisteredDumpsiteDTO? RegisteredDumpsite { get; set; }
    public RegisteredDumpsiteInspectionStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? Findings { get; set; }
    public string? Recommendations { get; set; }
    public string? CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual UserDTO? CreatedBy { get; set; }
    public virtual ICollection<RegisteredDumpsiteInspectionFileDTO> InspectionFiles { get; set; } = new List<RegisteredDumpsiteInspectionFileDTO>();
    public virtual ICollection<InspectionAssignmentDTO>? Assignments { get; set; } = new List<InspectionAssignmentDTO>();

    //[JsonIgnore]
    //public List<SelectListItem> AvailableInspectors { get; set; } = new List<SelectListItem>();
}

