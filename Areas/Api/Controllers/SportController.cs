using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SportController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UserService _userService;

        public SportController(ApplicationContext db, UserService userService)
        {
            _db = db;
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sport>>> GetSports()
        {
            return (await _db.Sports.ToListAsync()).OfCulture(_userService.Culture);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSport(int id)
        {
            var sport = await _db.Sports.FindAsync(id);

            if (sport == null)
            {
                return NotFound();
            }

            return Ok(sport.OfCulture(_userService.Culture));
        }
    }
}
