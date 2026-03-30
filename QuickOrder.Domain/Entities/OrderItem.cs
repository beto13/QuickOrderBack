namespace QuickOrder.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }

    public Order Order { get; set; } = null!;
    public MenuProduct MenuProduct { get; set; } = null!;
    public ICollection<OrderItemModifier> Modifiers { get; set; } = [];
}
