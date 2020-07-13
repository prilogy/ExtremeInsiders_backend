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
    
    protected override IQueryable<Playlist> GetRecommendedQueryable() => _userService.User.Favorites.Count(x => x.Entity is Sport) > 0 
      ? _db.Playlists.Where(x => _userService.User.Favorites.Select(x => x.Id).Contains(x.SportId)) : _db.Playlists.OrderBy(x => x.DateCreated);
  }
}