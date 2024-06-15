namespace Dima.Core.Requests.Orders;

public class RefundOrderRequest : Request
{
    public long OrderId { get; set; }
}