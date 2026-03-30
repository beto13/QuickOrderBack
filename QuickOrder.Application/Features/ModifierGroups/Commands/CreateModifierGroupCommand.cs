using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.ModifierGroups.Commands;

public record CreateModifierGroupCommand(string Name, int MinSelections, int MaxSelections, bool IsRequired, int? ProductId, int? CategoryId) : IRequest<ModifierGroupDto>;

public class CreateModifierGroupCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateModifierGroupCommand, ModifierGroupDto>
{
    public async Task<ModifierGroupDto> Handle(CreateModifierGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new ModifierGroup
        {
            Name = request.Name,
            MinSelections = request.MinSelections,
            MaxSelections = request.MaxSelections,
            IsRequired = request.IsRequired,
            ProductId = request.ProductId,
            CategoryId = request.CategoryId
        };

        modifierGroupRepository.Add(group);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ModifierGroupDto(group.Id, group.Name, group.MinSelections, group.MaxSelections, group.IsRequired, group.ProductId, group.CategoryId);
    }
}
