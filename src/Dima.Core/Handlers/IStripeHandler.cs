using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IStripeHandler
{
    Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request);
}