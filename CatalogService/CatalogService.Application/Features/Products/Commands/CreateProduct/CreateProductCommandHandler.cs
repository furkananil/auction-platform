namespace CatalogService.Application.Features.Products.Commands.CreateProduct;

using MediatR;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Name,
            request.Description,
            request.SellerId,
            request.CategoryId,
            request.StartingPrice,
            request.ImageUrls);

        await _productRepository.AddAsync(product, cancellationToken);

        return product.Id;
    }
}