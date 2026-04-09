using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Menus.Commands;
using QuickOrder.Application.Features.Menus.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class MenusController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllMenusQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMenuByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMenuCommand(request.Name), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<MenuDto>.Ok(result.Value!, "Menú creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMenuCommand(id, request.Name, request.IsActive), cancellationToken);
        return ToResponse(result, "Menú actualizado correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMenuCommand(id), cancellationToken);
        return ToResponse(result, "Menú desactivado correctamente.");
    }
}
