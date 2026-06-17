using AutoMapper;
using LinqKit;
using System.Linq.Expressions;
using TransfersApi.App.__Feature__s.Dto.Request.Commands;
using TransfersApi.App.__Feature__s.Dto.Request.Queries;
using TransfersApi.App.__Feature__s.Dto.Response;
using TransfersApi.App.__Feature__s.Enums;
using TransfersApi.App.__Feature__s.Errors;
using TransfersApi.App.__Feature__s.Interfaces;
using TransfersApi.App.__Feature__s.Model;
using TransfersApi.App.Auth.Interfaces;
using TransfersApi.App.Countries.Errors;
using TransfersApi.App.Users.Interfaces;
using TransfersApi.Data.Interfaces;
using TransfersApi.Helpers;
using TransfersApi.Helpers.Responses;

namespace TransfersApi.App.__Feature__s.Services;

public class __Feature__Service : I__Feature__Service
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUserService _userService;

  public __Feature__Service(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService,
    IUserService userService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
    _userService = userService;
  }

  public async Task<Result<__Feature__Response>> AddAsync(__Feature__CreateCommand command)
  {
    var country = await _unitOfWork.Countries.GetByIdAsync(command.CountryId);
    if (country == null)
    {
      return Result<__Feature__Response>.Failure(CountryErrors.NotFound);
    }

    var __Feature__Code =
      await _unitOfWork.__Feature__s
        .GetFirstOrDefaultAsync(
          c =>
            c.Code == command.Code
        );

    if (__Feature__Code != null)
    {
      return Result<__Feature__Response>.Failure(__Feature__Errors.AlreadyExists);
    }

    var __feature__ = _mapper.Map<__Feature__>(command);
    await _unitOfWork.__Feature__s.AddAsync(__feature__);

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<__Feature__Response>(__feature__);
    return Result<__Feature__Response>.Success(data);
  }

  public async Task<Result<__Feature__Response>> UpdateAsync(int id, __Feature__UpdateCommand command)
  {
    var __feature__ = await _unitOfWork.__Feature__s.GetByIdAsync(id);
    if (__feature__ == null)
    {
      return Result<__Feature__Response>.Failure(__Feature__Errors.NotFound);
    }

    _mapper.Map(command, __feature__);
    await _unitOfWork.__Feature__s.UpdateAsync(__feature__);
    await _unitOfWork.SaveChangesAsync();

    return Result<__Feature__Response>.Success(_mapper.Map<__Feature__Response>(__feature__));
  }

  public async Task<Result<IEnumerable<__Feature__Response>>> GetAllAsync(__Feature__GetAllQuery query)
  {
    Expression<Func<__Feature__, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.Note.Contains(query.Search)
      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }

    if (query.CountryId != null)
    {
      predicate = predicate.And(c => c.CountryId == query.CountryId);
    }

    var sortFunc = SortHelper.BuildSort<__Feature__, __Feature__SortFieldEnum>(query.Sort);

    var pagedResult = await _unitOfWork.__Feature__s.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      sort: sortFunc,
      queryBuilder:
      q => q
        .Include(c => c.Country)
    );

    var data = _mapper.Map<IEnumerable<__Feature__Response>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<__Feature__Response>>.Success(data, paging);
  }

  public async Task<Result<__Feature__Response>> GetByIdAsync(int id)
  {
    var __feature__ =
      await _unitOfWork.__Feature__s
        .GetFirstOrDefaultAsync(
          o => o.Id == id,
          queryBuilder:
          o => o.Include(o => o.Country)
        );

    if (__feature__ == null)
    {
      return Result<__Feature__Response>.Failure(__Feature__Errors.NotFound);
    }

    var data = _mapper.Map<__Feature__Response>(__feature__);
    return Result<__Feature__Response>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    var __feature__ = await _unitOfWork.__Feature__s.GetByIdAsync(id);
    if (__feature__ == null)
    {
      return Result<object>.Failure(__Feature__Errors.NotFound);
    }

    await _unitOfWork.__Feature__s.RemoveAsync(__feature__);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }
}