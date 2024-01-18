namespace Entities.Intefaces
{
    public interface IBaseEntity<TId> where TId : IEquatable<TId>
    {
        TId Id { get; set; }
    }
}
