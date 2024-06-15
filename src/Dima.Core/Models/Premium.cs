namespace Dima.Core.Models;

public class Premium
{
    public long Id { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime EndedAt { get; set; } = DateTime.Now;

    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public bool IsActive
        => StartedAt <= DateTime.Now && EndedAt >= DateTime.Now;
}
