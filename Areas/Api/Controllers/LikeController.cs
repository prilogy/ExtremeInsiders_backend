using System;
using System.Linq;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet("{type}/{id}")]
    public IActionResult Get(string type, int id)
    {
      Console.WriteLine(type + id);
      var idd = _db.Movies.First().Id;
      var userId = _userService.User.Id;
      var like = _db.LikesMovies.FirstOrDefault(l => l.EntityId == idd && l.UserId == userId);
      if (like == null)
      {
        Console.WriteLine("add");
        _db.LikesMovies.Add(new LikeMovie
        {
          EntityId = idd,
          UserId = userId
        });
        
      }
      else
      {
        Console.WriteLine("delete");
        _db.Remove(like);
      }

      _db.SaveChanges();
      return Ok(new
      {
        movie = _db.Movies.FirstOrDefault(),
        user = _userService.User.WithoutSensitive(useLikeIds:true)
      });

      // var like = new Like
      // {
      //   VideoId = videoId,
      //   User = _userService.User
      // };
      // _db.Likes.Add(like);
      // _db.SaveChanges();
      //
      // return Ok(like);
      //
      // return BadRequest();
    }
  }
}