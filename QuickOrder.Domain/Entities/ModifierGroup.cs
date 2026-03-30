namespace QuickOrder.Domain.Entities;

public class ModifierGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinSelections { get; set; } = 0;
    public int MaxSelections { get; set; } = 1;
    public bool IsRequired { get; set; } = false;

    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }

    public Product? Product { get; set; }
    public Category? Category { get; set; }
    public ICollection<Modifier> Modifiers { get; set; } = [];
}
