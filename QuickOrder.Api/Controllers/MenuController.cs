using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;
using QuickOrder.Application.Features.Menu.Queries;

namespace QuickOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController(IMediator mediator) : ControllerBase
{
    [HttpGet("{menuId:int}")]
    public async Task<ActionResult<ApiResponse<List<MenuCategoryDto>>>> Get(int menuId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMenuQuery(menuId), cancellationToken);
        return Ok(ApiResponse<List<MenuCategoryDto>>.Ok(result));
    }
}
