using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface ITableRepository
{
    Task<(List<Table> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Table?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Table?> FindByNumberAsync(string number, CancellationToken cancellationToken = default);
    void Add(Table table);
}
