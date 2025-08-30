using Entities.Intefaces;

namespace Entities.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteInspection : BaseEntity<Guid>, ICreatedByUser
{
    public Guid RegisteredDumpsiteId { get; set; }
    public virtual RegisteredDumpsite RegisteredDumpsite { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int RegisteredDumpsiteInspectionStatusId { get; set; }
    public virtual RegisteredDumpsiteInspectionStatus? RegisteredDumpsiteInspectionStatus { get; set; }

    public string? Notes { get; set; }
    public string? Findings { get; set; }
    public string? Recommendations { get; set; }

    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser? CreatedBy { get; set; }

    public virtual ICollection<InspectionAssignment> Assignments { get; set; } = new List<InspectionAssignment>();
    public virtual ICollection<RegisteredDumpsiteInspectionFile> InspectionFiles { get; set; } = new List<RegisteredDumpsiteInspectionFile>();
}
