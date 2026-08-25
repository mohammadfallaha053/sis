using SisApi.App.Categories.Dto.Request.Commands;
using SisApi.App.Categories.Dto.Request.Queries;
using SisApi.App.Categories.Dto.Response;

namespace SisApi.App.Categories.Interfaces;

public interface ICategoriesService
{
  Task<Result<CategoriesResponse>> AddAsync(CategoriesCreateCommand command);
  Task<Result<IEnumerable<CategoriesResponse>>> GetAllAsync(CategoriesGetAllQuery query);
  Task<Result<CategoriesResponse>> GetByIdAsync(int id);
  Task<Result<CategoriesResponse>> UpdateAsync(int id, CategoriesUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}
