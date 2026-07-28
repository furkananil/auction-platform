namespace CatalogService.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    Guid SellerId,
    Guid CategoryId,
    decimal StartingPrice,
    List<string> ImageUrls);