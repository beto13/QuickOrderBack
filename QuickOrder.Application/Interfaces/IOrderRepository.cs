using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Order>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(Order order);
    void AddStatusHistory(OrderStatusHistory history);
}
