using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class PlaylistTranslationController : ControllerTranslationBase<Playlist, PlaylistTranslation>
  {
    public PlaylistTranslationController(ApplicationContext context, ImageService imageService) : base(context, imageService)
    {
    }
    public override IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "Playlist", new { Id = id });
  }
}