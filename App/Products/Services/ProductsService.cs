using AutoMapper;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.Categories.Errors;
using SisApi.App.MediaFiles.Enums;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.App.PointsTransactions.Enums;
using SisApi.App.PointsTransactions.Model;
using SisApi.App.Products.Dto.Request.Commands;
using SisApi.App.Products.Dto.Request.Queries;
using SisApi.App.Products.Dto.Response;
using SisApi.App.Products.Enums;
using SisApi.App.Products.Errors;
using SisApi.App.Products.Interfaces;
using SisApi.App.Products.Model;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace SisApi.Products.Services;

public class ProductsService : IProductsService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IFileService _fileService;

  public ProductsService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService,
    IFileService fileService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
    _fileService = fileService;
  }

  public async Task<Result<ProductsResponse>> AddAsync(
    ProductsCreateCommand command
  )
  {
    var category =
      await _unitOfWork.Categories.GetByIdAsync(command.CategoryId);

    if (category == null)
    {
      return Result<ProductsResponse>.Failure(
        CategoriesErrors.NotFound
      );
    }

    if (!category.IsActive)
    {
      return Result<ProductsResponse>.Failure(
        CategoriesErrors.Inactive
      );
    }

    await using var transaction = await _unitOfWork.BeginTransactionAsync();
    FileResponse? image = null;

    try
    {
      var model = _mapper.Map<Product>(command);
      model.Name = model.Name.Trim();

      model = await _unitOfWork.Products.AddAsync(model);

      await _unitOfWork.SaveChangesAsync();

      if (command.FileId.HasValue)
      {
        var fileResult =
          await _fileService.AttachFileAsync(
            fileId: command.FileId.Value,
            entityType: AttachmentEntityType.Product,
            entityId: model.Id.ToString()
          );

        if (!fileResult.IsSuccess)
        {
          await transaction.RollbackAsync();
          return Result<ProductsResponse>.Failure(fileResult.Error);
        }

        image = fileResult.Data;
      }

      await transaction.CommitAsync();

      model.Category = category;

      var data = _mapper.Map<ProductsResponse>(model);
      data.Image = image;

      return Result<ProductsResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<Result<ProductsResponse>> UpdateAsync(
    int id,
    ProductsUpdateCommand command
  )
  {
    var product =
      await _unitOfWork.Products.GetFirstOrDefaultAsync(
        predicate: product => product.Id == id,
        queryBuilder: products => products.Include(product => product.Category)
      );

    if (product == null)
    {
      return Result<ProductsResponse>.Failure(
        ProductsErrors.NotFound
      );
    }

    var category =
      await _unitOfWork.Categories.GetByIdAsync(command.CategoryId);

    if (category == null)
    {
      return Result<ProductsResponse>.Failure(
        CategoriesErrors.NotFound
      );
    }

    if (!category.IsActive)
    {
      return Result<ProductsResponse>.Failure(
        CategoriesErrors.Inactive
      );
    }

    _mapper.Map(command, product);
    product.Name = product.Name.Trim();
    product.Category = category;

    await _unitOfWork.Products.UpdateAsync(product);

    var fileResult =
      await _fileService.ProcessSingleFileUpdateAsync(
        newFileId: command.FileId,
        entityType: AttachmentEntityType.Product,
        entityId: product.Id.ToString()
      );

    if (!fileResult.IsSuccess)
    {
      return Result<ProductsResponse>.Failure(fileResult.Error);
    }

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<ProductsResponse>(product);
    data.Image = fileResult.Data;

    return Result<ProductsResponse>.Success(data);
  }

  public async Task<Result<IEnumerable<ProductsResponse>>> GetAllAsync(
    ProductsGetAllQuery query
  )
  {
    Expression<Func<Product, bool>> predicate =
      product =>
        string.IsNullOrEmpty(query.Search)
        ||
        product.Name.Contains(query.Search)
        ||
        (
          product.Description != null
          &&
          product.Description.Contains(query.Search)
        );

    if (query.IsActive.HasValue)
    {
      predicate =
        predicate.And(
          product => product.IsActive == query.IsActive.Value
        );
    }

    if (query.CategoryId.HasValue)
    {
      predicate =
        predicate.And(
          product => product.CategoryId == query.CategoryId.Value
        );
    }

    var sortFunc =
      SortHelper.BuildSort<Product, ProductsSortFieldEnum>(
        query.Sort
      );

    var pagedResult =
      await _unitOfWork.Products.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        sort: sortFunc,
        queryBuilder: products => products.Include(product => product.Category)
      );

    var products = pagedResult.Data.ToList();
    var data = _mapper.Map<List<ProductsResponse>>(products);

    var entityIds =
      products
        .Select(product => product.Id.ToString())
        .ToList();

    var imagesByEntityId =
      await _fileService.GetFirstFilesByEntitiesAsync(
        entityIds: entityIds,
        entityType: AttachmentEntityType.Product
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

    var paging = new AppPaging()
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<ProductsResponse>>.Success(
      data,
      paging
    );
  }

  public async Task<Result<ProductsResponse>> GetByIdAsync(int id)
  {
    var product =
      await _unitOfWork.Products.GetFirstOrDefaultAsync(
        predicate: product => product.Id == id,
        queryBuilder: products => products.Include(product => product.Category)
      );

    if (product == null)
    {
      return Result<ProductsResponse>.Failure(
        ProductsErrors.NotFound
      );
    }

    var data = _mapper.Map<ProductsResponse>(product);

    var file =
      await _fileService.GetFileByEntityAsync(
        entityId: product.Id.ToString(),
        entityType: AttachmentEntityType.Product
      );

    data.Image = file;

    return Result<ProductsResponse>.Success(data);
  }

  public async Task<Result<ProductPurchaseResponse>> PurchaseAsync(
    int id,
    ProductPurchaseCommand command
  )
  {
    var currentUserId = _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<ProductPurchaseResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user => user.Id == currentUserId
      );

    if (currentUser == null)
    {
      return Result<ProductPurchaseResponse>.Failure(
        UserErrors.NotFound
      );
    }

    if (
      currentUser.Role != RoleEnum.Client
      ||
      !currentUser.IsActive
    )
    {
      return Result<ProductPurchaseResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var product =
      await _unitOfWork.Products.GetFirstOrDefaultAsync(
        predicate: product => product.Id == id,
        queryBuilder: products => products.Include(product => product.Category)
      );

    if (product == null)
    {
      return Result<ProductPurchaseResponse>.Failure(
        ProductsErrors.NotFound
      );
    }

    if (!product.IsActive || !product.Category.IsActive)
    {
      return Result<ProductPurchaseResponse>.Failure(
        ProductsErrors.Inactive
      );
    }

    if (
      command.Quantity <= 0
      ||
      product.StockQuantity < command.Quantity
    )
    {
      return Result<ProductPurchaseResponse>.Failure(
        ProductsErrors.OutOfStock
      );
    }

    var totalPoints =
      product.PointsPrice * command.Quantity;

    if (currentUser.PointsBalance < totalPoints)
    {
      return Result<ProductPurchaseResponse>.Failure(
        ProductsErrors.InsufficientPoints
      );
    }

    await using var transaction =
      await _unitOfWork.BeginTransactionAsync();

    try
    {
      var balanceBefore = currentUser.PointsBalance;

      currentUser.PointsBalance -= totalPoints;
      product.StockQuantity -= command.Quantity;

      var pointsTransaction =
        new PointsTransaction
        {
          ClientId = currentUser.Id,
          Type = PointsTransactionTypeEnum.ProductPurchase,
          Points = totalPoints,
          BalanceBefore = balanceBefore,
          BalanceAfter = currentUser.PointsBalance,
          ProductId = product.Id,
          Quantity = command.Quantity,
          CreatedAt = DateTime.UtcNow
        };

      await _unitOfWork.Users.UpdateAsync(currentUser);
      await _unitOfWork.Products.UpdateAsync(product);
      await _unitOfWork.PointsTransactions.AddAsync(pointsTransaction);

      await _unitOfWork.SaveChangesAsync();

      await transaction.CommitAsync();

      var data =
        new ProductPurchaseResponse
        {
          ProductId = product.Id,
          ProductName = product.Name,
          Quantity = command.Quantity,
          PointsSpent = totalPoints,
          RemainingPoints = currentUser.PointsBalance,
          RemainingStock = product.StockQuantity
        };

      return Result<ProductPurchaseResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    try
    {
      var product = await _unitOfWork.Products.GetByIdAsync(id);

      if (product == null)
      {
        await transaction.RollbackAsync();
        return Result<object>.Failure(ProductsErrors.NotFound);
      }

      var files =
        await _fileService.GetFilesByEntityAsync(
          entityId: product.Id.ToString(),
          entityType: AttachmentEntityType.Product
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

      await _unitOfWork.Products.RemoveAsync(product);

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
