using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class PlaylistController : ControllerBase<Playlist>
  {
    public PlaylistController(ApplicationContext context) : base(context)
    {
    }
  }
}