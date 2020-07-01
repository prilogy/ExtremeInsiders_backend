using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class VideoController : ControllerBase<Video>
  {
    public VideoController(ApplicationContext context) : base(context)
    {
    }

    private SelectList PlaylistIdSelectList(int id=default)
    {
      var newList = new List<object>();
      var playlists = _context.Playlists.ToList().OfCulture(Culture.Russian);
      foreach(var item in playlists)
        newList.Add( new { 
          Id = item.Id, 
          Name = item.Content != null ? item.Content.Name : $"Нет названия - Id: {item.Id}"
        } );

      return new SelectList(newList, "Id", "Name", id);
    }
    
    public override IActionResult Create(int playlistId=default)
    {
      ViewData["PlaylistId"] = PlaylistIdSelectList(playlistId);
      return base.Create(playlistId);
    }

    public override Task<IActionResult> Edit(int? id)
    {
      ViewData["PlaylistId"] = PlaylistIdSelectList(_context.Videos.Find(id).PlaylistId);
      return base.Edit(id);
    }
  }
}