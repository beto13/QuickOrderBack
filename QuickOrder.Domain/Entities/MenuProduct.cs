namespace QuickOrder.Domain.Entities;

public class MenuProduct
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Menu Menu { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<MenuModifier> MenuModifiers { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
