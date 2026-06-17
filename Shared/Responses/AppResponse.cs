namespace LapisApi.Helpers.Responses;

public class AppResponse<T>
{
  public bool IsSuccess { get; set; }
  public T? Data { get; set; }
  public ErrorDetails? Error { get; set; }
  public AppPaging? Paging { get; set; }

  public static AppResponse<T> Success(T data, AppPaging? paging = null)
  {
    return new AppResponse<T>
    {
      IsSuccess = true,
      Data = data,
      Paging = paging
    };
  }

  public static AppResponse<T> Failure(ErrorDetails error)
  {
    return new AppResponse<T>
    {
      IsSuccess = false,
      Error = error
    };
  }
}

public class ErrorDetails
{
  public string? Code { get; set; }
  public string? Message { get; set; }
  
  public string? Details { get; set; }
  
  public static ErrorDetails? FromError(Error? error, AppLanguageEnum languageEnum)
  {
    if (error == null) return null;

    return new ErrorDetails
    {
      Code = error.Code,
      Message = languageEnum == AppLanguageEnum.Ar ? error.MessageAr : error.MessageEn
    };
  }
}

public class AppPaging
{
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalRecords { get; set; }
}