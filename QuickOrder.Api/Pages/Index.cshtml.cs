using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Queries;

namespace QuickOrder.Api.Pages;

public class IndexModel(IMediator mediator) : PageModel
{
    public List<OrderDto> Orders { get; set; } = [];

    public async Task OnGetAsync()
    {
        Orders = await mediator.Send(new GetActiveOrdersQuery());
    }
}
