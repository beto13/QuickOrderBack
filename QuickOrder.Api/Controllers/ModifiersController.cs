using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Modifiers.Commands;
using QuickOrder.Application.Features.Modifiers.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModifiersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ModifierDto>>>> GetByGroup(
        [FromQuery] int modifierGroupId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetModifiersByGroupQuery(modifierGroupId, pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<ModifierDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ModifierDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetModifierByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<ModifierDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ModifierDto>>> Create([FromBody] CreateModifierRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateModifierCommand(request.ModifierGroupId, request.Name, request.Description), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ModifierDto>.Ok(result, "Modificador creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ModifierDto>>> Update(int id, [FromBody] UpdateModifierRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateModifierCommand(id, request.Name, request.Description), cancellationToken);
        return Ok(ApiResponse<ModifierDto>.Ok(result, "Modificador actualizado correctamente."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteModifierCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Modificador eliminado correctamente."));
    }
}
