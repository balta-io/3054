using Dima.Core.Enums;

namespace Dima.Core.Models;

public class Order
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public EOrderStatus Status { get; set; } = EOrderStatus.WaitingPayment;
    public EPaymentGateway Gateway { get; set; } = EPaymentGateway.Stripe;
    public decimal Amount { get; set; }
    public string UserId { get; set; } = string.Empty;
}