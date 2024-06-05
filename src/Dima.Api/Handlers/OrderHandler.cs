using Dima.Api.Data;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Dima.Api.Handlers;

public class OrderHandler(
    AppDbContext context,
    UserManager<Models.User> userManager,
    SignInManager<Models.User> signInManager) : IOrderHandler
{
    public async Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            var order = new Order
            {
                CreatedAt = DateTime.Now,
                ExternalReference = "",
                Amount = 799.90M,
                UserId = request.UserId,
                Gateway = EPaymentGateway.Stripe,
                Number = Guid.NewGuid().ToString()[0..8],
                Status = EOrderStatus.WaitingPayment,
                UpdatedAt = DateTime.Now
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            return new Response<Order?>(order);
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível criar o pedido");
        }
    }

    public async Task<Response<Order?>> ConfirmOrderAsync(ConfirmOrderRequest request)
    {
        var order = await context.Orders.FirstOrDefaultAsync(x =>
            x.UserId == request.UserId 
            && x.Number == request.Number);

        if (order is null)
            return new Response<Order?>(null, 404, "Pedido não encontrado");

        if (order.Status == EOrderStatus.Paid)
            return new Response<Order?>(null, 400, "Este pedido já foi confirmado");

        var options = new ChargeSearchOptions
        {
            Query = $"metadata['order']:'{request.Number}'",
            Limit = 1
        };
        var service = new ChargeService();
        var result = await service.SearchAsync(options);

        if (result is null)
            return new Response<Order?>(null, 404, "Pagamento não encontrado");

        var charge = result.Data.FirstOrDefault();
        if (charge is null)
            return new Response<Order?>(null, 404, "Pagamento não encontrado");
        
        if (charge.Paid == false)
            return new Response<Order?>(null, 400, "Este pedido ainda não foi pago");

        order.Status = EOrderStatus.Paid;
        order.UpdatedAt = DateTime.Now;
        order.ExternalReference = charge.Id;

        context.Orders.Update(order);
        await context.SaveChangesAsync();

        var user = await userManager.FindByEmailAsync(request.UserId);
        if (user is null)
            return new Response<Order?>(null, 500, "Perfil não encontrado");

        var addRoleResult = await userManager.AddToRoleAsync(user, "Premium");
        if (addRoleResult.Succeeded == false)
            return new Response<Order?>(null, 500, "Não foi possível atribuir o perfil ao usuário");

        await signInManager.SignOutAsync();
        await signInManager.SignInAsync(user, true);

        return new Response<Order?>(order);
    }
}