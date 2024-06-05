using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IOrderHandler
{
    Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request);
    Task<Response<Order?>> ConfirmOrderAsync(ConfirmOrderRequest request);
}