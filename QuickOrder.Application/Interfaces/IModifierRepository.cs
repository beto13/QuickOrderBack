using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IModifierRepository
{
    Task<(List<Modifier> Items, int TotalCount)> GetPagedAsync(int modifierGroupId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<Modifier> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Modifier?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Modifier modifier);
    void Remove(Modifier modifier);
}
