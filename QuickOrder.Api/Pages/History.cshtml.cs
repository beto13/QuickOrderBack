using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Queries;

namespace QuickOrder.Api.Pages;

public class HistoryModel(IMediator mediator) : PageModel
{
    public List<OrderHistoryDto> Orders { get; set; } = [];

    public async Task OnGetAsync()
    {
        Orders = await mediator.Send(new GetOrderHistoryQuery());
    }
}
