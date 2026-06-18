using AutoMapper;
using LapisApi.App.Cities.Dto;
using LapisApi.App.Cities.Errors;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LapisApi.Interfaces.Cities;
using LapisApi.MyEnum.RegionSort;
using LinqKit;
using SisApi.App.Cities.Dto.Request.Commands;
using SisApi.App.Cities.Dto.Request.Queries;
using SisApi.App.Cities.Model;
using System.Linq.Expressions;
namespace SisApi.App.Cities.Services;

public class CityService : ICityService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;

  public CityService(IUnitOfWork unitOfWork, IMapper mapper)
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
  }

  public async Task<Result<CityResponse>> AddCityAsync(CityCreateCommand dto)
  {
    var City = _mapper.Map<City>(dto);

    await _unitOfWork.Cities.AddAsync(City);
    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<CityResponse>(City);
    return Result<CityResponse>.Success(data);
  }

  public async Task<Result<IEnumerable<CityResponse>>> GetAllCitiesAsync(CityGetAllQuery query)
  {
    Expression<Func<City, bool>> predicate = c =>
      (string.IsNullOrEmpty(query.Search) ||
       c.NameAr.Contains(query.Search) ||
       c.NameEn.ToLower().Contains(query.Search.ToLower())
      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }
    
    var sortFunc = SortHelper.BuildSort<City, CitySortField>(query.Sort);

    var pagedResult =
      await _unitOfWork.Cities.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        sort: sortFunc
      );

    var data = _mapper.Map<IEnumerable<CityResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<CityResponse>>.Success(data, paging);
  }

  public async Task<Result<IEnumerable<CityAutoCompleteResponse>>> GetAutoComplete(
    CityGetAutoCompleteQuery query
  )
  {
    Expression<Func<City, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.NameAr.Contains(query.Search)
        ||
        c.NameEn.ToLower().Contains(query.Search.ToLower())
      );

    predicate = predicate.And(c => c.IsActive);

    var pagedResult = await _unitOfWork.Cities.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize
    );

    var data = _mapper.Map<IEnumerable<CityAutoCompleteResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<CityAutoCompleteResponse>>.Success(data, paging);
  }

  public async Task<Result<CityResponse>> GetCityByIdAsync(int id)
  {
    var City = await _unitOfWork.Cities.GetByIdAsync(id);
    if (City == null)
    {
      return Result<CityResponse>.Failure(CityErrors.NotFound);
    }

    var data = _mapper.Map<CityResponse>(City);
    return Result<CityResponse>.Success(data);
  }

  public async Task<Result<CityResponse>> UpdateCityAsync(int id, UpdateCityCommand CityCommand)
  {
    var City = await _unitOfWork.Cities.GetByIdAsync(id);
    if (City == null)
    {
      return Result<CityResponse>.Failure(CityErrors.NotFound);
    }

    _mapper.Map(CityCommand, City);
    await _unitOfWork.Cities.UpdateAsync(City);
    await _unitOfWork.SaveChangesAsync();

    return Result<CityResponse>.Success(_mapper.Map<CityResponse>(City));
  }

  public async Task<Result<object>> DeleteCityAsync(int id)
  {
    var City = await _unitOfWork.Cities.GetByIdAsync(id);
    if (City == null)
    {
      return Result<object>.Failure(CityErrors.NotFound);
    }

    await _unitOfWork.Cities.RemoveAsync(City);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }
}