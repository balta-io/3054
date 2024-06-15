using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IProductHandler
{
    Task<PagedResponse<List<Product>>> GetAllAsync(GetAllProductsRequest request);
    Task<Response<Product?>> GetByIdAsync(GetProductByIdRequest request);
}