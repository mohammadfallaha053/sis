using LapisApi.Helpers.Responses;

namespace SisApi.App.PointsTransactions.Errors;

public static class PointsTransactionsErrors
{
  public static readonly Error InvalidDateRange = new(
    code: "PointsTransactions.InvalidDateRange",
    messageAr: "الفترة الزمنية المحددة غير صالحة",
    messageEn: "The selected date range is invalid",
    type: ErrorType.Validation
  );
}
