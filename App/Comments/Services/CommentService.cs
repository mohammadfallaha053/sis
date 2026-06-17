using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Comments.Dto;
using LapisApi.App.Comments.Dto.Request.Commands;
using LapisApi.App.Comments.Dto.Request.Queries;
using LapisApi.App.Comments.Dto.Response;
using LapisApi.App.Comments.Enums;
using LapisApi.App.Comments.Errors;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Comments.Model;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.App.Users.Dto.Response;
using LapisApi.App.Users.Interfaces;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LapisApi.Interfaces.Auth;
namespace LapisApi.App.Comments.Services;

public class CommentService : ICommentService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;

  public CommentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService,
    IUserService userService,
    IFileService fileService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
    _userService = userService;
    _fileService = fileService;
  }

  public async Task<Result<CommentResponse>> AddAsync(CommentCreateCommand command)
  {

    var userId = _claimService.GetUserId();

    var comment = _mapper.Map<Comment>(command);
    comment.UserId = userId;

    await _unitOfWork.Comments.AddAsync(comment);

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<CommentResponse>(comment);
    return Result<CommentResponse>.Success(data);
  }

  public async Task<Result<CommentResponse>> UpdateAsync(int id, CommentUpdateCommand command)
  {
    var comment = await _unitOfWork.Comments.GetByIdAsync(id);
    if (comment == null)
    {
      return Result<CommentResponse>.Failure(CommentErrors.NotFound);
    }

    _mapper.Map(command, comment);
    await _unitOfWork.Comments.UpdateAsync(comment);
    await _unitOfWork.SaveChangesAsync();

    return Result<CommentResponse>.Success(_mapper.Map<CommentResponse>(comment));
  }

  public async Task<Result<IEnumerable<CommentResponse>>> GetAllAsync(CommentGetAllQuery query)
  {
    Expression<Func<Comment, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.Text.Contains(query.Search)
      );

    if (query.IsAccepted != null)
    {
      predicate = predicate.And(c => c.IsAccepted == query.IsAccepted);
    }

    var sortFunc = SortHelper.BuildSort<Comment, CommentSortFieldEnum>(query.Sort);

    var pagedResult = await _unitOfWork.Comments.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      sort: sortFunc,
      queryBuilder:
      q => q
        .Include(c => c.User)
    );

    var data = _mapper.Map<IEnumerable<CommentResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<CommentResponse>>.Success(data, paging);
  }
  public async Task<Result<IEnumerable<CommentSliderResponse>>> GetSlider()
  {
    // 1. جلب أول 10 تعليقات مقبولة مع المستخدم
    var pagedResult = await _unitOfWork.Comments.GetPagedAsync(
      o => o.IsAccepted == true,
      pageNumber: 1,
      pageSize: 10,
      queryBuilder: q => q.Include(c => c.User)
    );

    var comments = pagedResult.Data;

    // 2. تجهيز القائمة النهائية يدويًا بدون AutoMapper
    var result = new List<CommentSliderResponse>();

    foreach (var comment in comments)
    {
      var files = await _fileService.GetFilesByEntityAsync(
        comment.UserId,
        AttachmentEntityType.User
      );

      var userDto = new UserBaseResponse
      {
        Id = comment.UserId,
        Email = comment.User?.Email ?? string.Empty,
        FirstName = comment.User?.FirstName ?? string.Empty,
        LastName = comment.User?.LastName ?? string.Empty,
        Image = files.FirstOrDefault(),
        City = comment.User?.City
      };

      result.Add(
        new CommentSliderResponse
        {
          Text = comment.Text,
          CreatedAt = comment.CreatedAt,
          User = userDto
        }
      );
    }

    return Result<IEnumerable<CommentSliderResponse>>.Success(result);
  }


  public async Task<Result<CommentResponse>> GetByIdAsync(int id)
  {
    var comment =
      await _unitOfWork.Comments
        .GetFirstOrDefaultAsync(
          o => o.Id == id,
          queryBuilder:
          o => o.Include(o => o.User)
        );

    if (comment == null)
    {
      return Result<CommentResponse>.Failure(CommentErrors.NotFound);
    }

    var data = _mapper.Map<CommentResponse>(comment);
    return Result<CommentResponse>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    var comment = await _unitOfWork.Comments.GetByIdAsync(id);
    if (comment == null)
    {
      return Result<object>.Failure(CommentErrors.NotFound);
    }

    await _unitOfWork.Comments.RemoveAsync(comment);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }
}