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

    public void SendEmailConfirmationAsync(User user)
    {
      var confirmationCode = new ConfirmationCode
      {
        UserId = user.Id,
        Type = ConfirmationCode.Types.EmailConfirmation
      };
      _db.ConfirmationCodes.Add(confirmationCode);
      _db.SaveChanges();
      var resource = CultureResources.EmailConfirmation(_userService.Culture);
      
      _ = _emailService.SendEmailAsync(user.Email, resource["subject"],
        $"<h3 style=\"font-weight: 300;\">{resource["body_before_code"]}:</h3> " +
        $"<h1 style=\"font-weight: 300;\">{confirmationCode.Code}</h1> <h3 style=\"font-weight: 300;\">{resource["body_after_code"]}</h3>");
    }

  }
}