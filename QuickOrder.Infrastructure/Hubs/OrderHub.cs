using Microsoft.AspNetCore.SignalR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Infrastructure.Hubs;

public class OrderHub : Hub
{
    public const string Url = "/hubs/orders";

    public async Task BroadcastOrderStatusChanged(int orderId, string status) =>
        await Clients.Others.SendAsync("OrderStatusChanged", new { orderId, status });

    public async Task BroadcastNewOrder(int orderId) =>
        await Clients.Others.SendAsync("NewOrderAlert", orderId);
}

public class OrderHubService(IHubContext<OrderHub> hubContext) : IOrderHub
{
    public async Task NotifyNewOrder(OrderDto order) =>
        await hubContext.Clients.All.SendAsync("NewOrder", order);

    public async Task NotifyOrderStatusChanged(int orderId, string status) =>
        await hubContext.Clients.All.SendAsync("OrderStatusChanged", new { orderId, status });
}
