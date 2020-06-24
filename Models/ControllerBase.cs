using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Models
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ControllerBase<T, TR> : Controller
  where T: EntityBase, ITranslatableEntity<T, TR>
  where TR: TranslatableEntityTranslation<T>
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;

    public ControllerBase(ApplicationContext db, UserService userService)
    {
      _db = db;
      _userService = userService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAll()
    {
      return (await _db.Set<T>().ToListAsync()).OfCulture(_userService.Culture);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<T>>> GetByIds(int[] ids)
    {
      return Ok((await _db.Set<Sport>().Where(x => ids.Contains(x.Id)).ToListAsync()).OfCulture(_userService.Culture));
    }
        
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetById(int id)
    {
      var entity = await _db.Set<T>().FindAsync(id);

      if (entity == null)
      {
        return NotFound();
      }

      return entity.OfCulture(_userService.Culture);
    }
  }
}