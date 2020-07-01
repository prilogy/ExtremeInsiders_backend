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
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class SubscriptionPlanPriceController : Controller
    {
        private readonly ApplicationContext _context;
        
        public SubscriptionPlanPriceController(ApplicationContext context)
        {
            _context = context;
        }
        
        public IActionResult RedirectToBaseEntity(int id) => RedirectToAction("Edit", "SubscriptionPlan", new { Id = id });


        // GET: EntitySaleablePrice
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.SubscriptionsPlansPrices;
            return View(await applicationContext.ToListAsync());
        }

        // GET: EntitySaleablePrice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.SubscriptionsPlansPrices
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
            ViewData["EntityId"] = new SelectList(_context.SubscriptionsPlans, "Id", "Id", baseEntityId);
            if (baseEntityId != default)
                ViewData["BaseIdIsReadOnly"] = true;
            if(currencyId != default)
                ViewData["CurrencyIdIsReadOnly"] = true;
                
            return View();
        }

        // POST: EntitySaleablePrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlanPrice entitySaleablePrice)
        {
            if (ModelState.IsValid)
            {
                
                _context.SubscriptionsPlansPrices.Add(entitySaleablePrice);
                await _context.SaveChangesAsync();
                return RedirectToBaseEntity(entitySaleablePrice.EntityId);
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.SubscriptionsPlans, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // GET: EntitySaleablePrice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.SubscriptionsPlansPrices .FindAsync(id);
            if (entitySaleablePrice == null)
            {
                return NotFound();
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.SubscriptionsPlans, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // POST: EntitySaleablePrice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubscriptionPlanPrice entitySaleablePrice)
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
                return RedirectToBaseEntity(entitySaleablePrice.EntityId);
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", entitySaleablePrice.CurrencyId);
            ViewData["EntityId"] = new SelectList(_context.SubscriptionsPlans, "Id", "Id", entitySaleablePrice.EntityId);
            return View(entitySaleablePrice);
        }

        // GET: EntitySaleablePrice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entitySaleablePrice = await _context.SubscriptionsPlansPrices 
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
            var entitySaleablePrice = await _context.SubscriptionsPlansPrices .FindAsync(id);
            var baseId = entitySaleablePrice.EntityId;
            _context.SubscriptionsPlansPrices .Remove(entitySaleablePrice);
            await _context.SaveChangesAsync();
            return RedirectToBaseEntity(baseId);
        }

        private bool EntitySaleablePriceExists(int id)
        {
            return _context.SubscriptionsPlansPrices.Any(e => e.Id == id);
        }
    }
}
