using AutoMapper;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.ItemTypes.Dto.Request.Commands;
using LapisApi.App.ItemTypes.Enums;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using SisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Request.Queries;
using SisApi.App.ItemTypes.Dto.Response;
using SisApi.App.ItemTypes.Errors;
using SisApi.App.ItemTypes.Interfaces;
using SisApi.App.ItemTypes.Model;
using SisApi.App.MediaFiles.Enums;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.App.Users.Interfaces;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace LapisApi.App.ItemTypes.Services;

public class ItemTypesService : IItemTypesService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;

  public ItemTypesService(
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

  public async Task<Result<ItemTypesResponse>> AddAsync(ItemTypesCreateCommand command)
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    FileResponse? image = null;
    try
    {
      var model = _mapper.Map<ItemType>(command);

      model = await _unitOfWork.ItemTypes.AddAsync(model);

      await _unitOfWork.SaveChangesAsync();

      if (command.FileId.HasValue)
      {
        var fileResult =
          await _fileService.AttachFileAsync(
            fileId: command.FileId.Value,
            entityType: AttachmentEntityType.ItemType,
            entityId: model.Id.ToString()
          );

        if (!fileResult.IsSuccess)
        {
          await transaction.RollbackAsync();
          return Result<ItemTypesResponse>.Failure(error: fileResult.Error);
        }
        image = fileResult.Data;
      }
      
      await transaction.CommitAsync();
      var data = _mapper.Map<ItemTypesResponse>(model);
      data.Image = image;

      return Result<ItemTypesResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  public async Task<Result<ItemTypesResponse>> UpdateAsync(
    int id,
    ItemTypesUpdateCommand command
  )
  {
    var ItemTypes =
      await _unitOfWork.ItemTypes.GetByIdAsync(id);

    if (ItemTypes == null)
    {
      return Result<ItemTypesResponse>.Failure(
        ItemTypesErrors.NotFound
      );
    }

    _mapper.Map(command, ItemTypes);

    await _unitOfWork.ItemTypes.UpdateAsync(ItemTypes);

    var fileResult =
      await _fileService.ProcessSingleFileUpdateAsync(
        newFileId: command.FileId,
        entityType: AttachmentEntityType.ItemType,
        entityId: ItemTypes.Id.ToString()
      );

    if (!fileResult.IsSuccess)
    {
      return Result<ItemTypesResponse>.Failure(
        error: fileResult.Error
      );
    }

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<ItemTypesResponse>(ItemTypes);

    data.Image = fileResult.Data;

    return Result<ItemTypesResponse>.Success(data);
  }
  public async Task<Result<IEnumerable<ItemTypesResponse>>> GetAllAsync(
    ItemTypesGetAllQuery query
  )
  {
    Expression<Func<ItemType, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.Name.Contains(query.Search)

      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }

    var sortFunc =
      SortHelper.BuildSort<ItemType, ItemTypesSortFieldEnum>(
        query.Sort
      );

    var pagedResult =
      await _unitOfWork.ItemTypes.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        sort: sortFunc
      );

    var specialists = pagedResult.Data.ToList();

    var data = _mapper.Map<List<ItemTypesResponse>>(specialists);

    var entityIds =
      specialists
        .Select(s => s.Id.ToString())
        .ToList();

    var imagesByEntityId =
      await _fileService.GetFirstFilesByEntitiesAsync(
        entityIds: entityIds,
        entityType: AttachmentEntityType.ItemType
      );

    foreach (var item in data)
    {
      if (imagesByEntityId.TryGetValue(item.Id.ToString(), out var image))
      {
        item.Image = image;
      }
    }

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<ItemTypesResponse>>.Success(data, paging);
  }
  public async Task<Result<ItemTypesResponse>> GetByIdAsync(int id)
  {
    var ItemTypes =
      await _unitOfWork.ItemTypes
        .GetFirstOrDefaultAsync(
          o => o.Id == id
          // queryBuilder:
          // o => o.Include(o => o.Country)
        );

    if (ItemTypes == null)
    {
      return Result<ItemTypesResponse>.Failure(ItemTypesErrors.NotFound);
    }

    var data = _mapper.Map<ItemTypesResponse>(ItemTypes);

    var file =
      await _fileService.GetFileByEntityAsync(
        entityId: ItemTypes.Id.ToString(),
        entityType: AttachmentEntityType.ItemType
      );

    if (file != null)
    {
      data.Image = file;
    }

    return Result<ItemTypesResponse>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    try
    {
      var ItemTypes =
        await _unitOfWork.ItemTypes.GetByIdAsync(id);

      if (ItemTypes == null)
      {
        await transaction.RollbackAsync();
        return Result<object>.Failure(ItemTypesErrors.NotFound);
      }

      var files =
        await _fileService.GetFilesByEntityAsync(
          entityId: ItemTypes.Id.ToString(),
          entityType: AttachmentEntityType.ItemType
        );

      foreach (var file in files)
      {
        var deleteFileResult =
          await _fileService.DeleteFileAsync(file.Id);

        if (!deleteFileResult.IsSuccess)
        {
          await transaction.RollbackAsync();
          return Result<object>.Failure(deleteFileResult.Error);
        }
      }

      await _unitOfWork.ItemTypes.RemoveAsync(ItemTypes);

      await _unitOfWork.SaveChangesAsync();

      await transaction.CommitAsync();

      return Result<object>.Success(null);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
}