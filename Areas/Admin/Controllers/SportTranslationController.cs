using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class SportTranslationController : ControllerTranslationBase<Sport, SportTranslation>
  {
    public SportTranslationController(ApplicationContext context, ImageService imageService) : base(context, imageService)
    {
    }
    public override IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "Sport", new { Id = id });
  }
}