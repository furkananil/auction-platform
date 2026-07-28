namespace CatalogService.Application.Features.Products.Commands.CreateProduct;

using MediatR;

public record CreateProductCommand(
    string Name,
    string Description,
    Guid SellerId,
    Guid CategoryId,
    decimal StartingPrice,
    List<string> ImageUrls) : IRequest<Guid>;