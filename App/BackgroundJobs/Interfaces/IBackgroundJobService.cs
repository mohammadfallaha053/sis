using LapisApi.App.BackgroundJobs.Dto;
using LapisApi.App.BackgroundJobs.Dto.Request.Commands;
using SisApi.App.BackgroundJobs.Dto.Response;
namespace LapisApi.App.BackgroundJobs.Interfaces;

public interface IBackgroundJobService
{
  Task<Result<BackgroundJobResponse>> AddAsync(BackgroundJobCreateRequest dto);
  Task<Result<IEnumerable<BackgroundJobResponse>>> GetAllAsync(BackgroundJobGetAllQuery query);
  Task<Result<BackgroundJobResponse>> GetByIdAsync(string id);
  Task<Result<BackgroundJobResponse>> UpdateAsync(string id, BackgroundJobUpdateRequest request);
  Task<Result<object>> DeleteAsync(string id);
  
  Task EnqueueAsync<TPayload>(string jobType, TPayload payload)
    where TPayload : IBackgroundJobPayload;
}