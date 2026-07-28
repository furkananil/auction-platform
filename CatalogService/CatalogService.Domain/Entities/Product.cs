namespace CatalogService.Domain.Entities;

using CatalogService.Domain.Common;

public class Product : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal StartingPrice { get; private set; }
    public List<string> ImageUrls { get; private set; }

    private Product() { }

    public Product(string name, string description, Guid sellerId, Guid categoryId, decimal startingPrice, List<string> imageUrls)
    {
        Name = name;
        Description = description;
        SellerId = sellerId;
        CategoryId = categoryId;
        StartingPrice = startingPrice;
        ImageUrls = imageUrls ?? new List<string>();
    }

    public void Update(string name, string description, Guid categoryId, decimal startingPrice, List<string> imageUrls)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        StartingPrice = startingPrice;
        ImageUrls = imageUrls;
        SetUpdatedDate();
    }
}