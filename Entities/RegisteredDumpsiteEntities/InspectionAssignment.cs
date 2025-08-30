namespace Entities.RegisteredDumpsiteEntities;
public class InspectionAssignment
{
    public Guid RegisteredDumpsiteInspectionId { get; set; }
    public RegisteredDumpsiteInspection RegisteredDumpsiteInspection { get; set; }

    public string InspectorId { get; set; }
    public ApplicationUser Inspector { get; set; }

    public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
}
