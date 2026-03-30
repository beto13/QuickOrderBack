namespace QuickOrder.Domain.Entities;

public class MenuModifier
{
    public int Id { get; set; }
    public int MenuProductId { get; set; }
    public int ModifierId { get; set; }
    public decimal ExtraPrice { get; set; } = 0;
    public bool IsAvailable { get; set; } = true;

    public MenuProduct MenuProduct { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}
