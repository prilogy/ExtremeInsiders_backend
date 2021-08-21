using System;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    public class NotificationController : ControllerBase<UserNotification>
    {
        private readonly FcmService _fcmService;

        public NotificationController(ApplicationContext context, FcmService fcmService) :
            base(context)
        {
            _fcmService = fcmService;
        }

        // POST: <Entity>/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create(UserNotification entity)
        {
            var sendNow = entity.DateCreated == null;
            if (ModelState.IsValid)
            {
                entity.DateCreated ??= DateTime.UtcNow;
                _context.Add(entity);
                await _context.SaveChangesAsync();

                if (sendNow) await _fcmService.SendNotificationAsync(entity);

                return RedirectToAction(nameof(Edit).ToString(), new { id = entity.Id });
            }

            return View(entity);
        }
    }
}