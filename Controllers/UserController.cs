using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class UserController: Controller
  {
    private readonly ApplicationContext _db;
    private readonly IUserService _userService;
    private readonly IEnumerable<SocialAuthService> _authServices;

    public UserController(ApplicationContext db, IUserService userService, IEnumerable<SocialAuthService> authServices)
    {
      _db = db;
      _userService = userService;
      _authServices = authServices;
    }
    
    [HttpPut("{type}")]
    public async Task<IActionResult> SocialAccount(string type, AuthenticationModels.SocialLogIn model)
    {
      var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
      if (handler != null)
      {
        var user = _userService.User;
        var account = await handler.CreateAccount(model.Token);
        if (account != null)
        {
          user.SocialAccounts.Add(account);
          _db.SaveChanges();
          return Ok(user.WithoutSensitive());
        }

        ModelState.AddModelError("Auth", $"Не удалось привязать {type} аккаунт.");
        return BadRequest(ModelState);
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети.");
      return NotFound(ModelState);
    }

    [HttpDelete("{type}")]
    public IActionResult SocialAccount(string type)
    {
      var user = _userService.User;
      var toRemove = user.SocialAccounts.SingleOrDefault(a => a.Provider.Name == type);

      if (toRemove != null)
      {
        user.SocialAccounts.Remove(toRemove);
        _db.SaveChanges();
        return Ok(user.WithoutSensitive());
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети или аккаунт не найден.");
      return NotFound(ModelState);
    }

  }
}