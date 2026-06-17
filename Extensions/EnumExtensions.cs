using System.ComponentModel;
using LapisApi.Shared.Attribute;
namespace LapisApi.Extensions;

public static class EnumExtensions
{
  public static string GetDescription(this Enum value, string lang = "en")
  {
    var field = value.GetType().GetField(value.ToString());

    if (field == null)
      return value.ToString();

    var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
    var arabicAttribute = Attribute.GetCustomAttribute(field, typeof(ArabicDescriptionAttribute)) as ArabicDescriptionAttribute;

    // fallback hierarchy: use requested lang if available, otherwise fall back to the other
    if (lang == "ar")
    {
      return arabicAttribute?.Description
             ?? attribute?.Description
             ?? value.ToString();
    }
    else
    {
      return attribute?.Description
             ?? arabicAttribute?.Description
             ?? value.ToString();
    }
  }
}