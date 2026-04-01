namespace QuickOrder.Domain.Entities;

public class Table
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int MenuId { get; set; }

    public Menu? Menu { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
}
