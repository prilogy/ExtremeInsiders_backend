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
  [Route("api/[controller]/[action]")]
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
    public IActionResult Like()
    {
      var videoId = _db.Videos.First().Id;
      var userId = _userService.User.Id;
      var like = _db.Likes.FirstOrDefault(l => l.VideoId == videoId && l.UserId == userId);
      if (like == null)
      {
        Console.WriteLine("add");
        _db.Likes.Add(new Like
        {
          VideoId = videoId,
          UserId = userId
        });
        
      }
      else
      {
        Console.WriteLine("delete");
        _db.Remove(like);
      }

      _db.SaveChanges();
      return Ok(_db.Videos.First().Likes);

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