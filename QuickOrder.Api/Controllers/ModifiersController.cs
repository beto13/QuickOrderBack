using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Modifiers.Commands;
using QuickOrder.Application.Features.Modifiers.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class ModifiersController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? modifierGroupId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = modifierGroupId.HasValue
            ? await mediator.Send(new GetModifiersByGroupQuery(modifierGroupId.Value, pageNumber, pageSize), cancellationToken)
            : await mediator.Send(new GetAllModifiersQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetModifierByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModifierRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateModifierCommand(request.ModifierGroupId, request.Name, request.Description), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<ModifierDto>.Ok(result.Value!, "Modificador creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModifierRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateModifierCommand(id, request.Name, request.Description), cancellationToken);
        return ToResponse(result, "Modificador actualizado correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteModifierCommand(id), cancellationToken);
        return ToResponse(result, "Modificador eliminado correctamente.");
    }
}
