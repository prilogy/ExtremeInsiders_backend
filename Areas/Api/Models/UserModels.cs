using System.ComponentModel.DataAnnotations;

namespace ExtremeInsiders.Areas.Api.Models
{
  public static class UserModels
  {
    public class PasswordReset
    {
      [Required]
      public string Code { get; set; }
      [Required]
      public string Password { get; set; }
    }
  }
}