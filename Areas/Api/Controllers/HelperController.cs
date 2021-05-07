using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using Payment = Yandex.Checkout.V3.Payment;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HelperController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserService _userService;
        private readonly KassaPaymentService _kassaPaymentService;

        public HelperController(ApplicationContext db, UserService userService, KassaPaymentService kassaPaymentService)
        {
            _db = db;
            _userService = userService;
            _kassaPaymentService = kassaPaymentService;
        }

        [HttpGet]
        public async Task<ActionResult<BannerEntity>> Banner()
        {
            var list = (await _db.BannerEntities.ToListAsync()).OfCulture(_userService.Culture);
            foreach (var item in list)
            {
                if (item.Entity != null)
                {
                    if (item.Entity is Sport s)
                        s = s.OfCulture(_userService.Culture);
                    else if (item.Entity is Playlist p)
                        p = p.OfCulture(_userService.Culture);
                    else if (item.Entity is Video v)
                        v = v.OfCulture(_userService.Culture);
                    else if (item.Entity is Movie m)
                        m = m.OfCulture(_userService.Culture);
                }
            }

            return Ok((await _db.BannerEntities.ToListAsync()).OfCulture(_userService.Culture));
        }
        
        [HttpPost]
        public async Task<IActionResult> CheckStatus([FromBody] string url)
            => await _kassaPaymentService.CheckStatusAsync(url) switch
            {
                PaymentStatus.Succeeded => Ok(),
                PaymentStatus.Canceled => BadRequest(),
                PaymentStatus.WaitingForCapture => StatusCode(StatusCodes.Status426UpgradeRequired),
                PaymentStatus.Pending => StatusCode(StatusCodes.Status426UpgradeRequired),
                _ => NotFound()
            };
    }
}