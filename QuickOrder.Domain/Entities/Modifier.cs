namespace QuickOrder.Domain.Entities;

public class Modifier
{
    public int Id { get; set; }
    public int ModifierGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ModifierGroup ModifierGroup { get; set; } = null!;
    public ICollection<MenuModifier> MenuModifiers { get; set; } = [];
    public ICollection<OrderItemModifier> OrderItemModifiers { get; set; } = [];
}
