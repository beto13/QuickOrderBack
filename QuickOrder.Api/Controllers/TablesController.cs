using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Tables.Commands;
using QuickOrder.Application.Features.Tables.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TableDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllTablesQuery(pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<TableDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TableDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTableByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<TableDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TableDto>>> Create([FromBody] CreateTableRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateTableCommand(request.Number), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TableDto>.Ok(result, "Mesa creada correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TableDto>>> Update(int id, [FromBody] UpdateTableRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateTableCommand(id, request.Number, request.IsActive), cancellationToken);
        return Ok(ApiResponse<TableDto>.Ok(result, "Mesa actualizada correctamente."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTableCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Mesa desactivada correctamente."));
    }
}
