namespace QuickOrder.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<MenuProduct> MenuProducts { get; set; } = [];
    public ICollection<ModifierGroup> ModifierGroups { get; set; } = [];
}
