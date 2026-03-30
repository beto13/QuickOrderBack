using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.ModifierGroups.Commands;
using QuickOrder.Application.Features.ModifierGroups.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/modifier-groups")]
public class ModifierGroupsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ModifierGroupDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllModifierGroupsQuery(pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<ModifierGroupDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ModifierGroupDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetModifierGroupByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<ModifierGroupDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ModifierGroupDto>>> Create([FromBody] CreateModifierGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateModifierGroupCommand(request.Name, request.MinSelections, request.MaxSelections, request.IsRequired, request.ProductId, request.CategoryId);
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ModifierGroupDto>.Ok(result, "Grupo de modificadores creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ModifierGroupDto>>> Update(int id, [FromBody] UpdateModifierGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateModifierGroupCommand(id, request.Name, request.MinSelections, request.MaxSelections, request.IsRequired);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<ModifierGroupDto>.Ok(result, "Grupo de modificadores actualizado correctamente."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteModifierGroupCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Grupo de modificadores eliminado correctamente."));
    }
}
