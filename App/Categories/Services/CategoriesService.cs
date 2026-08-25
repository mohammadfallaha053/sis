using AutoMapper;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using SisApi.App.Categories.Dto.Request.Commands;
using SisApi.App.Categories.Dto.Request.Queries;
using SisApi.App.Categories.Dto.Response;
using SisApi.App.Categories.Enums;
using SisApi.App.Categories.Errors;
using SisApi.App.Categories.Interfaces;
using SisApi.App.Categories.Model;
using SisApi.App.MediaFiles.Enums;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;

namespace SisApi.App.Categories.Services;

public class CategoriesService : ICategoriesService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IFileService _fileService;

  public CategoriesService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileService fileService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _fileService = fileService;
  }

  public async Task<Result<CategoriesResponse>> AddAsync(
    CategoriesCreateCommand command
  )
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    FileResponse? image = null;

    try
    {
      var model = _mapper.Map<Category>(command);
      model.Name = model.Name.Trim();

      model = await _unitOfWork.Categories.AddAsync(model);

      await _unitOfWork.SaveChangesAsync();

      if (command.FileId.HasValue)
      {
        var fileResult =
          await _fileService.AttachFileAsync(
            fileId: command.FileId.Value,
            entityType: AttachmentEntityType.Category,
            entityId: model.Id.ToString()
          );

        if (!fileResult.IsSuccess)
        {
          await transaction.RollbackAsync();
          return Result<CategoriesResponse>.Failure(fileResult.Error);
        }

        image = fileResult.Data;
      }

      await transaction.CommitAsync();

      var data = _mapper.Map<CategoriesResponse>(model);
      data.Image = image;

      return Result<CategoriesResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<Result<CategoriesResponse>> UpdateAsync(
    int id,
    CategoriesUpdateCommand command
  )
  {
    var category = await _unitOfWork.Categories.GetByIdAsync(id);

    if (category == null)
    {
      return Result<CategoriesResponse>.Failure(
        CategoriesErrors.NotFound
      );
    }

    _mapper.Map(command, category);
    category.Name = category.Name.Trim();

    await _unitOfWork.Categories.UpdateAsync(category);

    var fileResult =
      await _fileService.ProcessSingleFileUpdateAsync(
        newFileId: command.FileId,
        entityType: AttachmentEntityType.Category,
        entityId: category.Id.ToString()
      );

    if (!fileResult.IsSuccess)
    {
      return Result<CategoriesResponse>.Failure(fileResult.Error);
    }

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<CategoriesResponse>(category);
    data.Image = fileResult.Data;

    return Result<CategoriesResponse>.Success(data);
  }

  public async Task<Result<IEnumerable<CategoriesResponse>>> GetAllAsync(
    CategoriesGetAllQuery query
  )
  {
    Expression<Func<Category, bool>> predicate =
      category =>
        string.IsNullOrEmpty(query.Search)
        ||
        category.Name.Contains(query.Search);

    if (query.IsActive.HasValue)
    {
      predicate =
        predicate.And(
          category => category.IsActive == query.IsActive.Value
        );
    }

    var sortFunc =
      SortHelper.BuildSort<Category, CategoriesSortFieldEnum>(
        query.Sort
      );

    var pagedResult =
      await _unitOfWork.Categories.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        sort: sortFunc
      );

    var categories = pagedResult.Data.ToList();
    var data = _mapper.Map<List<CategoriesResponse>>(categories);

    var entityIds =
      categories
        .Select(category => category.Id.ToString())
        .ToList();

    var imagesByEntityId =
      await _fileService.GetFirstFilesByEntitiesAsync(
        entityIds: entityIds,
        entityType: AttachmentEntityType.Category
      );

    foreach (var item in data)
    {
      if (
        imagesByEntityId.TryGetValue(
          item.Id.ToString(),
          out var image
        )
      )
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

    return Result<IEnumerable<CategoriesResponse>>.Success(
      data,
      paging
    );
  }

  public async Task<Result<CategoriesResponse>> GetByIdAsync(int id)
  {
    var category =
      await _unitOfWork.Categories.GetFirstOrDefaultAsync(
        category => category.Id == id
      );

    if (category == null)
    {
      return Result<CategoriesResponse>.Failure(
        CategoriesErrors.NotFound
      );
    }

    var data = _mapper.Map<CategoriesResponse>(category);

    var file =
      await _fileService.GetFileByEntityAsync(
        entityId: category.Id.ToString(),
        entityType: AttachmentEntityType.Category
      );

    data.Image = file;

    return Result<CategoriesResponse>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    try
    {
      var category = await _unitOfWork.Categories.GetByIdAsync(id);

      if (category == null)
      {
        await transaction.RollbackAsync();
        return Result<object>.Failure(CategoriesErrors.NotFound);
      }

      var files =
        await _fileService.GetFilesByEntityAsync(
          entityId: category.Id.ToString(),
          entityType: AttachmentEntityType.Category
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

      await _unitOfWork.Categories.RemoveAsync(category);

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
