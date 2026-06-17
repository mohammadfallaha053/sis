using LapisApi.Helpers.Responses;

public class Result<T>
{
  public bool IsSuccess { get; }
  public T? Data { get; }
  
  public AppPaging? Paging { get; }
  public Error Error { get; }

  private Result(bool isSuccess, T? data, Error error, AppPaging? paging)
  {
    IsSuccess = isSuccess;
    Data = data;
    Error = error;
    Paging = paging;
  }

  public static Result<T> Success(T data) =>
    new Result<T>(true, data, Error.None, null);
  public static Result<T> Success(T data, AppPaging paging) =>
    new Result<T>(true, data, Error.None, paging);

  public static Result<T> Failure(Error error) =>
    new Result<T>(false, default, error, null);
}