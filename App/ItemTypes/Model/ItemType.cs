namespace SisApi.App.ItemTypes.Model
{
  public class ItemType
  {
    public int Id { get; set; }

    public string Name{ get; set; } 
    public int PointsPerKg { get; set; }
    public bool IsActive { get; set; } = true;
  }
}