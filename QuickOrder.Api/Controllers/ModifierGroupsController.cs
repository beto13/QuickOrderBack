using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.ModifierGroups.Commands;
using QuickOrder.Application.Features.ModifierGroups.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class ModifierGroupsController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllModifierGroupsQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetModifierGroupByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModifierGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateModifierGroupCommand(request.Name, request.MinSelections, request.MaxSelections, request.IsRequired, request.ProductId, request.CategoryId);
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<ModifierGroupDto>.Ok(result.Value!, "Grupo de modificadores creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModifierGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateModifierGroupCommand(id, request.Name, request.MinSelections, request.MaxSelections, request.IsRequired);
        var result = await mediator.Send(command, cancellationToken);
        return ToResponse(result, "Grupo de modificadores actualizado correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteModifierGroupCommand(id), cancellationToken);
        return ToResponse(result, "Grupo de modificadores eliminado correctamente.");
    }
}
