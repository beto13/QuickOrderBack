using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Products.Commands;
using QuickOrder.Application.Features.Products.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ProductDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProductsQuery(pageNumber, pageSize), cancellationToken);
        return Ok(ApiResponse<PaginatedResponse<ProductDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProductCommand(request.Name, request.Description), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductDto>.Ok(result, "Producto creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductCommand(id, request.Name, request.Description), cancellationToken);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Producto actualizado correctamente."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Producto eliminado correctamente."));
    }
}
