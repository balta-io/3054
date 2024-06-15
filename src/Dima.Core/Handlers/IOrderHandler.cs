using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IOrderHandler
{
    Task<Response<Order?>> CancelAsync(CancelOrderRequest request);
    Task<Response<Order?>> CreateAsync(CreateOrderRequest request);
    Task<Response<Order?>> PayAsync(PayOrderRequest request);
    Task<Response<Order?>> RefundAsync(RefundOrderRequest request);
}