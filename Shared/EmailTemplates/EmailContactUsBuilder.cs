using Microsoft.Extensions.Options;
using System.Web;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.OptionConfigurations;
using LapisApi.Shared.EmailTemplates.Base;
namespace LapisApi.Shared.Services;

public class EmailContactUsBuilder : EmailTemplateBase
{
    public EmailContactUsBuilder(
        IOptions<FrontendSettings> frontendOptions,
        IOptions<EmailOptionSettings> smtpOptions
    ) : base(frontendOptions, smtpOptions)
    {
    }

    public (string subject, string html) Build(ContactUsCommand data, bool isAgent, string lang = "ar")
    {
        var dir = lang == "en" ? "ltr" : "rtl";
        var align = lang == "en" ? "left" : "right";
        var textAlign = "center";

        var subject = isAgent
            ? (lang == "en" ? "New Agent Application - BuzCash" : "طلب انضمام كوكيل جديد - BuzCash")
            : (lang == "en" ? "New Contact Message - BuzCash" : "رسالة تواصل جديدة - BuzCash");

        var title = isAgent
            ? (lang == "en" ? "New Agent Application" : "طلب انضمام كوكيل")
            : (lang == "en" ? "New Contact Message" : "رسالة تواصل جديدة");

        var fullNameLabel = lang == "en" ? "Full Name" : "الاسم الكامل";
        var emailLabel = lang == "en" ? "Email" : "البريد الإلكتروني";
        var phoneLabel = lang == "en" ? "Phone Number" : "رقم الهاتف";
        var jobTypeLabel = lang == "en" ? "Job Type" : "نوع الوظيفة";
        var messageLabel = lang == "en" ? "Message" : "الرسالة";

        var jobTypeRow = isAgent && !string.IsNullOrWhiteSpace(data.JobType)
            ? $"<tr><th style='{thStyle(lang)}'>{jobTypeLabel}</th><td style='{tdStyle()}'>{HttpUtility.HtmlEncode(data.JobType)}</td></tr>"
            : "";

        
        var fontStyle = "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;";

        string html = $@"
<html dir='{dir}'>
<head>
  <style>
    {fontStyle}
    body {{
      background-color: #f9f9f9;
      padding: 20px;
      margin: 0;
    }}
    .container {{
      max-width: 650px;
      margin: auto;
      background: white;
      border-radius: 10px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }}
    .header {{
      background: #11385e;
      color: white;
      padding: 20px;
      text-align: {textAlign};
    }}
    .header h1 {{
      margin: 0;
      font-size: 20px;
      font-weight: 600;
    }}
    .content {{
      padding: 30px;
      text-align: {align};
    }}
    .content h2 {{
      color: #11385e;
      border-bottom: 2px solid #F8B500;
      padding-bottom: 8px;
      margin-bottom: 20px;
    }}
    .info-table {{
      width: 100%;
      border-collapse: collapse;
      margin: 20px 0;
      font-size: 15px;
    }}
    .info-table th, .info-table td {{
      padding: 10px;
      border: 1px solid #eee;
      vertical-align: top;
    }}
    .info-table th {{
      background-color: #f7f7f7;
      text-align: {align};
      width: 30%;
      font-weight: bold;
    }}
    .info-table td {{
      background-color: #fcfcfc;
    }}
    .message-box {{
      background: #f0f8ff;
      border: 1px solid #cce7ff;
      border-radius: 6px;
      padding: 15px;
      margin-top: 15px;
      font-style: italic;
      color: #333;
    }}
  </style>
</head>
<body>
  <div class='container'>


    <div class='header'>
      <h1>{title}</h1>
    </div>

    <div class='content'>
      <h2>{(lang == "en" ? "Client Details" : "تفاصيل المرسل")}</h2>
      <table class='info-table'>
        <tr>
          <th style='{thStyle(lang)}'>{fullNameLabel}</th>
          <td style='{tdStyle()}'>{HttpUtility.HtmlEncode(data.FullName)}</td>
        </tr>
        <tr>
          <th style='{thStyle(lang)}'>{emailLabel}</th>
          <td style='{tdStyle()}'><a href='mailto:{data.Email}'>{data.Email}</a></td>
        </tr>
        <tr>
          <th style='{thStyle(lang)}'>{phoneLabel}</th>
          <td style='{tdStyle()}'><a href='tel:{data.PhoneNumber}'>{data.PhoneNumber}</a></td>
        </tr>
        {jobTypeRow}
      </table>

      <h2>{messageLabel}</h2>
      <div class='message-box'>
        {HttpUtility.HtmlEncode(data.Message).Replace("\n", "<br/>")}
      </div>
    </div>
  </div>
</body>
</html>";

        return (subject, html);
    }

    // دوال مساعدة للـ inline styles (للتخصيص السريع)
    private string thStyle(string lang) => 
        $"text-align: {(lang == "en" ? "left" : "right")}; font-weight: bold; background: #f7f7f7; width: 30%;";

    private string tdStyle() => 
        "text-align: left; background: #fcfcfc;";
}