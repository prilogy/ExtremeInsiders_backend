using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class UserController: Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly IEnumerable<SocialAuthService> _authServices;

    public UserController(ApplicationContext db, UserService userService, IEnumerable<SocialAuthService> authServices)
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
          return Ok(user.WithoutSensitive(false));
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
        return Ok(user.WithoutSensitive(false));
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети или аккаунт не найден.");
      return NotFound(ModelState);
    }

    [HttpGet("{id}")]
    public IActionResult Like(int id)
    {
      var like = _db.Likes.FirstOrDefault(l => l.EntityId == id && l.UserId == _userService.UserId);
      if (like == null)
      {
        var entity = _db.EntitiesLikeable.FirstOrDefault(x => x.Id == id);
        if (entity == null) return NotFound();

        like = new Like
        {
          EntityId = entity.Id,
          UserId = _userService.UserId,
        };
        _db.Likes.Add(like);
      }
      else
      {
        _db.Remove(like);
      }

      _db.SaveChanges();
      return Ok();
    }
    
    [HttpGet("{id}")]
    public IActionResult Favorite(int id)
    {
      var favorite = _db.Favorites.FirstOrDefault(l => l.EntityId == id && l.UserId == _userService.UserId);
      if (favorite == null)
      {
        var entity = _db.EntitiesBase.FirstOrDefault(x => x.Id == id);
        if (entity == null) return NotFound();

        favorite = new Favorite
        {
          EntityId = entity.Id,
          UserId = _userService.UserId,
        };
        _db.Favorites.Add(favorite);
      }
      else
      {
        _db.Remove(favorite);
      }

      _db.SaveChanges();
      return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail([FromBody] string code)
    {
      var confirmationCode = _userService.User.ConfirmationCodes.FirstOrDefault(x =>
        x.Type == ConfirmationCode.Types.EmailConfirmation && x.Code == code && x.IsConfirmed == false);

      if (confirmationCode == null)
        return BadRequest();

      confirmationCode.IsConfirmed = true;
      await _db.SaveChangesAsync();

      return Ok();
    }
  }
}