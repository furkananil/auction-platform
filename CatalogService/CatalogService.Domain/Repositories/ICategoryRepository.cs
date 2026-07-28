namespace CatalogService.Domain.Repositories;

using CatalogService.Domain.Entities;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task UpdateAsync(Category category, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}