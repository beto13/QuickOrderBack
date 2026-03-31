namespace QuickOrder.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<MenuProduct> MenuProducts { get; set; } = [];
    public ICollection<ModifierGroup> ModifierGroups { get; set; } = [];
}
