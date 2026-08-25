using SisApi.App.ItemTypes.Model;

namespace SisApi.App.Orders.Model
{
  public class OrderItem
  {
    public int Id { get; set; }

    // الطلب الذي ينتمي إليه العنصر
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // نوع المادة: بلاستيك، زجاج، ألمنيوم...
    public int ItemTypeId { get; set; }
    public ItemType ItemType { get; set; } = null!;

    // الوزن الحقيقي بعد استلام المادة
    public decimal? WeightKg { get; set; }

    // قيمة النقاط لكل كيلو وقت تنفيذ الطلب
    public int PointsPerKg { get; set; }

    // إجمالي نقاط هذا العنصر
    public decimal Points { get; set; }
  }
}