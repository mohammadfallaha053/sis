using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Request.Queries;
using SisApi.App.Centers.Dto.Response;
namespace SisApi.App.Centers.Interfaces;

public interface ICentersService
{
  Task<Result<CentersResponse>> AddAsync(CentersCreateCommand command);
  Task<Result<IEnumerable<CentersResponse>>> GetAllAsync(CentersGetAllQuery query);
  Task<Result<CentersResponse>> GetByIdAsync(int id);
  Task<Result<CentersResponse>> UpdateAsync(int id, CentersUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}