namespace QuickOrder.Application.Messages;

public record OrderCreatedEvent(
    int OrderId,
    int TableId,
    string TableNumber,
    string? Notes,
    DateTime CreatedAt,
    List<OrderCreatedItemEvent> Items
);

public record OrderCreatedItemEvent(
    int MenuProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string? Notes
);
