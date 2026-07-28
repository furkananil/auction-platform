namespace CatalogService.Domain.Entities;

using CatalogService.Domain.Common;

public class Category : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    private Category() { }

    public Category(string name, string description, Guid? parentCategoryId = null)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public void Update(string name, string description, Guid? parentCategoryId)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        SetUpdatedDate();
    }
}