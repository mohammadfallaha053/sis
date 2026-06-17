using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using LapisApi.OptionConfigurations;
namespace LapisApi.Shared.Providers;

// public class CustomEmailTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
// {
//   // public CustomEmailTokenProvider(
//   //   IDataProtectionProvider dataProtectionProvider,
//   //   IOptions<CustomEmailTokenProviderOptions> options,
//   //   ILogger<DataProtectorTokenProvider<TUser>> logger)
//   //   : base(dataProtectionProvider, options, logger)
//   // {
//   // }
// }