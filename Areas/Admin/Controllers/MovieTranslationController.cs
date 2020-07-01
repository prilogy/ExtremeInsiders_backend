using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class MovieTranslationController : ControllerTranslationBase<Movie, MovieTranslation>
  {
    public MovieTranslationController(ApplicationContext context, ImageService imageService) : base(context, imageService)
    {
    }
    public override IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "Movie", new { Id = id });
  }
}