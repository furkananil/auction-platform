namespace CatalogService.Application.Features.Products.Queries.GetProductById;

using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Repositories;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IDistributedCache _cache;
    private const string CacheKeyPrefix = "Product_";

    public GetProductByIdQueryHandler(IProductRepository productRepository, IDistributedCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"{CacheKeyPrefix}{request.Id}";
        var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedProduct);
        }

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return null;

        var dto = new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.SellerId,
            product.CategoryId,
            product.StartingPrice,
            product.ImageUrls);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(dto), 
            cacheOptions, 
            cancellationToken);

        return dto;
    }
}