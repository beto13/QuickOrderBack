using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Queries;

namespace QuickOrder.Api.Pages;

public class HistoryModel(IMediator mediator) : PageModel
{
    public PaginatedResponse<OrderHistoryDto> Orders { get; set; } = PaginatedResponse<OrderHistoryDto>.Create([], 0, 1, 20);

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 20)
    {
        var result = await mediator.Send(new GetOrderHistoryQuery(pageNumber, pageSize));
        if (result.IsSuccess)
            Orders = result.Value!;
    }
}
