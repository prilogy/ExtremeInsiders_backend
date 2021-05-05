using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class BannerEntityTranslationController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ImageService _imageService;
        
        public BannerEntityTranslationController(ApplicationContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "BannerEntity", new { Id = id });


        // GET: translation
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BannerEntitiesTranslations;
            return View(await applicationContext.ToListAsync());
        }

        // GET: SportTranslation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.BannerEntitiesTranslations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // GET: SportTranslation/Create
        public IActionResult Create(int baseEntityId = default, int cultureId = default)
        {
            var list = new SelectList(_context.BannerEntities, "Id", "Id");
            if (baseEntityId != default)
            {
                var item = list.FirstOrDefault(x => x.Value == baseEntityId.ToString());
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

            ViewData["BaseEntityId"] = list;
            ViewData["CultureId"] = cultureList;
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");
            return View();
        }

        // POST: SportTranslation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerEntityTranslation translation, [FromForm] IFormFile imageSrc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageSrc != null)
                    {
                        var image = await _imageService.AddImage(imageSrc);
                        translation.ImageId = image.Id;
                    }

                    _context.Add(translation);
                    await _context.SaveChangesAsync();
                    return RedirectToBaseEntity(translation.BaseEntityId);
                }
                catch
                {
                    ModelState.AddModelError("", "Такое сочетание ключей уже существует.");
                    return Create();
                }
            }

            return View(translation);
        }

        // GET: SportTranslation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.BannerEntitiesTranslations.FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            ViewData["BaseEntityId"] = new SelectList(_context.Sports, "Id", "Discriminator", translation.BaseEntityId);
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Id", translation.CultureId);
            if (translation is ITranslationWithImage withImage)
                ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", withImage.ImageId);
            return View(translation);
        }

        // POST: SportTranslation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BannerEntityTranslation translation,
            [FromForm] IFormFile imageSrc)
        {
            if (id != translation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(translation);
                    await _context.SaveChangesAsync();
                    
                    if (imageSrc != null)
                    {
                        var image = await _imageService.AddImage(imageSrc);
                        translation.ImageId = image.Id;
                    }           
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TranslationExists(translation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToBaseEntity(translation.BaseEntityId);
            }

            return View(translation);
        }

        // GET: SportTranslation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.BannerEntitiesTranslations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // POST: SportTranslation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var translation = await _context.BannerEntitiesTranslations.FindAsync(id);
            var baseId = translation.BaseEntityId;
            _context.BannerEntitiesTranslations.Remove(translation);
            await _context.SaveChangesAsync();
            return RedirectToBaseEntity(baseId);
        }

        private bool TranslationExists(int id)
        {
            return _context.BannerEntitiesTranslations.Any(e => e.Id == id);
        }
    }
}