using LapisApi.App.Settings.Dto;
using LapisApi.App.Settings.Dto.Commands;
using LapisApi.App.Settings.Dto.Response;
namespace LapisApi.App.Settings.Interfaces;

public interface ISettingService
{
    Task<Result<SettingResponse>> GetSettingsAsync();
    
    Task<Result<GetStatisticResponse>> GetGetStatisticAsync();
    
    Task<Result<SettingResponse>> UpdateSettingAsync(SettingUpdateCommand settingCommand);
    Task<Result<AdsClientResponse>> GetClientAdsAsync();
    Task<Result<AdsResponse>> GetAdsAsync();
    Task<Result<AdsResponse>> UpdateAdsAsync(AdsUpdateCommand command);
}