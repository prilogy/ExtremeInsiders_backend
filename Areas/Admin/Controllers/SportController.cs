using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
  public class SportController : Controller
  {
    private readonly ApplicationContext _context;
    private readonly ImageService _imageService;

    public SportController(ApplicationContext context, ImageService imageService)
    {
      _context = context;
      _imageService = imageService;
    }

    // GET: Sport
    public async Task<IActionResult> Index()
    {
      return View(await _context.Sports.Where(s => s.Image != null).ToListAsync());
    }

    // GET: Sport/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var sport = await _context.Sports
        .Include(s => s.Image)
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
      ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");
      return View();
    }

    // POST: Sport/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sport sport, [FromForm, Required]IFormFile imageSrc)
    {
      if (ModelState.IsValid)
      {
        var image = await _imageService.AddImage(imageSrc);
        sport.Image = image;
        _context.Add(sport);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", sport.ImageId);
      return View(sport);
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

      ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", sport.ImageId);
      return View(sport);
    }

    // POST: Sport/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sport sport, [FromForm] IFormFile imageSrc)
    {
      if (id != sport.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          if (imageSrc != null)
          {
            sport.Image = await _imageService.AddImage(imageSrc);
          }
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

      ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", sport.ImageId);
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
        .Include(s => s.Image)
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