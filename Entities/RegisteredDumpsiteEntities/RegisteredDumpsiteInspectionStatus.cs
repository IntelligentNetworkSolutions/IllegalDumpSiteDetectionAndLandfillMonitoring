namespace Entities.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteInspectionStatus : BaseEntity<int>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}
