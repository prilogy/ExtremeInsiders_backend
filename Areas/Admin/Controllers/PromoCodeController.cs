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
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
  public class PromoCodeController : Controller
  {
    private readonly ApplicationContext _context;

    public PromoCodeController(ApplicationContext context)
    {
      _context = context;
    }

    // GET: PromoCode
    public async Task<IActionResult> Index()
    {
      var applicationContext = _context.PromoCodes.Include(p => p.EntitySaleable).Include(p => p.SubscriptionPlan);
      return View(await applicationContext.ToListAsync());
    }

    // GET: PromoCode/Create
    public IActionResult Create()
    {
      ViewData["EntitySaleableId"] = SaleableSelectList();
      ViewData["SubscriptionPlanId"] = SubscriptionPlanSelectList();
      return View();
    }

    // POST: PromoCode/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Code,SubscriptionPlanId,EntitySaleableId,IsInfinite,IsValid")]
      PromoCode promoCode)
    {
      if (ModelState.IsValid)
      {
        promoCode = ValidateIds(promoCode);
        
        _context.Add(promoCode);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      ViewData["EntitySaleableId"] = SaleableSelectList(promoCode.EntitySaleableId ?? 0);
      ViewData["SubscriptionPlanId"] = SubscriptionPlanSelectList(promoCode.SubscriptionPlanId ?? 0);
      return View(promoCode);
    }
    

    // GET: PromoCode/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var promoCode = await _context.PromoCodes.FindAsync(id);
      if (promoCode == null)
      {
        return NotFound();
      }

      ViewData["EntitySaleableId"] = SaleableSelectList(promoCode.EntitySaleableId ?? 0);
      ViewData["SubscriptionPlanId"] = SubscriptionPlanSelectList(promoCode.SubscriptionPlanId ?? 0);
      return View(promoCode);
    }

    // POST: PromoCode/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
      [Bind("Id,Code,SubscriptionPlanId,EntitySaleableId,IsInfinite,IsValid")]
      PromoCode promoCode)
    {
      if (id != promoCode.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          promoCode = ValidateIds(promoCode);
          _context.Update(promoCode);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!PromoCodeExists(promoCode.Id))
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

      ViewData["EntitySaleableId"] = SaleableSelectList(promoCode.EntitySaleableId ?? 0);
      ViewData["SubscriptionPlanId"] = SubscriptionPlanSelectList(promoCode.SubscriptionPlanId ?? 0);
      return View(promoCode);
    }

    // GET: PromoCode/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var promoCode = await _context.PromoCodes
        .FirstOrDefaultAsync(m => m.Id == id);
      if (promoCode == null)
      {
        return NotFound();
      }

      return View(promoCode);
    }

    // POST: PromoCode/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var promoCode = await _context.PromoCodes.FindAsync(id);
      _context.PromoCodes.Remove(promoCode);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool PromoCodeExists(int id)
    {
      return _context.PromoCodes.Any(e => e.Id == id);
    }

    private PromoCode ValidateIds(PromoCode p)
    {
      if (p.SubscriptionPlanId == 0)
        p.SubscriptionPlanId = default;
      if (p.EntitySaleableId == 0)
        p.EntitySaleableId = default;

      return p;
    }
    
    private SelectList SubscriptionPlanSelectList(int id = default)
    {
      var newList = new List<object> {new {Id = 0, Name = "-"}};
      var sports = _context.SubscriptionsPlans.ToList().OfCulture(Culture.Russian);
      foreach (var item in sports)
        newList.Add(new
        {
          Id = item.Id,
          Name = item.Content != null ? item.Content.Name : $"Нет названия - Id: {item.Id}"
        });

      return new SelectList(newList, "Id", "Name", id);
    }

    private SelectList SaleableSelectList(int id = default)
    {
      string GetEntityName(EntitySaleable entity)
      {
        var type = "";
        var name = "";
        switch (entity)
        {
          case Playlist p:
            type = "Плейлист";
            name = p.OfCulture(Culture.Russian).Content?.Name;
            break;
          case Video v:
            type = "Видео";
            name = v.OfCulture(Culture.Russian).Content?.Name;
            break;
          case Movie m:
            type = "Фильм";
            name = m.OfCulture(Culture.Russian).Content?.Name;
            break;
        }

        if (string.IsNullOrEmpty(name))
          name = "Нет названия";
        return $"{type} - \"{name}\", Id - {entity.Id}";
      }


      var newList = new List<object> {new {Id = 0, Name = "-"}};

      var list = _context.EntitiesSaleable.Where(x => x.Prices.Count > 0).ToList();

      foreach (var item in list)
        newList.Add(new
        {
          Id = item.Id,
          Name = GetEntityName(item)
        });

      return new SelectList(newList, "Id", "Name", id);
    }
  }
}