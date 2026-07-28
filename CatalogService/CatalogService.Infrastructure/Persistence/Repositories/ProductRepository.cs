namespace CatalogService.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}