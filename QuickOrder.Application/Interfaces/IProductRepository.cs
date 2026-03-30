using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Product product);
    void Remove(Product product);
}
