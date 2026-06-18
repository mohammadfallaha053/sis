using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Centers.Dto;
using LapisApi.App.Centers.Dto.Request.Commands;
using LapisApi.App.Centers.Dto.Response;
using LapisApi.App.Centers.Enums;
using LapisApi.App.Centers.Errors;
using LapisApi.App.Centers.Interfaces;
using LapisApi.App.Regions.Errors;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.App.Users.Dto;
using LapisApi.App.Users.Interfaces;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Request.Queries;
using SisApi.App.Centers.Dto.Response;
using SisApi.App.Centers.Model;
namespace LapisApi.App.Centers.Services;

public class CenterService : ICenterService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;
  public CenterService(
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

  public async Task<Result<CenterResponse>> AddCenterAsync(CenterCreateCommand command)
  {
    var Region = await _unitOfWork.Regions.GetByIdAsync(command.RegionId);
    if (Region == null)
    {
      return Result<CenterResponse>.Failure(RegionErrors.NotFound);
    }

    var centerName =
      await _unitOfWork.Centers
        .GetFirstOrDefaultAsync(
          c =>
            c.NameAr == command.NameAr
            ||
            c.NameEn == command.NameEn
        );

    if (centerName != null)
    {
      return Result<CenterResponse>.Failure(CenterErrors.AlreadyExists);
    }

    var center = _mapper.Map<Center>(command);
    await _unitOfWork.Centers.AddAsync(center);
    

    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<CenterResponse>(center);
    return Result<CenterResponse>.Success(data);
  }

  public async Task<Result<IEnumerable<CenterResponse>>> GetAllCentersAsync(CenterGetAllQuery query)
  {
    Expression<Func<Center, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
        ||
        c.NameAr.Contains(query.Search)
        ||
        c.NameEn.ToLower().Contains(query.Search.ToLower())
      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }
    
    if (query.RegionId != null)
    {
      predicate = predicate.And(c => c.RegionId == query.RegionId);
    }

    if (query.CityId != null)
    {
      predicate = predicate.And(c => c.Region.CityId == query.CityId);
    }

    var sortFunc = SortHelper.BuildSort<Center, CenterSortFieldEnum>(query.Sort);

    var pagedResult = await _unitOfWork.Centers.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      sort: sortFunc,
      queryBuilder:
      q => q
        .Include(c => c.Region)
        .ThenInclude(Region => Region.City)
        .Include(c => c.Manager)
    );
    
    
    var data = _mapper.Map<IEnumerable<CenterResponse>>(pagedResult.Data);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<CenterResponse>>.Success(data, paging);
  }


  public async Task<Result<IEnumerable<CenterGetForClientResponse>>> GetCentersAsync(CenterGetForClientQuery query)
  {
    Expression<Func<Center, bool>> predicate = c => c.IsActive;

    if (query.RegionId != null)
    {
      predicate = predicate.And(c => c.RegionId == query.RegionId);
    }

    if (query.CityId != null)
    {
      predicate = predicate.And(c => c.Region.CityId == query.CityId);
    }

    var centers = await _unitOfWork.Centers.GetAllAsync(
      predicate: predicate,
      queryBuilder:
      q => q
        .Include(c => c.Region)
        .ThenInclude(Region => Region.City)
    );

    var data = _mapper.Map<List<CenterGetForClientResponse>>(centers);

    await FileMergerHelper
      .AddEntityImagesAsync(
        dtos: data,
        entities: centers,
        getEntityId: c => c.Id,
        setImageUrl: (dto, url) => dto.ImageUrl = url,
        fileService: _fileService,
        entityType: AttachmentEntityType.Center
      );

    return Result<IEnumerable<CenterGetForClientResponse>>.Success(data);
  }


  public async Task<Result<CenterResponse>> GetCenterByIdAsync(string id)
  {
    var center =
      await _unitOfWork.Centers
        .GetFirstOrDefaultAsync(
          o => o.Id == id,
          queryBuilder:
          o => o.Include(o => o.Region).ThenInclude(Region => Region.City)
        );

    if (center == null)
    {
      return Result<CenterResponse>.Failure(CenterErrors.NotFound);
    }

    var mediaFiles = await _fileService.GetFilesByEntityAsync(
      entityId: id,
      entityType: AttachmentEntityType.Center
    );

    var data = _mapper.Map<CenterResponse>(center);
    data.Image = mediaFiles.FirstOrDefault();

    return Result<CenterResponse>.Success(data);
  }

  public async Task<Result<CenterInfoResponse>> GetCenterInfo()
  {
    var centerId = _claimService.GetCenterId();
    if (centerId == null)
    {
      return Result<CenterInfoResponse>.Failure(CenterErrors.NotFound);
    }

    var center =
      await _unitOfWork.Centers
        .GetFirstOrDefaultAsync(
          o => o.Id == centerId,
          queryBuilder:
          o => o.Include(o => o.Region).ThenInclude(Region => Region.City)
        );

    if (center == null)
    {
      return Result<CenterInfoResponse>.Failure(CenterErrors.NotFound);
    }

    var data = _mapper.Map<CenterInfoResponse>(center);

    var files =
      await _fileService.GetFilesByEntityAsync(
        centerId,
        AttachmentEntityType.Center
      );

    data.Image = files.FirstOrDefault();
    return Result<CenterInfoResponse>.Success(data);
  }

  public async Task<Result<CenterResponse>> UpdateCenterAsync(string id, CenterUpdateCommand request)
  {
    var center = await _unitOfWork.Centers.GetByIdAsync(id);
    if (center == null)
    {
      return Result<CenterResponse>.Failure(CenterErrors.NotFound);
    }

    _mapper.Map(request, center);
    await _unitOfWork.Centers.UpdateAsync(center);
    await _unitOfWork.SaveChangesAsync();

    return Result<CenterResponse>.Success(_mapper.Map<CenterResponse>(center));
  }

  public async Task<Result<CenterResponse>> UpdateCenterInfoAsync(string id,CenterUpdateInfoCommand command)
  {
    var center = await _unitOfWork.Centers.GetByIdAsync(id);
    if (center == null)
    {
      return Result<CenterResponse>.Failure(CenterErrors.NotFound);
    }

    var oldFileIds =
      await _fileService.GetFilesByEntityAsync(
        entityId: center.Id,
        entityType: AttachmentEntityType.Center
      );

    int? oldFileId = oldFileIds.Count == 0 ? null : oldFileIds.FirstOrDefault().Id;

    var fileResult = await _fileService.ProcessFileUpdateAsync(
      newFileId: command.FileId,
      oldFileId: oldFileId,
      entityType: AttachmentEntityType.Center,
      entityId: center.Id
    );

    if (!fileResult.IsSuccess)
    {
      return Result<CenterResponse>.Failure(error: fileResult.Error);
    }

    _mapper.Map(command, center);
    await _unitOfWork.Centers.UpdateAsync(center);


    await _unitOfWork.SaveChangesAsync();

    return Result<CenterResponse>.Success(_mapper.Map<CenterResponse>(center));
  }


  public async Task<Result<object>> DeleteCenterAsync(string id)
  {
    var center = await _unitOfWork.Centers.GetByIdAsync(id);
    if (center == null)
    {
      return Result<object>.Failure(CenterErrors.NotFound);
    }

    await _unitOfWork.Centers.RemoveAsync(center);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }
}