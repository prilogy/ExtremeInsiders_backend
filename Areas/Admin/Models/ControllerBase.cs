﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace ExtremeInsiders.Areas.Admin.Models
{
    [ApiExplorerSettings(IgnoreApi = true)]
[Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public abstract class ControllerBase<T> : Controller
    where T : EntityBase
    {
        protected readonly ApplicationContext _context;

        public ControllerBase(ApplicationContext context)
        {
            _context = context;
        }

        // GET: <Entity>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Set<T>().ToListAsync());
        }

        // GET: <Entity>/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // GET: <Entity>/Create
        public virtual IActionResult Create(int baseId=default)
        {
            if (baseId != default)
                ViewData["BaseIdIsReadOnly"] = true; 
            return View();
        }

        // POST: <Entity>/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(T entity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit).ToString(), new {id = entity.Id });
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
            
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        // POST: <Entity>/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, T entity)
        {

            if (id != entity.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
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

            entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            
            return View(entity);
        }

        // GET: <Entity>/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Set<T>()
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
            var entity = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index).ToString());
        }

        private bool EntityExists(int id)
        {
            return _context.Set<T>().Any(e => e.Id == id);
        }
    }
}
