namespace QuickOrder.Application.DTOs;

public record OrderHistoryDto(
    int Id,
    int TableId,
    string TableNumber,
    int MenuId,
    string MenuName,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<OrderItemDto> Items,
    List<StatusMovementDto> Movements
);

public record StatusMovementDto(string FromStatus, string ToStatus, DateTime ChangedAt);
