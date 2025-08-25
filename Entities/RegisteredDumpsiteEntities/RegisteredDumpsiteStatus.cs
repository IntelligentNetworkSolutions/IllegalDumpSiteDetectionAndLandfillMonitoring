namespace Entities.RegisteredDumpsiteEntities
{
    public class RegisteredDumpsiteStatus : BaseEntity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }

    }
}
