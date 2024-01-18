using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Intefaces;

namespace Entities
{
    public abstract class BaseEntity<TId> : IBaseEntity<TId> where TId : IEquatable<TId>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId Id { get; set; }
    }
}
