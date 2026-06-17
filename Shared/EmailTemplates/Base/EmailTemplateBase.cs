using Microsoft.Extensions.Options;
using LapisApi.OptionConfigurations;
namespace LapisApi.Shared.EmailTemplates.Base;

public abstract class EmailTemplateBase
{
  protected readonly FrontendSettings _frontend;
  protected readonly EmailOptionSettings _smtp;

  protected EmailTemplateBase(
    IOptions<FrontendSettings> frontendOptions,
    IOptions<EmailOptionSettings> smtpOptions
  )
  {
    _frontend = frontendOptions.Value;
    _smtp = smtpOptions.Value;
  }

  /// <summary>
  /// شعار الموقع - يتم تضمينه في أعلى البريد
  /// </summary>
  protected string GetLogoHtml() => string.IsNullOrWhiteSpace(_frontend.SiteLogo)
    ? ""
    : $"<div style='text-align:center; background-color: #3c3c3c ;margin-bottom: 15px'><img src='{_frontend.SiteLogo}' alt='logo' style='max-height:60px; margin:20px auto;' /></div>";


  protected string GetLogoUrl()
  {
    return _frontend.SiteLogo;
  }
  /// <summary>
  /// نص الفوتر الذي يتضمن بريد الدعم - بلغة واتجاه معينين
  /// </summary>
  protected string GetFooterHtml(string lang, string align)
  {
    string supportEmail = string.IsNullOrWhiteSpace(_smtp.SupportEmail)
      ? "info@buzcash.com"
      : _smtp.SupportEmail;

    return $@"
<p style='font-size:13px;color:#999;text-align:{align};margin-top:30px;'>
  {(lang == "en"
    ? $"This is an automated email. Please do not reply. For assistance, contact <a href='mailto:{supportEmail}'>{supportEmail}</a>"
    : $"هذه رسالة تلقائية، الرجاء عدم الرد  للتواصل، يرجى الكتابة إلى <a href='mailto:{supportEmail}'>{supportEmail}</a>")}
</p>";
  }

  protected string GetFontFamilyStyle(string lang)
  {
    return  "font-family: 'IBM Plex Sans Arabic', sans-serif;";
  }

  /// <summary>
  /// رابط عام للموقع (مثلاً لزر تتبع أو سجل الحوالات)
  /// </summary>
  protected string GetSiteUrl(string path = "")
  {
    var baseUrl = string.IsNullOrWhiteSpace(_frontend.SiteUrl) ? "https://buzcash.com" : _frontend.SiteUrl;
    return string.IsNullOrWhiteSpace(path) ? baseUrl : $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
  }
}