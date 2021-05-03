using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
[Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class EntitySaleablePriceController : Controller
    {
        private readonly ApplicationContext _context;
        
        public IActionResult RedirectToBaseEntity(int id, string name) => RedirectToAction("Edit", name, new { Id = id });

        private async Task<IActionResult> RedirectToBaseById(int id)
        {
            var baseEntity = await _context.EntitiesBase.FindAsync(id);
            if(baseEntity == null)
                return RedirectToAction(nameof(Index));
            
            var controllerName = "";
            if (baseEntity is Playlist)
                controllerName = nameof(Playlist);
            else if (baseEntity is Video)
                controllerName = nameof(Video);
            else if (baseEntity is Movie)
                controllerName = nameof(Movie);

            if (controllerName == "")
                return RedirectToAction(nameof(Index));
                
            return RedirectToBaseEntity(id, controllerName);
        }
        
        public EntitySaleablePriceController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: EntitySaleablePrice
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.EntitySaleablePrices;
            return View(await applicationContext.ToListAsync());
        }

        // GET: EntitySaleablePrice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.EntitySaleablePrices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entitySaleablePrice == null)
            {
                return NotFound();
            }

            return View(entitySaleablePrice);
        }

        // GET: EntitySaleablePrice/Create
        public IActionResult Create(int baseEntityId=default, int currencyId=default)
        {
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", currencyId);
            ViewData["EntityId"] = new SelectList(_context.EntitiesSaleable, "Id", "Id", baseEntityId);
            if (baseEntityId != default)
                ViewData["BaseIdIsReadOnly"] = true;
            if(currencyId != default)
                ViewData["CurrencyIdIsReadOnly"] = true;
                
            return View();
        }

        // POST: EntitySaleablePrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntitySaleablePrice entitySaleablePrice)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(entitySaleablePrice);
                await _context.SaveChangesAsync();
                return await RedirectToBaseById(entitySaleablePrice.EntityId);
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.EntitiesSaleable, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // GET: EntitySaleablePrice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.EntitySaleablePrices.FindAsync(id);
            if (entitySaleablePrice == null)
            {
                return NotFound();
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.EntitiesSaleable, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // POST: EntitySaleablePrice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EntitySaleablePrice entitySaleablePrice)
        {
            if (id != entitySaleablePrice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entitySaleablePrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntitySaleablePriceExists(entitySaleablePrice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return await RedirectToBaseById(entitySaleablePrice.EntityId);
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.EntitiesSaleable, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // GET: EntitySaleablePrice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.EntitySaleablePrices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entitySaleablePrice == null)
            {
                return NotFound();
            }

            return View(entitySaleablePrice);
        }

        // POST: EntitySaleablePrice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entitySaleablePrice = await _context.EntitySaleablePrices.FindAsync(id);
            var baseId = entitySaleablePrice.EntityId;
            _context.EntitySaleablePrices.Remove(entitySaleablePrice);
            await _context.SaveChangesAsync();
            return await RedirectToBaseById(baseId);
        }

        private bool EntitySaleablePriceExists(int id)
        {
            return _context.EntitySaleablePrices.Any(e => e.Id == id);
        }
    }
}
