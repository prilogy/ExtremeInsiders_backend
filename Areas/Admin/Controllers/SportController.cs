using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  public class SportController : ControllerBase<Sport>
  {
    public SportController(ApplicationContext context) : base(context)
    {
    }
  }
}