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
  public class PlaylistController : ControllerBase<Playlist, PlaylistTranslation>
  {
    public PlaylistController(ApplicationContext db, UserService userService) : base(db, userService)
    {
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<Playlist>>> GetPopular([FromQuery] int page, [FromQuery] int pageSize)
    {
      var list = _db.Set<Playlist>()
        .OrderByDescending(x => (from y in _db.Videos where y.PlaylistId == x.Id select y.Likes).Count());
      return await Paging(list, page, pageSize);
    }

    public override async Task<IActionResult> Search(string query, int id)
    {
      var playlist = await _db.Playlists.FirstOrDefaultAsync(x => x.Id == id);
      if (playlist == null)
        return NotFound();

      return Ok(new
      {
        Videos = playlist.Videos.SearchAtWithQueryAsync<Video, VideoTranslation>(query).OfFormat(_userService)
      });
    }

    protected override IQueryable<Playlist> GetRecommendedQueryable()
    {
      var ids = _userService.User.Favorites.Where(x => x.Entity is Sport)
        .SelectMany(x => (x.Entity as Sport)?.Playlists).Select(x => x.Id).ToList();
      return ids.Count != 0
        ? _db.Playlists.Where(x => ids.Contains(x.Id)).OrderByDescending(x => x.Videos.Count).AsQueryable()
        : _db.Playlists.OrderByDescending(x => x.Videos.Count).ThenByDescending(x => x.DateCreated);
    }
  }
}