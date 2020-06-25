using System;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AuthenticationModels = ExtremeInsiders.Areas.Admin.Models.AuthenticationModels;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class AuthController : Controller
  {
    private readonly UserService _userService;
    private readonly AppSettings _appSettings;

    public AuthController(UserService userService, IOptions<AppSettings> appSettings)
    {
      _userService = userService;
      _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult LogIn()
    {
      return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogIn(AuthenticationModels.LogIn model)
    {
      var user = await _userService.AuthenticateCookies(model.Email, model.Password, true);
      if (user != null)
        return Redirect("/admin/");
      
      ModelState.AddModelError("", "Неправильный пароль или логин, или вы не админ.");
      return View(model);
    }
    
    [HttpGet]
    public IActionResult SignUp()
    {
      return View();
    }

    [HttpPost]
    public IActionResult SignUp(AuthenticationModels.SignUp model)
    {
      if (model.Secret != _appSettings.AdminSignUpSecret)
      {
        ModelState.AddModelError("","Неправильный секретный код");
        return View(model);
      }
      
      return View();
    }
    
    [HttpGet]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LogOut()
    {
      await HttpContext.SignOutAsync("Cookies");
      return Redirect("/admin/auth/login");
    }
  }
}