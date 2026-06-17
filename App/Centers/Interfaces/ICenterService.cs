using LapisApi.App.Centers.Dto;
using LapisApi.App.Centers.Dto.Request.Commands;
using LapisApi.App.Centers.Dto.Request.Queries;
using LapisApi.App.Centers.Dto.Response;
namespace LapisApi.App.Centers.Interfaces;

public interface ICenterService
{
    Task<Result<CenterResponse>> AddCenterAsync(CenterCreateCommand command);
    Task<Result<IEnumerable<CenterResponse>>> GetAllCentersAsync(CenterGetAllQuery query);
  
    Task<Result<IEnumerable<CenterGetForClientResponse>>> GetCentersAsync(CenterGetForClientQuery query);

    
    Task<Result<CenterResponse>> GetCenterByIdAsync(string id);
    
    Task<Result<CenterInfoResponse>> GetCenterInfo();
    Task<Result<CenterResponse>> UpdateCenterAsync(string id, CenterUpdateCommand command);
    
    Task<Result<CenterResponse>> UpdateCenterInfoAsync(CenterUpdateInfoCommand command);
    
    Task<Result<object>> DeleteCenterAsync(string id);
}