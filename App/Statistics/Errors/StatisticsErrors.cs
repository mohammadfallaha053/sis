using LapisApi.Helpers.Responses;
namespace SisApi.App.Statistics.Errors;

public static class StatisticsErrors
{
  public static readonly Error InvalidDateRange = new(
    code: "Statistics.InvalidDateRange",
    messageAr: "تاريخ البداية يجب أن يكون قبل تاريخ النهاية",
    messageEn: "From date must be before or equal to to date",
    type: ErrorType.Validation
  );
}
