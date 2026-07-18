using SisApi.App.Users.Model;
using System.Drawing;
namespace SisApi.App.Centers.Model
{
  public class Center
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public string? ManagerId { get; set; }
    public ApplicationUser? Manager { get; set; }
    
    // جميع العاملين التابعين للمركز
    public ICollection<ApplicationUser> Employees { get; set; }
      = new List<ApplicationUser>();
  }
}