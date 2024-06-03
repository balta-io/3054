using Dima.Api.Common.Api;
using Dima.Api.Endpoints.Categories;
using Dima.Api.Endpoints.Identity;
using Dima.Api.Endpoints.Orders;
using Dima.Api.Endpoints.Reports;
using Dima.Api.Endpoints.Stripe;
using Dima.Api.Endpoints.Transactions;
using Dima.Api.Models;

namespace Dima.Api.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" });

        endpoints.MapGroup("v1/categories")
            .WithTags("Categories")
            .RequireAuthorization()
            .MapEndpoint<CreateCategoryEndpoint>()
            .MapEndpoint<UpdateCategoryEndpoint>()
            .MapEndpoint<DeleteCategoryEndpoint>()
            .MapEndpoint<GetCategoryByIdEndpoint>()
            .MapEndpoint<GetAllCategoriesEndpoint>();

        endpoints.MapGroup("v1/transactions")
            .WithTags("Transactions")
            .RequireAuthorization()
            .MapEndpoint<CreateTransactionEndpoint>()
            .MapEndpoint<UpdateTransactionEndpoint>()
            .MapEndpoint<DeleteTransactionEndpoint>()
            .MapEndpoint<GetTransactionByIdEndpoint>()
            .MapEndpoint<GetTransactionsByPeriodEndpoint>();
        
        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();
            
        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<GetRolesEndpoint>();

        endpoints.MapGroup("/v1/reports")
            .WithTags("Reports")
            .RequireAuthorization()
            .MapEndpoint<GetIncomesAndExpensesEndpoint>()
            .MapEndpoint<GetIncomesByCategoryEndpoint>()
            .MapEndpoint<GetExpensesByCategoryEndpoint>()
            .MapEndpoint<GetFinancialSummaryEndpoint>();

        endpoints.MapGroup("v1/orders")
            .WithTags("Orders")
            .RequireAuthorization()
            .MapEndpoint<CreateOrderEndpoint>()
            .MapEndpoint<ConfirmOrderEndpoint>();

        endpoints.MapGroup("v1/payments/stripe")
            .WithTags("Payments - Stripe")
            .RequireAuthorization()
            .MapEndpoint<CreateSessionEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}