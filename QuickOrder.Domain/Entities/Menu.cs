namespace QuickOrder.Domain.Entities;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<MenuProduct> MenuProducts { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
