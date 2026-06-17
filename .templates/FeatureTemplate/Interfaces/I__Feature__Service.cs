using TransfersApi.App.__Feature__s.Dto;
using TransfersApi.App.__Feature__s.Dto.Request.Commands;
using TransfersApi.App.__Feature__s.Dto.Request.Queries;
using TransfersApi.App.__Feature__s.Dto.Response;
namespace TransfersApi.App.__Feature__s.Interfaces;

public interface I__Feature__Service
{
  Task<Result<__Feature__Response>> AddAsync(__Feature__CreateCommand command);
  Task<Result<IEnumerable<__Feature__Response>>> GetAllAsync(__Feature__GetAllQuery query);
  Task<Result<__Feature__Response>> GetByIdAsync(int id);
  Task<Result<__Feature__Response>> UpdateAsync(int id, __Feature__UpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}