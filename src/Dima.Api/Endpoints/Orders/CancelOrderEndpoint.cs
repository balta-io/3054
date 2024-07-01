using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Endpoints.Orders;

public class CancelOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/cancel", HandleAsync)
            .WithName("Orders: Cancel a new order")
            .WithSummary("Cancela um pedido")
            .WithDescription("Cancela um pedido")
            .WithOrder(2)
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        CancelOrderRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity!.Name ?? string.Empty;

        var result = await handler.CancelAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}