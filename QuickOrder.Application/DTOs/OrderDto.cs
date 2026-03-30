namespace QuickOrder.Application.DTOs;

public record OrderDto(
    int Id,
    int TableId,
    string TableNumber,
    int MenuId,
    string MenuName,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

public record OrderItemDto(int MenuProductId, string ProductName, int Quantity, decimal UnitPrice, string? Notes);

public record CreateOrderRequest(int TableId, int MenuId, string? Notes, List<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(int MenuProductId, int Quantity, string? Notes);

public record UpdateOrderStatusRequest(string Status);
