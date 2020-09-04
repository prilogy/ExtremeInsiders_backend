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
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  public class SportController : ControllerBase<Sport, SportTranslation>
  {
    public SportController(ApplicationContext db, UserService userService) : base(db, userService)
    {
    }

    public override async Task<IActionResult> Search(string query, int id)
    {
      query = query.ToLower();

      var sport = await _db.Sports.FirstOrDefaultAsync(x => x.Id == id);
      if (sport == null)
        return NotFound();

      return Ok(new
      {
        Videos = sport.Playlists.SelectMany(x => x.Videos).SearchAtWithQueryAsync<Video, VideoTranslation>(query)
          .OfFormat(_userService),
        Playlists = sport.Playlists.SearchAtWithQueryAsync<Playlist, PlaylistTranslation>(query).OfFormat(_userService)
      });
    }

    protected override IQueryable<Sport> GetRecommendedQueryable() =>
      _db.Sports.OrderByDescending(x => x.Playlists.Count).ThenByDescending(x => x.Movies.Count);
  }
}