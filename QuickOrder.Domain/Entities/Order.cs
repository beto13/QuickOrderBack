using QuickOrder.Domain.Enums;

namespace QuickOrder.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int MenuId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Table Table { get; set; } = null!;
    public Menu Menu { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = [];
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
}
