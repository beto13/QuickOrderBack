using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Tables.Commands;
using QuickOrder.Application.Features.Tables.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class TablesController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllTablesQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("by-number/{number}")]
    public async Task<IActionResult> GetByNumber(string number, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTableByNumberQuery(number), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTableByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateTableCommand(request.Number, request.MenuId), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<TableDto>.Ok(result.Value!, "Mesa creada correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateTableCommand(id, request.Number, request.IsActive, request.MenuId), cancellationToken);
        return ToResponse(result, "Mesa actualizada correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTableCommand(id), cancellationToken);
        return ToResponse(result, "Mesa desactivada correctamente.");
    }
}
