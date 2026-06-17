using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LapisApi.App.Settings.Model
{
  public class SettingConfiguration : IEntityTypeConfiguration<Setting>
  {
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
      builder.HasData(new Setting
      {
        Id = 1,
        PhoneNumber = "123-456-7890",
        Email = "example@example.com",
        Address = "123 Example St.",

        AboutUs_Ar = "نحن شركة متخصصة في خدمات التحويلات المالية.",
        AboutUs_En = "We are a company specialized in money transfer services.",

        ContactUs_Ar = "يمكنكم التواصل معنا عبر البريد الإلكتروني أو الهاتف.",
        ContactUs_En = "You can contact us via email or phone.",

        FacebookUrl = "https://facebook.com/example",
        InstagramUrl = "https://instagram.com/example",
        TwitterUrl = "https://twitter.com/example",
        YoutubeUrl = "https://youtube.com/@example",
        TiktokUrl = "https://tiktok.com/@example"
      });
    }
  }
}