namespace CatalogService.Application.Features.Products.Queries.GetProductById;

using MediatR;
using CatalogService.Application.DTOs;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;