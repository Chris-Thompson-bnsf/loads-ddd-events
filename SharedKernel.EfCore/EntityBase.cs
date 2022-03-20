using System.ComponentModel.DataAnnotations;

namespace SharedKernel.EfCore;

public abstract class EntityBase
{
    [Key]
    public virtual int Id { get; protected set; }

    public virtual DateTime CreatedUtc { get; protected set; }
}