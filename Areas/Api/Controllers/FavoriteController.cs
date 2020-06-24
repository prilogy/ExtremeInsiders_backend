using System;
using System.Linq;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class FavoriteController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;

    public FavoriteController(ApplicationContext db, UserService userService)
    {
      _db = db;
      _userService = userService;
    }
    
    [HttpGet("{id}")]
    public IActionResult Get(int id)
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
      return Ok(_userService.User.WithoutSensitive(useLikeIds: true, useFavoriteIds: true));
    }
  }
}