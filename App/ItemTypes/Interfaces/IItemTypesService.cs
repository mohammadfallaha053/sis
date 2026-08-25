using LapisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Request.Queries;
using SisApi.App.ItemTypes.Dto.Response;
namespace SisApi.App.ItemTypes.Interfaces;

public interface IItemTypesService
{
  Task<Result<ItemTypesResponse>> AddAsync(ItemTypesCreateCommand command);
  Task<Result<IEnumerable<ItemTypesResponse>>> GetAllAsync(ItemTypesGetAllQuery query);
  Task<Result<ItemTypesResponse>> GetByIdAsync(int id);
  Task<Result<ItemTypesResponse>> UpdateAsync(int id, ItemTypesUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}