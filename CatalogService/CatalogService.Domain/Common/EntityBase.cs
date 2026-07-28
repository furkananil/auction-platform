namespace CatalogService.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected void SetUpdatedDate() => UpdatedAt = DateTime.UtcNow;
}