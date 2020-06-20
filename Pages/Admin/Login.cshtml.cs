using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExtremeInsiders.Pages.Admin
{
  
  public class Login : PageModel
  {
    private readonly UserService _userService;

    public Login(UserService userService)
    {
      _userService = userService;
    }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    
    public void OnGet()
    {
     
    }

    public async Task<IActionResult> OnPost()
    {
      var user = await _userService.AuthenticateCookies(Request.Form["email"], Request.Form["password"]);
      if (user != null && user.Role.Name == Role.AdminRole)
        return RedirectToPage("/admin/index");
      
      ModelState.AddModelError("", "Некорректные логин или пароль, или вы не админ");
      return Page();
    }
    
    public async Task<IActionResult> OnGetLogOut()
    {
      Console.WriteLine("lllllllll");
      if (User.Identity.IsAuthenticated)
      {
        await HttpContext.SignOutAsync();
        return Redirect("/admin/login");
      }

      return BadRequest();
    }
  }
}