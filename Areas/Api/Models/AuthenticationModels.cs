using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Areas.Api.Models
{
  public static class AuthenticationModels
  {
    public class SignUp
    {
      [Required]
      [EmailAddress(ErrorMessage = "Неправильный Email")]
      public string Email { get; set; }

      [Required]
      [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
      public string Password { get; set; }

      public string Name { get; set; }
    }

    public class LogIn
    {
      [Required] public string Email { get; set; }
      [Required] public string Password { get; set; }
    }

    public class SocialLogIn
    {
      [Required] public string Token { get; set; }
    }
  }
}