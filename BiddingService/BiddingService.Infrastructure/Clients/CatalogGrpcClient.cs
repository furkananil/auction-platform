namespace BiddingService.Infrastructure.Clients;

using BiddingService.Application.Common.Interfaces;
using CatalogService.Api.Protos;

public class CatalogGrpcClient : ICatalogGrpcClient
{
    private readonly CatalogGrpc.CatalogGrpcClient _client;

    public CatalogGrpcClient(CatalogGrpc.CatalogGrpcClient client)
    {
        _client = client;
    }

    public async Task<bool> CheckProductExistsAsync(Guid productId, CancellationToken cancellationToken)
    {
        var request = new ProductRequest { ProductId = productId.ToString() };
        var response = await _client.CheckProductExistsAsync(request, cancellationToken: cancellationToken);
        return response.Exists;
    }
}