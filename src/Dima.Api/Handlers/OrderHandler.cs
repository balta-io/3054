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
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao obter pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O pedido já foi cancelado!");

            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "Um pedido pago não pode ser cancelado!");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Um pedido reembolsado não pode ser cancelado");

            case EOrderStatus.WaitingPayment:
                break;

            default:
                return new Response<Order?>(order, 400, "Pedido com situação inválida!");
        }

        order.Status = EOrderStatus.Canceled;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Não foi possível atualizar seu pedido");
        }

        return new Response<Order?>(order, 200, $"Pedido {order.Number} atualizado!");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        Product? product;
        try
        {
            product = await context
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive == true);

            if (product is null)
                return new Response<Order?>(null, 404, "Produto não encontrado ou inativo");

            context.Attach(product);
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao verificar produto");
        }

        Voucher? voucher = null;
        try
        {
            if (request.VoucherId is not null)
            {
                voucher = await context
                    .Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.VoucherId && x.IsActive == true);

                if (voucher is null)
                    return new Response<Order?>(null, 404, "Voucher não encontrado ou inativo");

                if (voucher.IsActive == false)
                    return new Response<Order?>(null, 404, "Este voucher já foi utilizado");

                voucher.IsActive = false;

                // context.Attach(voucher);
                context.Vouchers.Update(voucher);
            }
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao verificar voucher");
        }

        var order = new Order
        {
            UserId = request.UserId,
            Product = product,
            ProductId = request.ProductId,
            Voucher = voucher,
            VoucherId = request.VoucherId
        };

        try
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível registrar seu pedido");
        }

        return new Response<Order?>(order, 201, $"Pedido {order.Number} cadastrado com sucesso!");
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao consultar pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O está cancelado!");

            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "O pedido já foi pago");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Um pedido reembolsado não pode ser cancelado");

            case EOrderStatus.WaitingPayment:
                break;

            default:
                return new Response<Order?>(order, 400, "Situação do pedido inválida");
        }

        order.Status = EOrderStatus.Paid;
        order.ExternalReference = request.ExternalReference;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Não foi possível realizar o pagamento do seu pedido!");
        }

        return new Response<Order?>(order, 200, $"Pedido {order.Number} pago com sucesso!");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao consultar seu pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "O pedido está cancelado!");

            case EOrderStatus.WaitingPayment:
                return new Response<Order?>(order, 400, "O pedido ainda não foi pago");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "O pedido já foi reembolsado");

            case EOrderStatus.Paid:
                break;

            default:
                return new Response<Order?>(order, 400, "Situação do pedido inválida");
        }

        order.Status = EOrderStatus.Refunded;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Não foi possível reembolsar seu pedido");
        }

        return new Response<Order?>(order, 200, $"Pedido {order.Number} estornado com sucesso!");
    }
}