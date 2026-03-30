namespace QuickOrder.Domain.Entities;

public class OrderItemModifier
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public int ModifierId { get; set; }
    public decimal ExtraPrice { get; set; }

    public OrderItem OrderItem { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}
