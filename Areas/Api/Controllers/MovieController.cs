using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  public class MovieController : ControllerBase<Movie, MovieTranslation>
  {
    public MovieController(ApplicationContext db, UserService userService) : base(db, userService)
    {
    }

    protected override IQueryable<Movie> GetRecommendedQueryable() => _db.Movies.OrderByDescending(x => x.Likes.Count);
  }

  // Интересная имплементация вместо generic
  // [ApiController]
  // [Route("api/[controller]")]
  // public class EntityController : Controller
  // {
  //   private readonly ApplicationContext _db;
  //   private readonly UserService _userService;
  //
  //   public EntityController(ApplicationContext db, UserService userService)
  //   {
  //     _db = db;
  //     _userService = userService;
  //   }
  //
  //   [HttpGet("{id}")]
  //   public async Task<IActionResult> GetById(int id)
  //   {
  //     var entity = await _db.EntitiesBase.FindAsync(id);
  //     return entity switch
  //     {
  //       null => NotFound(),
  //       Sport s => Ok(s.OfFormat(_userService)),
  //       Playlist p => Ok(p.OfFormat(_userService)),
  //       Video v => Ok(v.OfFormat(_userService)),
  //       Movie m => Ok(m.OfFormat(_userService)),
  //       _ => BadRequest()
  //     };
  //   }
  // }
}