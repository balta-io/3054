namespace Dima.Core.Requests.Stripe;

public class CreateSessionRequest : Request
{
    public string OrderNumber { get; set; } = string.Empty;
}