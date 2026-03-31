using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Features.Orders.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<OrderDto>.Ok(result));
    }

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetActiveOrders(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActiveOrdersQuery(), cancellationToken);
        return Ok(ApiResponse<List<OrderDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.TableId, request.MenuId, request.Notes, request.Items);
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrderDto>.Ok(result, "Pedido creado correctamente."));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateOrderStatusCommand(id, request.Status), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Estado actualizado correctamente."));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<OrderHistoryDto>>>> GetHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetOrderHistoryQuery(pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<OrderHistoryDto>>.Ok(result));
    }
}
