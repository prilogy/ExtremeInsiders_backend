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
using Microsoft.CodeAnalysis.CSharp;
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

    public AuthController(ApplicationContext db, UserService userService, IEnumerable<SocialAuthService> authServices,
      ConfirmationService confirmationService)
    {
      _db = db;
      _userService = userService;
      _authServices = authServices;
      _confirmationService = confirmationService;
    }

    [HttpPut("signUp")]
    public async Task<IActionResult> SignUp([FromForm] AuthenticationModels.SignUp model)
    {
      var user = await SignUpInternal(model);
      if (user != null)
      {
        return Ok();
      }

      ModelState.AddModelError("Auth", "Email уже зарегистрирован.");
      return BadRequest(ModelState);
    }

    [HttpPost("logIn")]
    public async Task<IActionResult> LogIn(AuthenticationModels.LogIn model)
    {
      var user = await _userService.Authenticate(model.Email, model.Password);

      if (user != null)
        return Ok(user.WithoutSensitive(token: true, useLikeIds: true, useFavoriteIds: true, useSaleIds: true));

      ModelState.AddModelError("Auth", "Неправильный логин или пароль");
      return NotFound(ModelState);
    }

    [HttpPost("logIn/{type}")]
    public async Task<IActionResult> LogIn(string type, AuthenticationModels.SocialLogIn model)
    {
      var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
      if (handler != null)
      {
        var identity = await handler.GetIdentity(model.Token);

        var user = await handler.FindUser(identity);
        if (user == null)
        {
          await using var transaction = await _db.Database.BeginTransactionAsync();
          try
          {
            var account = await handler.CreateAccount(identity);
            if (account == null) return BadRequest();
          
            user = await SignUpInternal(new AuthenticationModels.SignUp());
            
            if (user == null) return BadRequest();
            account.UserId = user.Id;

            await _db.AddAsync(account);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
          }
          catch (Exception)
          {
            return BadRequest();
          }
        }

        user = await _userService.Authenticate(user);
        
        if (user != null)
          return Ok(user.WithoutSensitive(token: true, useLikeIds: true, useFavoriteIds: true, useSaleIds: true));

        return NotFound(ModelState);
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети.");
      return NotFound(ModelState);
    }


    [Authorize]
    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh([FromQuery] bool token)
    {
      if (_userService.User == null) return BadRequest();
      if (token)
        return Ok((await _userService.Authenticate(_userService.User)).WithoutSensitive(token: true, useLikeIds: true,
          useFavoriteIds: true, useSaleIds: true));
      return Ok(_userService.User.WithoutSensitive(token: false, useLikeIds: true, useFavoriteIds: true,
        useSaleIds: true));
    }

    private async Task<User> SignUpInternal(AuthenticationModels.SignUp model)
    {
      var user = await _userService.Create(model);
      if (user == null) return null;
      user.CultureId = _userService.Culture.Id;
      user.CurrencyId = _userService.Currency.Id;

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