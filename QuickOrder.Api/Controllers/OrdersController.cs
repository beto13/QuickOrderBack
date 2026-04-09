using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Features.Orders.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ApiController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveOrders(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActiveOrdersQuery(), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.TableId, request.MenuId, request.Notes, request.Items);
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<OrderDto>.Ok(result.Value!, "Pedido creado correctamente."));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateOrderStatusCommand(id, request.Status), cancellationToken);
        return ToResponse(result, "Estado actualizado correctamente.");
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetOrderHistoryQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }
}
