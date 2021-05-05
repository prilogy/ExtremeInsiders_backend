using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using Payment = Yandex.Checkout.V3.Payment;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AppleNotificationController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplePaymentService _applePaymentService;

        public AppleNotificationController(UserService userService, ApplePaymentService applePaymentService)
        {
            _userService = userService;
            _applePaymentService = applePaymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Payment([FromBody] ApplePayment payment)
        {
            var r = await _applePaymentService.HandleAsync(payment, _userService.User);
            if (r) return Ok();
            return BadRequest();
        }
    }
}