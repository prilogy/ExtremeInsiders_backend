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
  public class SubscriptionPlanTranslationController : Controller
  {
    private readonly ApplicationContext _context;
    
    public SubscriptionPlanTranslationController(ApplicationContext context)
    {
      _context = context;
    }
    public IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "SubscriptionPlan", new { Id = id });
    
    
        // GET: translation
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.SubscriptionsPlansTranslations;
            return View(await applicationContext.ToListAsync());
        }

        // GET: SportTranslation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.SubscriptionsPlansTranslations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // GET: SportTranslation/Create
        public IActionResult Create(int baseEntityId=default, int cultureId=default)
        {
            var list = new SelectList(_context.SubscriptionsPlans, "Id", "Id");
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
        public async Task<IActionResult> Create(SubscriptionPlanTranslation sportTranslation, [FromForm] IFormFile imageSrc)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

            var translation = await _context.SubscriptionsPlansTranslations.FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }
            ViewData["BaseEntityId"] = new SelectList(_context.Sports, "Id", "Discriminator", translation.BaseEntityId);
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Id", translation.CultureId);
            if(translation is ITranslationWithImage withImage)
                ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", withImage.ImageId);
            return View(translation);
        }

        // POST: SportTranslation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubscriptionPlanTranslation translation, [FromForm] IFormFile imageSrc)
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

            var translation = await _context.SubscriptionsPlansTranslations
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
            var translation = await _context.SubscriptionsPlansTranslations.FindAsync(id);
            var baseId = translation.BaseEntityId;
            _context.SubscriptionsPlansTranslations.Remove(translation);
            await _context.SaveChangesAsync();
            return RedirectToBaseEntity(baseId);
        }

        private bool TranslationExists(int id)
        {
            return _context.SubscriptionsPlansTranslations.Any(e => e.Id == id);
        }
  }
}