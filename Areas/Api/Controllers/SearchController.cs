using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class SearchController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;

    public SearchController(ApplicationContext db, UserService userService)
    {
      _db = db;
      _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody]string query)
    {
      return Ok(new
      {
        Sports = (await _db.Sports.SearchAtWithQueryAsync<Sport, SportTranslation>(query)).OfFormat(_userService),
        Playlists = (await _db.Playlists.SearchAtWithQueryAsync<Playlist, PlaylistTranslation>(query)).OfFormat(_userService),
        Videos = (await _db.Videos.SearchAtWithQueryAsync<Video, VideoTranslation>(query)).OfFormat(_userService),
        Movies = (await _db.Movies.SearchAtWithQueryAsync<Movie, MovieTranslation>(query)).OfFormat(_userService),
      });
    }

    [HttpPost("predict")]
    public async Task<IActionResult> Predict([FromBody] string query)
    {
      var culture = _userService.Culture;
      var sportResults = await _db.Sports.PredictWithQueryAsync<Sport, SportTranslation>(query, culture);
      var videoResults = await _db.Videos.PredictWithQueryAsync<Video, VideoTranslation>(query, culture);
      var playlistResults = await _db.Playlists.PredictWithQueryAsync<Playlist, PlaylistTranslation>(query, culture);
      var movieResults = await _db.Movies.PredictWithQueryAsync<Movie, MovieTranslation>(query, culture);
      
      var results = sportResults.Concat(playlistResults).Concat(videoResults).Concat(movieResults);
      return Ok(results);
    }
  }
}