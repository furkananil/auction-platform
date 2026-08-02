namespace BiddingService.Application.Common.Interfaces;

public interface ICatalogGrpcClient
{
    Task<bool> CheckProductExistsAsync(Guid productId, CancellationToken cancellationToken);
}