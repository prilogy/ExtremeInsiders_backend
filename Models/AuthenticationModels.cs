using System.ComponentModel.DataAnnotations;

namespace ExtremeInsiders.Models
{
  public class AuthenticationModels
  {
    public class SignUp
    {
      [Required]
      [EmailAddress(ErrorMessage = "Неправильный Email")]
      public string Email { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
      public string Password { get; set; }
      
      public static explicit operator SignUp(SocialSignUp a)  // explicit byte to digit conversion operator
      {
        return new SignUp
        {
          Email = a.Email,
          Name = a.Name,
          Password = a.Password
        };
      }
    }

    public class SocialSignUp
    {
      [Required]
      [EmailAddress(ErrorMessage = "Неправильный Email")]
      public string Email { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
      public string Password { get; set; }
      [Required]
      public int SocialAccountId { get; set; }
      [Required]
      public string SocialAccountKey { get; set; }
    }
    
    public class LogIn
    {
      [Required] public string Email { get; set; }
      [Required] public string Password { get; set; }
    }

    public class SocialLogIn
    {
      [Required]
      public string Token { get; set; } 
    }
  }
}