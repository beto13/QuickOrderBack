using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllWithMenuProductsAsync(int menuId, CancellationToken cancellationToken = default);
    Task<(List<Category> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Category?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Category category);
    void Remove(Category category);
}
