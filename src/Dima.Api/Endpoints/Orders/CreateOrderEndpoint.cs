using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Endpoints.Orders;

public class CreateOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Orders: Create a new order")
            .WithSummary("Cria um novo pedido")
            .WithDescription("Cria um novo pedido")
            .WithOrder(1)
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        CreateOrderRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity!.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"v1/orders/{result.Data?.Number}", result)
            : TypedResults.BadRequest(result);
    }
}