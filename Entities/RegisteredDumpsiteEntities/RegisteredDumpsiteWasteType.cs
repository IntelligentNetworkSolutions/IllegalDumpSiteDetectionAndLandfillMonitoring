using Entities.Intefaces;

namespace Entities.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteWasteType : BaseEntity<Guid>, ICreatedByUser
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public ApplicationUser? CreatedBy { get; set; }
}
