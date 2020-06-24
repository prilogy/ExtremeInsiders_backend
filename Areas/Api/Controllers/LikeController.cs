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
  public class LikeController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;

    public LikeController(ApplicationContext db, UserService userService)
    {
      _db = db;
      _userService = userService;
    }

    // TODO: refactor
    [HttpGet("{id}")]
    public IActionResult Get(int id)
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
      return Ok(_userService.User.WithoutSensitive(useLikeIds: true));
    }

    // [HttpGet("movie/{id}")]
    // public IActionResult LikeMovie(int id)
    // {
    //   var like = _db.LikesMovies.FirstOrDefault(l => l.EntityId == id && l.UserId == _userService.UserId);
    //   if (like == null)
    //   {
    //     var movie = _db.Videos.FirstOrDefault(x => x.Id == id);
    //     if (movie == null) return NotFound();
    //
    //     like = new LikeMovie()
    //     {
    //       EntityId = movie.Id,
    //       UserId = _userService.UserId,
    //     };
    //     _db.LikesMovies.Add(like);
    //   }
    //   else
    //   {
    //     _db.Remove(like);
    //   }
    //
    //   _db.SaveChanges();
    //   return Ok(_userService.User.WithoutSensitive(useLikeIds: true));
    // }
  }
}