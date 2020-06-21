using System;
using System.Linq;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class SportController : Controller
  {
    private readonly UserService _userService;
    private readonly ApplicationContext _db;

    public SportController(UserService userService, ApplicationContext db)
    {
      _userService = userService;
      _db = db;
    }

    [HttpGet]
    public ActionResult<Sport> Get()
    {
      return _db.Sports.First().OfCulture(_userService.Culture);
    }
  }
}