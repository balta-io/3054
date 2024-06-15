namespace Dima.Core.Requests.Orders;

public class PayOrderRequest : Request
{
    public long Id { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
}