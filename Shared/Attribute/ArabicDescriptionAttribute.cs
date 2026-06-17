namespace LapisApi.Shared.Attribute;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public class ArabicDescriptionAttribute : System.Attribute
{
  public string Description { get; }

  public ArabicDescriptionAttribute(string description)
  {
    Description = description;
  }
}