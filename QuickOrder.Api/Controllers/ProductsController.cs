using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Products.Commands;
using QuickOrder.Application.Features.Products.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProductsQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProductCommand(request.Name, request.Description), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<ProductDto>.Ok(result.Value!, "Producto creado correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductCommand(id, request.Name, request.Description), cancellationToken);
        return ToResponse(result, "Producto actualizado correctamente.");
    }

    [HttpPost("{id}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UploadProductImageCommand(id, file.OpenReadStream(), file.ContentType, file.FileName, file.Length),
            cancellationToken);
        return ToResponse(result, "Imagen subida correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return ToResponse(result, "Producto eliminado correctamente.");
    }
}
