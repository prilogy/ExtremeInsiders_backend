using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
[Area("Admin")]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
  public class SubscriptionPlanController : Controller
  {
    protected readonly ApplicationContext _context;

    public SubscriptionPlanController(ApplicationContext context)
    {
      _context = context;
    }

    // GET: <Entity>
    public async Task<IActionResult> Index()
    {
      return View(await _context.SubscriptionsPlans.ToListAsync());
    }

    // GET: <Entity>/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var entity = await _context.SubscriptionsPlans
        .FirstOrDefaultAsync(m => m.Id == id);
      if (entity == null)
      {
        return NotFound();
      }

      return View(entity);
    }

    // GET: <Entity>/Create
    public virtual IActionResult Create()
    {
      return View();
    }

    // POST: <Entity>/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriptionPlan entity, [Required] int durationInDays)
    {
      if (ModelState.IsValid)
      {
        entity.Duration = TimeSpan.FromDays(durationInDays);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Edit).ToString(), new {id = entity.Id});
      }

      return View(entity);
    }

    // GET: <Entity>/Edit/5
    public virtual async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var entity = await _context.SubscriptionsPlans.FindAsync(id);
      if (entity == null)
      {
        return NotFound();
      }

      return View(entity);
    }

    // POST: <Entity>/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubscriptionPlan entity, int durationInDays = default)
    {
      if (id != entity.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          if (durationInDays != default)
            entity.Duration = TimeSpan.FromDays(durationInDays);
          _context.Update(entity);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!EntityExists(entity.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }

        return RedirectToAction(nameof(Index).ToString());
      }

      return View(entity);
    }

    // GET: <Entity>/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var entity = await _context.SubscriptionsPlans
        .FirstOrDefaultAsync(m => m.Id == id);
      if (entity == null)
      {
        return NotFound();
      }

      return View(entity);
    }

    // POST: <Entity>/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var entity = await _context.SubscriptionsPlans.FindAsync(id);
      _context.SubscriptionsPlans.Remove(entity);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index).ToString());
    }

    private bool EntityExists(int id)
    {
      return _context.SubscriptionsPlans.Any(e => e.Id == id);
    }
  }
}