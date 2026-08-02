namespace CatalogService.Api.Services;

using Grpc.Core;
using MediatR;
using CatalogService.Api.Protos;
using CatalogService.Application.Features.Products.Queries.GetProductById;

public class CatalogGrpcService : CatalogGrpc.CatalogGrpcBase
{
    private readonly IMediator _mediator;

    public CatalogGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ProductResponse> CheckProductExists(ProductRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return new ProductResponse { Exists = false };
        }

        var query = new GetProductByIdQuery(productId);
        var product = await _mediator.Send(query);

        return new ProductResponse
        {
            Exists = product != null
        };
    }
}