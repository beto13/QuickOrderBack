using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IMenuProductRepository
{
    Task<Dictionary<int, MenuProduct>> GetByIdsAsync(IEnumerable<int> menuProductIds, CancellationToken cancellationToken = default);
}
