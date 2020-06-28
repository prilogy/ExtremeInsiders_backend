using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Google.Apis.Auth;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly IEnumerable<SocialAuthService> _authServices;
    private readonly ConfirmationService _confirmationService;

    public AuthController(ApplicationContext db, UserService userService, IEnumerable<SocialAuthService> authServices, ConfirmationService confirmationService)
    {
      _db = db;
      _userService = userService;
      _authServices = authServices;
      _confirmationService = confirmationService;
    }

    [HttpPut("signUp")]
    public async Task<IActionResult> SignUp([FromForm]AuthenticationModels.SignUp model)
    {
      var user = await SignUpInternal(model);
      if (user != null)
      {
        return Ok();
      }

      ModelState.AddModelError("Auth", "Email уже зарегистрирован.");
      return BadRequest(ModelState);
    }

    [HttpPost("signUp/{type}")]
    public async Task<IActionResult> SignUp(string type, AuthenticationModels.SocialLogIn model)
    {
      var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
      if (handler != null)
      {
        var identity = await handler.GetIdentity(model.Token);
        if (identity != null)
        {
          var account = await handler.CreateAccount(identity);
          if (account != null)
          {
            _db.SocialAccounts.Add(account);
            _db.SaveChanges();

            return Ok(new AuthenticationModels.SocialSignUp
            {
              SocialAccountKey = identity.Id, Email = identity.Email, Name = identity.Name, SocialAccountId = account.Id
            });
          }
        }

        ModelState.AddModelError("Auth",
          $"Профиль {type} не существует или уже привязан к одному из аккаунтов Extreme Insiders.");
        return BadRequest(ModelState);
      }

      ModelState.AddModelError("Auth", "Неправильный тип социальной сети.");
      return NotFound(ModelState);
    }

    [HttpPut("signUp/{type}")]
    public async Task<IActionResult> SignUp(string type, [FromForm]AuthenticationModels.SocialSignUp model)
    {
      var user = await SignUpInternal((AuthenticationModels.SignUp) model);
      if (user != null)
      {
        var account =
          await _db.SocialAccounts.SingleOrDefaultAsync(a =>
            a.Id == model.SocialAccountId && a.Key == model.SocialAccountKey && a.UserId == null);
        if (account != null)
        {
          account.UserId = user.Id;
          await _db.SaveChangesAsync();
          return Ok();
        }
        
        ModelState.AddModelError("Auth", $"Профиль {type} уже привязан к одному из аккаунтов Extreme Insiders.");
        return BadRequest(ModelState);
      }

      ModelState.AddModelError("Auth", "Email уже зарегистрирован.");
      return BadRequest(ModelState);
    }

    [HttpPost("logIn")]
    public async Task<IActionResult> LogIn(AuthenticationModels.LogIn model)
    {
      var user = await _userService.Authenticate(model.Email, model.Password);

      if (user != null)
        return Ok(user.WithoutSensitive(token: true, useLikeIds: true, useFavoriteIds: true));

      ModelState.AddModelError("Auth", "Неправильный логин или пароль");
      return NotFound(ModelState);
    }

    [HttpPost("logIn/{type}")]
    public async Task<IActionResult> LogIn(string type, AuthenticationModels.SocialLogIn model)
    {
      var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
      if (handler != null)
      {
        var user = await handler.FindUser(model.Token);

        if (user != null)
          return Ok(user.WithoutSensitive(token: true, useLikeIds: true, useFavoriteIds: true));

        ModelState.AddModelError("Auth", $"Ваш профиль {type} не привязан к аккаунту.");
        return NotFound(ModelState);
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети.");
      return NotFound(ModelState);
    }
    
        
    [Authorize]
    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
      return Ok((await _userService.Authenticate(_userService.User)).WithoutSensitive(token: true, useLikeIds: true, useFavoriteIds:true));
    }

    private async Task<User> SignUpInternal(AuthenticationModels.SignUp model)
    {
      var user = await _userService.Create(model);
      if (user == null) return null;
      user.CultureId = _userService.Culture.Id;
      user.CurrencyId = _userService.Currency.Id;
      
      if (model.Avatar != null && user.Avatar == null)
      {
        ModelState.AddModelError("Auth", "Неправильный файл для аватара");
        return null;
      }

      await _db.Users.AddAsync(user);
      await _db.SaveChangesAsync();
      
      await _confirmationService.SendEmailConfirmationAsync(user);
      var subscription = Subscription.Demo(user);
      await _db.Subscriptions.AddAsync(subscription);
      await _db.SaveChangesAsync();
      return user;
    }

  }
}