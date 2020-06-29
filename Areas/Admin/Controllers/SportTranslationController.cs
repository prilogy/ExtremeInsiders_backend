using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class SportTranslationController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ImageService _imageService;

        public SportTranslationController(ApplicationContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        private IActionResult RedirectToBaseEntity(int id) =>
            RedirectToAction("Edit", "Sport", new { Id = id });
        
        // GET: SportTranslation
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.SportsTranslations.Include(s => s.BaseEntity).Include(s => s.Culture).Include(s => s.Image);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportTranslation == null)
            {
                return NotFound();
            }

            return View(sportTranslation);
        }

        // GET: SportTranslation/Create
        public IActionResult Create(int baseEntityId=default, int cultureId=default)
        {
            var sportList = new SelectList(_context.Sports, "Id", "Id");
            if (baseEntityId != default)
            {
                var item = sportList.FirstOrDefault(x => x.Value == baseEntityId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                    ViewData["BaseEntityIsReadOnly"] = true;
                }
                else ModelState.AddModelError("", "Id базового представления не существует.");
            }
            
            var cultureList = new SelectList(_context.Cultures, "Id", "Key");
            if (cultureId != default)
            {
                var item = cultureList.FirstOrDefault(x => x.Value == cultureId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                    ViewData["CultureIdIsReadOnly"] = true;
                }
                else ModelState.AddModelError("", "Id культуры представления не существует.");
            }

            ViewData["BaseEntityId"] = sportList;
            ViewData["CultureId"] = cultureList;
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");
            return View();
        }

        // POST: SportTranslation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImageId,Id,BaseEntityId,CultureId")] SportTranslation sportTranslation, [FromForm] IFormFile imageSrc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageSrc != null)
                    {
                        var image = await _imageService.AddImage(imageSrc);
                        sportTranslation.ImageId = image.Id;
                    }
                    _context.Add(sportTranslation);
                    await _context.SaveChangesAsync();
                    return RedirectToBaseEntity(sportTranslation.BaseEntityId);
                }
                catch
                {
                    ModelState.AddModelError("", "Такое сочетание ключей уже существует.");
                    return Create();
                }
            }

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
            ViewData["BaseEntityId"] = new SelectList(_context.Sports, "Id", "Discriminator", sportTranslation.BaseEntityId);
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Id", sportTranslation.CultureId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", sportTranslation.ImageId);
            return View(sportTranslation);
        }

        // POST: SportTranslation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,ImageId,Id,BaseEntityId,CultureId")] SportTranslation sportTranslation, [FromForm] IFormFile imageSrc)
        {
            if (id != sportTranslation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageSrc != null)
                    {
                        var image = await _imageService.AddImage(imageSrc);
                        sportTranslation.ImageId = image.Id;
                    }
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
                return RedirectToBaseEntity(sportTranslation.BaseEntityId);
            }

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
            var baseId = sportTranslation.BaseEntityId;
            _context.SportsTranslations.Remove(sportTranslation);
            await _context.SaveChangesAsync();
            return RedirectToBaseEntity(baseId);
        }

        private bool SportTranslationExists(int id)
        {
            return _context.SportsTranslations.Any(e => e.Id == id);
        }
    }
}
