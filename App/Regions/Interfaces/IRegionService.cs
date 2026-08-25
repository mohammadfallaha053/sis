using SisApi.App.Regions.Dto.Request.Commands;
using SisApi.App.Regions.Dto.Request.Queries;
using SisApi.App.Regions.Dto.Response;
namespace SisApi.App.Regions.Interfaces;

public interface IRegionService
{
  Task<Result<RegionResponse>> AddAsync(RegionCreateCommand command);
  Task<Result<IEnumerable<RegionResponse>>> GetAllAsync(RegionGetAllQuery query);
  
  Task<Result<RegionResponse>> GetByIdAsync(int id);
  Task<Result<RegionResponse>> UpdateAsync(int id, RegionUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}