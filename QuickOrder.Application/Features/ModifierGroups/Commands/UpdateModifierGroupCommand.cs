using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Commands;

public record UpdateModifierGroupCommand(int Id, string? Name, int? MinSelections, int? MaxSelections, bool? IsRequired) : IRequest<Result<ModifierGroupDto>>;

public class UpdateModifierGroupCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateModifierGroupCommand, Result<ModifierGroupDto>>
{
    public async Task<Result<ModifierGroupDto>> Handle(UpdateModifierGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.Id, cancellationToken);
        if (group is null)
            return Result<ModifierGroupDto>.Fail(Error.NotFound($"Grupo de modificadores {request.Id} no encontrado."));

        if (request.Name is not null) group.Name = request.Name;
        if (request.MinSelections.HasValue) group.MinSelections = request.MinSelections.Value;
        if (request.MaxSelections.HasValue) group.MaxSelections = request.MaxSelections.Value;
        if (request.IsRequired.HasValue) group.IsRequired = request.IsRequired.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ModifierGroupDto>.Ok(new ModifierGroupDto(group.Id, group.Name, group.MinSelections, group.MaxSelections, group.IsRequired, group.ProductId, group.CategoryId));
    }
}
