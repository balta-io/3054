using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

public class OrderHandler(IHttpClientFactory httpClientFactory) : IOrderHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders", request);
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível criar seu pedido");
    }

    public async Task<Response<Order?>> ConfirmOrderAsync(ConfirmOrderRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/orders", request);
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível atualizar seu pedido");
    }
}