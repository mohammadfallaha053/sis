using AutoMapper;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers.Responses;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.PointsTransactions.Dto.Request.Queries;
using SisApi.App.PointsTransactions.Dto.Response;
using SisApi.App.PointsTransactions.Errors;
using SisApi.App.PointsTransactions.Interfaces;
using SisApi.App.PointsTransactions.Model;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace SisApi.PointsTransactions.Services;

public class PointsTransactionsService : IPointsTransactionsService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;

  public PointsTransactionsService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
  }

  public async Task<Result<IEnumerable<PointsTransactionResponse>>> GetAllAsync(
    PointsTransactionsGetAllQuery query
  )
  {
    if (
      query.FromDate.HasValue
      &&
      query.ToDate.HasValue
      &&
      query.FromDate.Value > query.ToDate.Value
    )
    {
      return Result<IEnumerable<PointsTransactionResponse>>.Failure(
        PointsTransactionsErrors.InvalidDateRange
      );
    }

    var currentUserId = _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<IEnumerable<PointsTransactionResponse>>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user => user.Id == currentUserId
      );

    if (currentUser == null)
    {
      return Result<IEnumerable<PointsTransactionResponse>>.Failure(
        UserErrors.NotFound
      );
    }

    Expression<Func<PointsTransaction, bool>> predicate =
      transaction => true;

    switch (currentUser.Role)
    {
      case RoleEnum.Admin:
        if (!string.IsNullOrWhiteSpace(query.ClientId))
        {
          predicate =
            predicate.And(
              transaction => transaction.ClientId == query.ClientId
            );
        }

        break;

      case RoleEnum.Client:
        predicate =
          predicate.And(
            transaction => transaction.ClientId == currentUser.Id
          );

        break;

      default:
        return Result<IEnumerable<PointsTransactionResponse>>.Failure(
          AuthErrors.Unauthorized
        );
    }

    if (query.Type.HasValue)
    {
      predicate =
        predicate.And(
          transaction => transaction.Type == query.Type.Value
        );
    }

    if (query.FromDate.HasValue)
    {
      predicate =
        predicate.And(
          transaction => transaction.CreatedAt >= query.FromDate.Value
        );
    }

    if (query.ToDate.HasValue)
    {
      predicate =
        predicate.And(
          transaction => transaction.CreatedAt <= query.ToDate.Value
        );
    }

    var pagedResult =
      await _unitOfWork.PointsTransactions.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        queryBuilder: transactions => transactions
          .Include(transaction => transaction.Product)
      );

    var data =
      _mapper.Map<List<PointsTransactionResponse>>(
        pagedResult.Data.ToList()
      );

    var paging = new AppPaging()
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<PointsTransactionResponse>>.Success(
      data,
      paging
    );
  }
}
