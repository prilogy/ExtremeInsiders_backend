using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

    public class ProfileEdit
    {
      public string Email { get; set; }
      public string Name { get; set; }
      public IFormFile AvatarSrc { get; set; }
      public string PhoneNumber { get; set; }
    }
  }
}