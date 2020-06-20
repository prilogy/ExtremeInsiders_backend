using System;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExtremeInsiders.Pages.Admin
{
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
  public class SignUp : PageModel
  {
    public string Email { get; set; } 
    public string Password { get; set; } 
    public string ConfirmPassword { get; set; } 
    

    public void OnGet()
    {
      Console.WriteLine("ssss");  
    }
    
  }
}