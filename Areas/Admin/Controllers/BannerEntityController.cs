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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class BannerEntityController : Controller
    {
        private readonly ApplicationContext _context;

        public BannerEntityController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BannerEntity
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BannerEntities;
            return View(await applicationContext.ToListAsync());
        }

        // GET: BannerEntity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerEntity = await _context.BannerEntities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bannerEntity == null)
            {
                return NotFound();
            }

            return View(bannerEntity);
        }

        // GET: BannerEntity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BannerEntity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EntityId")] BannerEntity bannerEntity)
        {
            if (ModelState.IsValid)
            {
                
                if (bannerEntity.EntityId != default && !_context.EntitiesBase.Any(x => x.Id == bannerEntity.EntityId))
                {
                    ModelState.AddModelError("", "Id связанного объекта не существует");
                    return View();
                }
                    
                _context.Add(bannerEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new {id = bannerEntity.Id});
            }
            return View(bannerEntity);
        }

        // GET: BannerEntity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerEntity = await _context.BannerEntities.FindAsync(id);
            if (bannerEntity == null)
            {
                return NotFound();
            }
            ViewData["EntityId"] = new SelectList(_context.EntitiesBase, "Id", "Id", bannerEntity.EntityId);
            return View(bannerEntity);
        }

        // POST: BannerEntity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EntityId")] BannerEntity bannerEntity)
        {
            if (id != bannerEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bannerEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerEntityExists(bannerEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EntityId"] = new SelectList(_context.EntitiesBase, "Id", "Id", bannerEntity.EntityId);
            return View(bannerEntity);
        }

        // GET: BannerEntity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerEntity = await _context.BannerEntities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bannerEntity == null)
            {
                return NotFound();
            }

            return View(bannerEntity);
        }

        // POST: BannerEntity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bannerEntity = await _context.BannerEntities.FindAsync(id);
            _context.BannerEntities.Remove(bannerEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerEntityExists(int id)
        {
            return _context.BannerEntities.Any(e => e.Id == id);
        }
    }
}
