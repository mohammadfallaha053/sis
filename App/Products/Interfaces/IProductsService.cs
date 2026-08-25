using SisApi.App.Products.Dto.Request.Commands;
using SisApi.App.Products.Dto.Request.Queries;
using SisApi.App.Products.Dto.Response;

namespace SisApi.App.Products.Interfaces;

public interface IProductsService
{
  Task<Result<ProductsResponse>> AddAsync(ProductsCreateCommand command);
  Task<Result<IEnumerable<ProductsResponse>>> GetAllAsync(ProductsGetAllQuery query);
  Task<Result<ProductsResponse>> GetByIdAsync(int id);
  Task<Result<ProductsResponse>> UpdateAsync(int id, ProductsUpdateCommand command);
  Task<Result<ProductPurchaseResponse>> PurchaseAsync(int id, ProductPurchaseCommand command);
  Task<Result<object>> DeleteAsync(int id);
}
