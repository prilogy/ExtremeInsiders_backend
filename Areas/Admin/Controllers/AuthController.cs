using System;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using AuthenticationModels = ExtremeInsiders.Areas.Admin.Models.AuthenticationModels;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class AuthController : Controller
  {
    private readonly UserService _userService;
    private readonly AppSettings _appSettings;
    private readonly ApplicationContext _db;

    public AuthController(UserService userService, IOptions<AppSettings> appSettings, ApplicationContext db)
    {
      _userService = userService;
      _appSettings = appSettings.Value;
      _db = db;
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
        return RedirectToAction("Index", "Home");
      
      ModelState.AddModelError("", "Неправильный пароль или логин, или вы не админ.");
      return View(model);
    }
    
    [HttpGet]
    public IActionResult SignUp()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(AuthenticationModels.SignUp model)
    {
      if (model.Secret != _appSettings.AdminSignUpSecret)
      {
        ModelState.AddModelError("","Неправильный секретный код");
        return View(model);
      }

      var newModel = new Api.Models.AuthenticationModels.SignUp
      {
        Password = model.Password,
        Email = model.Email,
        Name = model.Name,
        DateBirthday = "20.02.2000",
      };
      
      var user = await _userService.Create(newModel, true);
      await _db.Users.AddAsync(user);
      await _db.SaveChangesAsync();
      
      return RedirectToRoute(new { area = "Admin", controller = "Auth", action = "LogIn" });
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