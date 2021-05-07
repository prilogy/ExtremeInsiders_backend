using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Interfaces;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ExtremeInsiders.Areas.Admin.Models
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public abstract class ControllerTranslationBase<T, TR> : Controller
        where T : EntityBase, ITranslatableEntity<T, TR>
        where TR : TranslatableEntityTranslation<T>
    {
        private readonly ApplicationContext _context;
        private readonly ImageService _imageService;

        public ControllerTranslationBase(ApplicationContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public abstract IActionResult RedirectToBaseEntity(int id);
        // RedirectToAction("Edit", "Sport", new { Id = id });

        // GET: translation
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Set<TR>();
            return View(await applicationContext.ToListAsync());
        }

        // GET: SportTranslation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.Set<TR>()
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
            var list = new SelectList(_context.Set<T>(), "Id", "Id");
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
        public async Task<IActionResult> Create(TR translation, [FromForm] IFormFile imageSrc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageSrc != null && translation is ITranslationWithImage withImage)
                    {
                        var image = await _imageService.AddImage(imageSrc);
                        withImage.ImageId = image.Id;
                    }

                    await CalcDuration(translation);

                    _context.Add(translation);
                    await _context.SaveChangesAsync();

                    if (ModelState.ErrorCount > 0)
                        return await Edit(translation.Id);

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

            var translation = await _context.Set<TR>().FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            ViewData["BaseEntityId"] = new SelectList(_context.Set<T>(), "Id", "Id", translation.BaseEntityId);
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
        public async Task<IActionResult> Edit(int id, TR translation, [FromForm] IFormFile imageSrc)
        {
            if (id != translation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (translation is ITranslationWithImage withImage)
                    {
                        if (imageSrc != null)
                        {
                            var image = await _imageService.AddImage(imageSrc);
                            withImage.ImageId = image.Id;
                        }
                        else
                        {
                            if (await _context.Set<TR>().FindAsync(id) is ITranslationWithImage current)
                            {
                                var imageId = current.ImageId;
                                withImage.ImageId = imageId;
                                _context.Entry(current).State = EntityState.Detached;
                            }
                        }
                    }

                    await CalcDuration(translation);

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
                
                if (ModelState.ErrorCount > 0)
                    return await Edit(translation.Id);
                
                return RedirectToBaseEntity(translation.BaseEntityId);
            }

            return View(translation);
        }

        private async Task CalcDuration(TR translation)
        {
            if (translation is IWithUrlAndDuration { Url: { } } withVimeo)
            {
                var vimeoVideo = await VimeoHelpers.GetVideoAsync(withVimeo.Url);
                if (vimeoVideo == null)
                {
                    withVimeo.Duration = null;
                    ModelState.AddModelError("", "Url некорректна или видео не найдено ");
                }
                else withVimeo.Duration = vimeoVideo.Video?.DurationFmt;
            }
        }

        // GET: SportTranslation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _context.Set<TR>()
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
            var translation = await _context.Set<TR>().FindAsync(id);
            var baseId = translation.BaseEntityId;
            _context.Set<TR>().Remove(translation);
            await _context.SaveChangesAsync();
            return RedirectToBaseEntity(baseId);
        }

        private bool TranslationExists(int id)
        {
            return _context.Set<TR>().Any(e => e.Id == id);
        }
    }
}