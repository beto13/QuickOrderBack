using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Categories.Commands;
using QuickOrder.Application.Features.Categories.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class CategoriesController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(pageNumber, pageSize), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateCategoryCommand(request.Name, request.DisplayOrder), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ApiResponse<CategoryDto>.Ok(result.Value!, "Categoría creada correctamente."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.DisplayOrder), cancellationToken);
        return ToResponse(result, "Categoría actualizada correctamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return ToResponse(result, "Categoría eliminada correctamente.");
    }
}
