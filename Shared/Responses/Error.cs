using LapisApi.Helpers.Responses;

public class Error
{
  public string Code { get; }
  public string MessageAr { get; }
  public string MessageEn { get; }
  public ErrorType Type { get; }

  public Error(string code, string messageAr, string messageEn, ErrorType type)
  {
    Code = code;
    MessageAr = messageAr;
    MessageEn = messageEn;
    Type = type;
  }

  public static readonly Error None = new Error(
    code: "None",
    messageAr: string.Empty,
    messageEn: string.Empty,
    type: ErrorType.Validation
  );

  public string GetLocalizedMessage(string lang)
  {
    return lang?.ToLower() switch
    {
      "ar" => MessageAr,
      _ => MessageEn
    };
  }

}