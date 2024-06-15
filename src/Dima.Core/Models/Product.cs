namespace Dima.Core.Models;

public class Product
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal Price { get; set; }
}
