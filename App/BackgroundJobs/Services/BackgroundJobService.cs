using AutoMapper;
using System.Linq.Expressions;
using System.Text.Json;
using LapisApi.App.BackgroundJobs.Dto;
using LapisApi.App.BackgroundJobs.Dto.Request.Commands;
using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Errors;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Jobs.Payloads;
using LapisApi.App.BackgroundJobs.Model;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using SisApi.App.BackgroundJobs.Dto.Response;
namespace LapisApi.App.BackgroundJobs.Services;

public class BackgroundJobService : IBackgroundJobService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;

  public BackgroundJobService(
    IUnitOfWork unitOfWork,
    IMapper mapper
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
  }

  public async Task<Result<BackgroundJobResponse>> AddAsync(BackgroundJobCreateRequest request)
  {
    await EnqueueAsync(
      BackgroundJobTypes.CleanBackgroundJobHandler,
      new CleanBackgroundJobPayload()
    );

    await _unitOfWork.SaveChangesAsync();
    return Result<BackgroundJobResponse>.Success(null);
  }

  public async Task<Result<BackgroundJobResponse>> UpdateAsync(string id, BackgroundJobUpdateRequest request)
  {
    var BackgroundJob = await _unitOfWork.BackgroundJobs.GetByIdAsync(id);
    if (BackgroundJob == null)
    {
      return Result<BackgroundJobResponse>.Failure(BackgroundJobErrors.NotFound);
    }

    _mapper.Map(request, BackgroundJob);
    await _unitOfWork.BackgroundJobs.UpdateAsync(BackgroundJob);
    await _unitOfWork.SaveChangesAsync();

    return Result<BackgroundJobResponse>.Success(_mapper.Map<BackgroundJobResponse>(BackgroundJob));
  }

  public async Task<Result<IEnumerable<BackgroundJobResponse>>> GetAllAsync(BackgroundJobGetAllQuery query)
  {
    Expression<Func<BackgroundJob, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.ErrorMessage.Contains(query.Search)
      );

    var sortFunc = SortHelper.BuildSort<BackgroundJob, BackgroundJobSortFieldEnum>(query.Sort);

    var pagedResult = await _unitOfWork.BackgroundJobs.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      sort: sortFunc
    );

    var data = _mapper.Map<IEnumerable<BackgroundJobResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<BackgroundJobResponse>>.Success(data, paging);
  }

  public async Task<Result<BackgroundJobResponse>> GetByIdAsync(string id)
  {
    var BackgroundJob =
      await _unitOfWork.BackgroundJobs
        .GetFirstOrDefaultAsync(
          o => o.Id == id
        );

    if (BackgroundJob == null)
    {
      return Result<BackgroundJobResponse>.Failure(BackgroundJobErrors.NotFound);
    }

    var data = _mapper.Map<BackgroundJobResponse>(BackgroundJob);
    return Result<BackgroundJobResponse>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(string id)
  {
    var BackgroundJob = await _unitOfWork.BackgroundJobs.GetByIdAsync(id);
    if (BackgroundJob == null)
    {
      return Result<object>.Failure(BackgroundJobErrors.NotFound);
    }

    await _unitOfWork.BackgroundJobs.RemoveAsync(BackgroundJob);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }

  public async Task EnqueueAsync<TPayload>(
    string jobType,
    TPayload payload
  )
    where TPayload : IBackgroundJobPayload
  {
    var json = JsonSerializer.Serialize(payload);

    await _unitOfWork.BackgroundJobs
      .AddAsync(
        new BackgroundJob
        {
          JobType = jobType,
          PayloadJson = json,
          Status = JobStatus.Pending
        }
      );
  }
}