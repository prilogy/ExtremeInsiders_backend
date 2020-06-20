using System.ComponentModel.DataAnnotations;

namespace ExtremeInsiders.Areas.Admin.Models
{
  public class AuthenticationModels
  {
    public class SignUp
    {
      [Required]
      public string Name { get; set; }
      [Required]
      [EmailAddress]
      public string Email { get; set; }
      [Required]
      [MinLength(6)]
      public string Password { get; set; }
      public string Secret { get; set; }
    }
    
    public class LogIn
    {
      [Required]
      public string Email { get; set; }
      [Required]
      public string Password { get; set; }
    }
  }
}