using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Role.AdminRole)]
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Users.Include(u => u.Avatar).Include(u => u.Culture).Include(u => u.Currency).Include(u => u.Role);
            return View(await applicationContext.ToListAsync());
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Key", user.CultureId);
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", user.CurrencyId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Name,Password,AvatarId,DateBirthday,DateSignUp,PhoneNumber,RoleId,CultureId,CurrencyId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["CultureId"] = new SelectList(_context.Cultures, "Id", "Key", user.CultureId);
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "Id", "Key", user.CurrencyId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Avatar)
                .Include(u => u.Culture)
                .Include(u => u.Currency)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // POST: User/DeleteSubscription/5
        [HttpGet]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            var userId = subscription.UserId;
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new {Id = userId});
        }
        
        [HttpPost]
        public async Task<IActionResult> AddSubscription(int userId, int planId)
        {
            var plan = await _context.SubscriptionsPlans.FindAsync(planId);
            var user = await _context.Users.FindAsync(userId);
            if (plan == null || user == null)
            {
                ModelState.AddModelError("", "Ошибка при создании подписки");
                return RedirectToAction(nameof(Index));
            }
            
            var subscription = new Subscription
            {
                PlanId = plan.Id,
                UserId = user.Id,
                DateStart = DateTime.UtcNow,
                PaymentId = null
            };
      
            if (user.Subscription == null)
                subscription.DateEnd = DateTime.UtcNow + plan.Duration;
            else
            {
                subscription.DateStart = user.Subscription.DateEnd;
                subscription.DateEnd = user.Subscription.DateEnd + plan.Duration;
            }

            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new {Id = user.Id});
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
