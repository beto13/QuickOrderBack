using QuickOrder.Application.DTOs;

namespace QuickOrder.Application.Interfaces;

public interface IOrderHub
{
    Task NotifyNewOrder(OrderDto order);
    Task NotifyOrderStatusChanged(int orderId, string status);
}
