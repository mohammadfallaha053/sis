namespace LapisApi.Helpers;

public class EnumHelper
{
  public static bool IsValueInEnum<T>(
    string value
  ) where T : Enum
  {
    return
      Enum
        .GetNames(
          typeof(T)
        )
        .Contains(
          value
        );
  }
}