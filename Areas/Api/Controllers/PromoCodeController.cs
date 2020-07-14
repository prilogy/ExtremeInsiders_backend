using System;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class PromoCodeController : Controller
  {
    private readonly UserService _userService;
    private readonly ApplicationContext _db;

    public PromoCodeController(UserService userService, ApplicationContext db)
    {
      _userService = userService;
      _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> GetInfo([FromBody] string code)
    {
      var promoCode = await _db.PromoCodes.FirstOrDefaultAsync(x => x.Code == code);
      if (promoCode == null || !promoCode.IsValid)
        return NotFound();

      if (_userService.User.PromoCodes.Any(x => x.PromoCodeId == promoCode.Id))
        return NotFound();

      if (promoCode.SubscriptionPlan != null)
        promoCode.SubscriptionPlan = promoCode.SubscriptionPlan.OfFormat(_userService);
      
      if (promoCode.EntitySaleable != null)
      {
        promoCode.EntitySaleable = promoCode.EntitySaleable switch
        {
          Playlist p => p.OfFormat(_userService),
          Video v => v.OfFormat(_userService),
          Movie m => m.OfFormat(_userService),
          _ => promoCode.EntitySaleable
        };
      }
      return Ok(promoCode);
    }

    [HttpPut]
    public async Task<IActionResult> Confirm([FromBody] string code)
    {
      var promoCode = await _db.PromoCodes.FirstOrDefaultAsync(x => x.Code == code);
      if (promoCode == null)
        return NotFound();
      
      if (_userService.User.PromoCodes.Any(x => x.PromoCodeId == promoCode.Id))
        return NotFound();

      if (promoCode.EntitySaleable != null && _userService.User.Sales.All(x => x.EntityId != promoCode.EntitySaleableId))
      {
        var sale = new Sale
          {
            EntityId = promoCode.EntitySaleable.Id,
            UserId = _userService.UserId
          };
          await _db.Sales.AddAsync(sale);
      }

      if (promoCode.SubscriptionPlan != null)
      {
        var subscription  = Subscription.Create(promoCode.SubscriptionPlan, _userService.User);

        await _db.Subscriptions.AddAsync(subscription);
      }

      if (!promoCode.IsInfinite)
        promoCode.IsValid = false;
      
      var promoCodeUser = new PromoCodeUser
      {
        PromoCodeId = promoCode.Id,
        UserId = _userService.UserId
      };

      await _db.PromoCodesUsers.AddAsync(promoCodeUser);
      
      await _db.SaveChangesAsync();

      return Ok();
    }
  }
}