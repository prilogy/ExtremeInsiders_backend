using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Helpers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ExtremeInsiders.Services
{
  public class EmailService
  {
    private readonly AppSettings _appSettings;
    private readonly ApplicationContext _db;

    public EmailService(IOptions<AppSettings> appSettings, ApplicationContext db)
    {
      _appSettings = appSettings.Value;
      _db = db;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("ExtremeInsiders", _appSettings.EmailSenderLogin));
      emailMessage.To.Add(new MailboxAddress("", email));
      emailMessage.Subject = subject;
      emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = message + "<hr/> <h3>© 2020 ExtremeInsiders</h3>" +
               "<style>* {font-weight: 300 !important}</style>"
      };
      
      await Task.Run(async () =>
      {
        using var client = new SmtpClient();
        
        await client.ConnectAsync(_appSettings.EmailSenderSmtp, 25, false);
        await client.AuthenticateAsync(_appSettings.EmailSenderLogin, _appSettings.EmailSenderPassword);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
      });
    }
  }
}