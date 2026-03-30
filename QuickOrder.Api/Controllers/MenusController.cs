using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Menus.Commands;
using QuickOrder.Application.Features.Menus.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MenuDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllMenusQuery(pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<MenuDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMenuByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<MenuDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMenuCommand(request.Name), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<MenuDto>.Ok(result, "Menú creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMenuCommand(id, request.Name, request.IsActive), cancellationToken);
        return Ok(ApiResponse<MenuDto>.Ok(result, "Menú actualizado correctamente."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteMenuCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Menú desactivado correctamente."));
    }
}
