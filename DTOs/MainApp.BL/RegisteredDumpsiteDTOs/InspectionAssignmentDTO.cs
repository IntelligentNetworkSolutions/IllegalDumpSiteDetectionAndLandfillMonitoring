namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
public class InspectionAssignmentDTO
{
    public Guid RegisteredDumpsiteInspectionId { get; set; }
    public Guid InspectorId { get; set; }
    public UserDTO? Inspector { get; set; }
    public DateTime AssignedOn { get; set; }
}
