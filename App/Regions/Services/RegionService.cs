using AutoMapper;
using LapisApi.App.Regions.Dto;
using LapisApi.App.Regions.Enums;
using LapisApi.App.Regions.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Regions.Dto.Request.Commands;
using SisApi.App.Regions.Dto.Request.Queries;
using SisApi.App.Regions.Dto.Response;
using SisApi.App.Regions.Interfaces;
using SisApi.App.Regions.Model;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace SisApi.App.Regions.Services;

public class RegionService : IRegionService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;

  public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
  }

  public async Task<Result<RegionResponse>> AddAsync(RegionCreateCommand command)
  {
    var Region = _mapper.Map<Region>(command);

    await _unitOfWork.Regions.AddAsync(Region);
    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<RegionResponse>(Region);
    return Result<RegionResponse>.Success(data);
  }



  public async Task<Result<IEnumerable<RegionResponse>>> GetAllAsync(RegionGetAllQuery query)
  {
    Expression<Func<Region, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }

    if (query.CenterId != null)
    {
      predicate = predicate.And(c => c.CenterId == query.CenterId);
    }

    var sortFunc = SortHelper.BuildSort<Region, RegionSortFieldEnum>(query.Sort);

    var pagedResult = await _unitOfWork.Regions.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      sort: sortFunc,
      queryBuilder: o => o.Include(o => o.Center)
    );

    var data = _mapper.Map<IEnumerable<RegionResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<RegionResponse>>.Success(data, paging);
  }
  

  public async Task<Result<RegionResponse>> GetByIdAsync(int id)
  {
    var Region = await _unitOfWork.Regions.GetByIdAsync(id);
    if (Region == null)
    {
      return Result<RegionResponse>.Failure(RegionErrors.NotFound);
    }

    var data = _mapper.Map<RegionResponse>(Region);
    return Result<RegionResponse>.Success(data);
  }

  public async Task<Result<RegionResponse>> UpdateAsync(int id, RegionUpdateCommand command)
  {
    var Region = await _unitOfWork.Regions.GetByIdAsync(id);
    if (Region == null)
    {
      return Result<RegionResponse>.Failure(RegionErrors.NotFound);
    }

    _mapper.Map(command, Region);
    await _unitOfWork.Regions.UpdateAsync(Region);
    await _unitOfWork.SaveChangesAsync();

    return Result<RegionResponse>.Success(_mapper.Map<RegionResponse>(Region));
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    var Region = await _unitOfWork.Regions.GetByIdAsync(id);
    if (Region == null)
    {
      return Result<object>.Failure(RegionErrors.NotFound);
    }

    await _unitOfWork.Regions.RemoveAsync(Region);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }
}