using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class SportController : Controller
  {
    private readonly ApplicationContext _context;

    public SportController(ApplicationContext context)
    {
      _context = context;
    }

    // GET: Sport
    public async Task<IActionResult> Index()
    {
      return View(await _context.Sports.ToListAsync());
    }

    // GET: Sport/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var sport = await _context.Sports
        .FirstOrDefaultAsync(m => m.Id == id);
      if (sport == null)
      {
        return NotFound();
      }

      return View(sport);
    }

    // GET: Sport/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Sport/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sport sport)
    {
      if (!ModelState.IsValid) return View(sport);

      _context.Add(sport);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Edit), new {id = sport.Id});
    }

    // GET: Sport/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var sport = await _context.Sports.FindAsync(id);
      if (sport == null)
      {
        return NotFound();
      }

      return View(sport);
    }

    // POST: Sport/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreated")] Sport sport)
    {
      if (id != sport.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(sport);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SportExists(sport.Id))
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

      return View(sport);
    }

    // GET: Sport/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var sport = await _context.Sports
        .FirstOrDefaultAsync(m => m.Id == id);
      if (sport == null)
      {
        return NotFound();
      }

      return View(sport);
    }

    // POST: Sport/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var sport = await _context.Sports.FindAsync(id);
      _context.Sports.Remove(sport);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool SportExists(int id)
    {
      return _context.Sports.Any(e => e.Id == id);
    }
  }
}