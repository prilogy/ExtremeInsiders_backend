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
      public string Name { get; set; }
      [Required]
      [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
      public string Password { get; set; }
      
      [Required]
      public string PhoneNumber { get; set; }
      [Required]
      public string DateBirthday { get; set; }
      public IFormFile Avatar { get; set; }
      
      public static explicit operator SignUp(SocialSignUp a)  // explicit byte to digit conversion operator
      {
        return new SignUp
        {
          Email = a.Email,
          Name = a.Name,
          Password = a.Password,
          DateBirthday = a.BirthDate,
          Avatar = a.Avatar,
          PhoneNumber = a.PhoneNumber
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
      public string PhoneNumber { get; set; }
      [Required]
      public string BirthDate { get; set; }
      public IFormFile Avatar { get; set; }
      
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