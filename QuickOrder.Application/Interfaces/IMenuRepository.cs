using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IMenuRepository
{
    Task<(List<Menu> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Menu?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Menu menu);
}
