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
  public class MovieController : ControllerBase<Movie>
  {
    public MovieController(ApplicationContext context) : base(context)
    {
    }

    private SelectList SportIdSelectList(int id=default)
    {
      var newList = new List<object>();
      var sports = _context.Sports.ToList().OfCulture(Culture.Russian);
      foreach(var item in sports)
        newList.Add( new { 
          Id = item.Id, 
          Name = item.Content != null ? item.Content.Name : $"Нет названия - Id: {item.Id}"
        } );

      return new SelectList(newList, "Id", "Name", id);
    }
    
    public override IActionResult Create(int sportId=default)
    {
      ViewData["SportId"] = SportIdSelectList(sportId);
      return base.Create(sportId);
    }

    public override Task<IActionResult> Edit(int? id)
    {
      ViewData["SportId"] = SportIdSelectList(_context.Movies.Find(id).SportId);
      return base.Edit(id);
    }
  }
}