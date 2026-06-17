using LapisApi.App.Regions.Dto;
using SisApi.App.Regions.Dto;
using SisApi.App.Regions.Dto.Request.Queries;
namespace SisApi.App.Regions.Interfaces;

public interface IRegionService
{
  Task<Result<RegionResponse>> AddAsync(RegionCreateCommand command);
  Task<Result<IEnumerable<RegionResponse>>> GetAllAsync(RegionGetAllQuery query);
  
  Task<Result<IEnumerable<RegionAutoCompleteResponse>>> GetAutoComplete(RegionGetAutoCompleteQuery query);
  
  Task<Result<RegionResponse>> GetByIdAsync(int id);
  Task<Result<RegionResponse>> UpdateAsync(int id, RegionUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}