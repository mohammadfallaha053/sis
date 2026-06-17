using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LapisApi.App.Auth.Enums;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.App.Settings.Dto;
using LapisApi.App.Settings.Dto.Commands;
using LapisApi.App.Settings.Dto.Response;
using LapisApi.App.Settings.Errors;
using LapisApi.App.Settings.Interfaces;
using LapisApi.Data.Interfaces;
using LapisApi.OptionConfigurations;
namespace LapisApi.App.Settings.Services;

public class SettingService : ISettingService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IFileService _fileService;
  private readonly IMemoryCache _cache;
  private readonly IOptions<FrontendSettings> _frontendOptions;

  public SettingService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileService fileService,
    IOptions<FrontendSettings> frontendOptions,
    IMemoryCache cache
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _fileService = fileService;
    _frontendOptions = frontendOptions;
    _cache = cache;
  }
  public async Task<Result<SettingResponse>> GetSettingsAsync()
  {
    var setting = await _unitOfWork.Settings.GetFirstOrDefaultAsync(o => o.Id == 1);
    if (setting == null)
    {
      return Result<SettingResponse>.Failure(SettingErrors.NotFound);
    }

    var data = _mapper.Map<SettingResponse>(setting);

    return Result<SettingResponse>.Success(data);
  }
  public async Task<Result<GetStatisticResponse>> GetGetStatisticAsync()
  {
    const string cacheKey = "dashboard_statistics";

    if (_cache.TryGetValue(cacheKey, out GetStatisticResponse cachedStats))
    {
      return Result<GetStatisticResponse>.Success(cachedStats);
    }

    // تنفيذ الحسابات الثقيلة
    var totalCenters = await _unitOfWork.Centers.CountAsync();
    var activeCenters = await _unitOfWork.Centers.CountAsync(c => c.IsActive);
    var inactiveCenters = totalCenters - activeCenters;

    var totalRegions = await _unitOfWork.Regions.CountAsync();
    var totalCities = await _unitOfWork.Cities.CountAsync();

    var totalUsers =
      await _unitOfWork.Users.CountAsync(
        u => u.Role == RoleEnum.Client
      );
    var activeUsers =
      await _unitOfWork.Users.CountAsync(
        u => u.IsActive && u.Role == RoleEnum.Client
      );
    var inactiveUsers = totalUsers - activeUsers;
    

    var response = new GetStatisticResponse
    {
      TotalCenters = totalCenters,
      ActiveCenters = activeCenters,
      InactiveCenters = inactiveCenters,

      TotalRegions = totalRegions,
      TotalCities = totalCities,

      TotalUsers = totalUsers,
      ActiveUsers = activeUsers,
      InactiveUsers = inactiveUsers,
      
    };

    // حفظ في الكاش لمدة دقيقة
    _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));

    return Result<GetStatisticResponse>.Success(response);
  }


  public async Task<Result<SettingResponse>> UpdateSettingAsync(SettingUpdateCommand command)
  {
    var setting = await _unitOfWork.Settings.GetFirstOrDefaultAsync(o => o.Id == 1);
    if (setting == null)
    {
      return Result<SettingResponse>.Failure(SettingErrors.NotFound);
    }

    _mapper.Map(command, setting);
    await _unitOfWork.Settings.UpdateAsync(setting);
    await _unitOfWork.SaveChangesAsync();

    return Result<SettingResponse>.Success(_mapper.Map<SettingResponse>(setting));
  }
  public async Task<Result<AdsClientResponse>> GetClientAdsAsync()
  {
    var setting = await _unitOfWork.Settings.GetFirstOrDefaultAsync(o => o.Id == 1);
    if (setting == null)
    {
      return Result<AdsClientResponse>.Failure(SettingErrors.NotFound);
    }

    var data = _mapper.Map<AdsClientResponse>(setting);

    if (!setting.isExternal)
    {
      var files = await _fileService.GetFilesByEntityAsync(
        entityId: setting.Id.ToString(),
        entityType: AttachmentEntityType.Ads
      );

      var baseUrl = _frontendOptions.Value.BaseUrl?.TrimEnd('/');
      var filePath = files.FirstOrDefault()?.FilePath?.TrimStart('/');

      if (baseUrl != null && filePath != null)
      {
        data.Url = $"{baseUrl}/{filePath}";
      }
    }

    return Result<AdsClientResponse>.Success(data);
  }


  public async Task<Result<AdsResponse>> GetAdsAsync()
  {
    var setting = await _unitOfWork.Settings.GetFirstOrDefaultAsync(o => o.Id == 1);
    if (setting == null)
    {
      return Result<AdsResponse>.Failure(SettingErrors.NotFound);
    }

    var data = _mapper.Map<AdsResponse>(setting);


    var files =
      await _fileService.GetFilesByEntityAsync(
        entityId: setting.Id.ToString(),
        entityType: AttachmentEntityType.Ads
      );

    data.Video = files.FirstOrDefault();

    return Result<AdsResponse>.Success(data);
  }
  
  public async Task<Result<AdsResponse>> UpdateAdsAsync(AdsUpdateCommand command)
  {
    var setting = await _unitOfWork.Settings.GetFirstOrDefaultAsync(o => o.Id == 1);
    if (setting == null)
    {
      return Result<AdsResponse>.Failure(SettingErrors.NotFound);
    }

    var oldFileIds =
      await _fileService.GetFilesByEntityAsync(
        entityId: setting.Id.ToString(),
        entityType: AttachmentEntityType.Ads
      );

    int? oldFileId = oldFileIds.Count == 0 ? null : oldFileIds.FirstOrDefault().Id;

    var fileResult = await _fileService.ProcessFileUpdateAsync(
      newFileId: command.FileId,
      oldFileId: oldFileId,
      entityType: AttachmentEntityType.Ads,
      entityId: setting.Id.ToString()
    );

    if (!fileResult.IsSuccess)
    {
      return Result<AdsResponse>.Failure(error: fileResult.Error);
    }

    _mapper.Map(command, setting);

    await _unitOfWork.Settings.UpdateAsync(setting);
    await _unitOfWork.SaveChangesAsync();

    var data = _mapper.Map<AdsResponse>(setting);

    var files =
      await _fileService.GetFilesByEntityAsync(
        entityId: setting.Id.ToString(),
        entityType: AttachmentEntityType.Ads
      );

    data.Video = files.FirstOrDefault();

    return Result<AdsResponse>.Success(data);
  }
}