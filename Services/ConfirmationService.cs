using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Services
{
  public class ConfirmationService
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly EmailService _emailService;

    public ConfirmationService(ApplicationContext db, UserService userService, EmailService emailService)
    {
      _db = db;
      _userService = userService;
      _emailService = emailService;
    }

    public async Task SendEmailConfirmationAsync(User user)
    {
      var confirmationCode = new ConfirmationCode
      {
        UserId = user.Id,
        Type = ConfirmationCode.Types.EmailConfirmation
      };
      await _db.ConfirmationCodes.AddAsync(confirmationCode);
      await _db.SaveChangesAsync();
      var resource = CultureResources.EmailConfirmation(_userService.Culture);
      _ = _emailService.SendEmailAsync(user.Email, resource["subject"],
        GenerateMessageWithCode(beforeCode:
          resource["body_before_code"],
          code: confirmationCode.Code,
          afterCode: resource["body_after_code"]));
    }

    public async Task SendPasswordReset(User user)
    {
      var confirmationCode = new ConfirmationCode
      {
        UserId = user.Id,
        Type = ConfirmationCode.Types.PasswordReset
      };

      await _db.ConfirmationCodes.AddAsync(confirmationCode);
      await _db.SaveChangesAsync();
      var resource = CultureResources.PasswordReset(_userService.Culture);
      _ = _emailService.SendEmailAsync(user.Email, resource["subject"],
        GenerateMessageWithCode(beforeCode:
          resource["body_before_code"],
          code: confirmationCode.Code,
          afterCode: resource["body_after_code"]));
    }

    private string GenerateMessageWithCode(string beforeCode, string afterCode, string code) =>
      $"<h3 style=\"font-weight: 300;\">{beforeCode}:</h3> " +
      $"<h1 style=\"font-weight: 300;\">{code}</h1> <h3 style=\"font-weight: 300;\">{afterCode}</h3>";
  }
}