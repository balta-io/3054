using Dima.Api.Data;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class OrderHandler(AppDbContext context) : IOrderHandler
{
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        var order = await context
            .Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

        if (order is null)
            return new Response<Order?>(null, 404, "Pedido não encontrado");

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O pedido já foi cancelado!");

            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "Um pedido pago não pode ser cancelado!");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Um pedido reembolsado não pode ser cancelado");

            case EOrderStatus.WaitingPayment: break;
        }

        order.Status = EOrderStatus.Canceled;
        order.UpdatedAt = DateTime.Now;

        context.Orders.Update(order);
        await context.SaveChangesAsync();

        return new Response<Order?>(order, 200, $"Pedido {order.Number} atualizado!");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        var productExists = await context
            .Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ProductId && x.IsActive == true);

        if (productExists is false)
            return new Response<Order?>(null, 404, "Produto não encontrado ou inativo");

        if (request.VoucherId is not null)
        {
            var voucherExists = await context
                .Vouchers
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.VoucherId && x.IsActive == true);

            if (voucherExists is false)
                return new Response<Order?>(null, 404, "Voucher não encontrado ou inativo");
        }

        var order = new Order
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            VoucherId = request.VoucherId
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return new Response<Order?>(order, 201, $"Pedido {order.Number} cadastrado com sucesso!");
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        var order = await context
            .Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

        if (order is null)
            return new Response<Order?>(null, 404, "Pedido não encontrado");

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O está cancelado!");

            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "O pedido já foi pago");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Um pedido reembolsado não pode ser cancelado");

            case EOrderStatus.WaitingPayment: break;
        }

        order.Status = EOrderStatus.Paid;
        order.ExternalReference = request.ExternalReference;
        order.UpdatedAt = DateTime.Now;

        context.Orders.Update(order);
        await context.SaveChangesAsync();

        return new Response<Order?>(order, 200, $"Pedido {order.Number} pago com sucesso!");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        var order = await context
            .Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

        if (order is null)
            return new Response<Order?>(null, 404, "Pedido não encontrado");

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O pedido está cancelado!");

            case EOrderStatus.WaitingPayment:
                return new Response<Order?>(order, 400, "O pedido ainda não foi pago");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "O pedido já foi reembolsado");

            case EOrderStatus.Paid: break;
        }

        order.Status = EOrderStatus.Refunded;
        order.UpdatedAt = DateTime.Now;

        context.Orders.Update(order);
        await context.SaveChangesAsync();

        return new Response<Order?>(order, 200, $"Pedido {order.Number} estornado com sucesso!");
    }
}