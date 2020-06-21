using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class SportTranslationController : Controller
    {
        private RedirectToRouteResult RedirectToSport(int id)
        {
           return RedirectToRoute( new RouteValueDictionary(new{ area="Admin", controller = "Sport", action="Edit", id}));
        }
        
        private readonly ApplicationContext _context;

        public SportTranslationController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: SportTranslation
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.SportsTranslations.Include(s => s.Culture).Include(s => s.Sport);
            return View(await applicationContext.ToListAsync());
        }

        // GET: SportTranslation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportTranslation = await _context.SportsTranslations
                .Include(s => s.Culture)
                .Include(s => s.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportTranslation == null)
            {
                return NotFound();
            }

            return View(sportTranslation);
        }

        // GET: SportTranslation/Create
        public IActionResult Create([FromRoute]int? id)
        {
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Key");
            var sportIds = new SelectList(_context.Sports, "Id", "Id");
            var toBeSelected = sportIds.SingleOrDefault(i => i.Value == id.ToString());
            if (toBeSelected != null)
                toBeSelected.Selected = true;
            else ModelState.AddModelError("", $"Id {id} не найден");
            ViewData["SportId"] = sportIds;
            return View();
        }

        // POST: SportTranslation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SportId,CultureId")] SportTranslation sportTranslation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new SportTranslation
                {
                    CultureId = sportTranslation.CultureId,
                    SportId = sportTranslation.SportId,
                    Name = sportTranslation.Name
                });
                await _context.SaveChangesAsync();
                return RedirectToSport(sportTranslation.SportId);
            }
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Key", sportTranslation.CultureId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", sportTranslation.SportId);
            return View(sportTranslation);
        }

        // GET: SportTranslation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportTranslation = await _context.SportsTranslations.FindAsync(id);
            if (sportTranslation == null)
            {
                return NotFound();
            }
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Id", sportTranslation.CultureId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", sportTranslation.SportId);
            return View(sportTranslation);
        }

        // POST: SportTranslation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SportId,CultureId")] SportTranslation sportTranslation)
        {
            if (id != sportTranslation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportTranslation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportTranslationExists(sportTranslation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToSport(sportTranslation.SportId);
            }
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Id", sportTranslation.CultureId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", sportTranslation.SportId);
            return View(sportTranslation);
        }

        // GET: SportTranslation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportTranslation = await _context.SportsTranslations
                .Include(s => s.Culture)
                .Include(s => s.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportTranslation == null)
            {
                return NotFound();
            }

            return View(sportTranslation);
        }

        // POST: SportTranslation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sportTranslation = await _context.SportsTranslations.FindAsync(id);
            var sportId = sportTranslation.SportId;
            _context.SportsTranslations.Remove(sportTranslation);
            await _context.SaveChangesAsync();
            return RedirectToSport(sportId);
        }

        private bool SportTranslationExists(int id)
        {
            return _context.SportsTranslations.Any(e => e.Id == id);
        }
    }
}
