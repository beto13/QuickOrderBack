using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Features.Menu.Queries;

namespace QuickOrder.Api.Controllers;

[Route("api/[controller]")]
public class MenuController(IMediator mediator) : ApiController
{
    [HttpGet("{menuId:int}")]
    public async Task<IActionResult> Get(int menuId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMenuQuery(menuId), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("{menuId:int}/products/{menuProductId:int}/modifiers")]
    public async Task<IActionResult> GetModifiers(int menuId, int menuProductId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductModifiersQuery(menuProductId), cancellationToken);
        return ToResponse(result);
    }
}
