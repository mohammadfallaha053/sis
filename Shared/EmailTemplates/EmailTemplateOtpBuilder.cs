using Microsoft.Extensions.Options;
using LapisApi.App.Auth.Enums;
using LapisApi.OptionConfigurations;
using LapisApi.Shared.EmailTemplates.Base;

namespace LapisApi.Shared.EmailTemplates;

public class EmailTemplateOtpBuilder : EmailTemplateBase
{
  public EmailTemplateOtpBuilder(
    IOptions<FrontendSettings> frontendOptions,
    IOptions<EmailOptionSettings> smtpOptions
  ) : base(frontendOptions, smtpOptions)
  {
  }

  public (string subject, string html) Build(OtpPurposeEnum purpose, string code, string lang)
  {
    var direction = lang == "en" ? "ltr" : "rtl";
    var subject = GetSubject(purpose, lang);
    var title = GetTitle(purpose, lang);
    var buttonText = lang == "en" ? "Visit site" : "زيارة الموقع";
    var noteBox = lang == "en"
      ? "<div style='background:#fff7e6;border-left:4px solid #ffcc00;padding:10px 15px;margin-bottom:30px;font-size:14px; color:#333;'><strong>Important:</strong> This code will expire in 5 minutes. If you didn't request it, please ignore.</div>"
      : "<div style='background:#fff7e6;border-right:4px solid #ffcc00;padding:10px 15px;margin-bottom:30px;font-size:14px; color:#333;'><strong>هام:</strong> سينتهي صلاحية هذا الرمز خلال 5 دقائق. إذا لم تطلبه، تجاهل الرسالة.</div>";

    var logo = GetLogoHtml();
    var footer = GetFooterHtml(lang, "center");
    var siteUrl = GetSiteUrl();
    string fontStyle = GetFontFamilyStyle(lang);
    var html = $@"
<html dir='{direction}'>
  <body style='margin:0;padding:0;background-color:#f4f4f4;{fontStyle}text-align:center;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='padding:30px 0;'>
      <tr>
        <td align='center'>
          <table cellpadding='0' cellspacing='0' border='0' style='max-width:600px; background-color:#ffffff; border-radius:6px; width:100%;'>
            <tr>
              <td style='padding: 40px 20px; text-align: center;'>
                {logo}
                <h2 style='margin:0 0 20px 0; font-size:22px;'>{title}</h2>
                {noteBox}
                <p style='font-size:18px; margin-bottom:30px;'>{(lang == "en" ? "Your code is" : "رمز التحقق هو")}:
                  <strong style='font-size:20px;'>{code}</strong>
                </p>
                <a href='{siteUrl}' 
                   style='display:inline-block;padding:12px 25px;background:#F8B500;color:#3c3c3c;text-decoration:none;border-radius:5px;font-size:16px;margin-bottom:20px;'>
                  {buttonText}
                </a>
                {footer}
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";

    return (subject, html);
  }

  private string GetSubject(OtpPurposeEnum purpose, string lang) => purpose switch
  {
    OtpPurposeEnum.EmailConfirmation => lang == "en" ? "Confirm your email" : "تأكيد البريد الإلكتروني",
    OtpPurposeEnum.PasswordReset => lang == "en" ? "Password Reset Code" : "رمز إعادة تعيين كلمة المرور",
    _ => lang == "en" ? "Two Factor Verification Code" : "🔐 رمز التحقق بخطوتين"
  };

  private string GetTitle(OtpPurposeEnum purpose, string lang) => purpose switch
  {
    OtpPurposeEnum.EmailConfirmation => lang == "en" ? "Confirm your email" : "تأكيد بريدك الإلكتروني",
    OtpPurposeEnum.PasswordReset => lang == "en" ? "Reset your password" : "إعادة تعيين كلمة المرور",
    _ => lang == "en" ? "Your verification code" : "رمز التحقق الخاص بك"
  };
}