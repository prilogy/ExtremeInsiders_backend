using System;
using System.IO;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Enums;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using Payment = ExtremeInsiders.Entities.Payment;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly SaleService _saleService;
        private readonly KassaPaymentService _kassaPaymentService;

        public PaymentController(KassaPaymentService kassaPaymentService, SaleService saleService)
        {
            _kassaPaymentService = kassaPaymentService;
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<IActionResult> Notification()
        {
            var message = Client.ParseMessage(Request.Method, Request.ContentType,
                await new StreamReader(HttpContext.Request.Body).ReadToEndAsync());
            if (message == null) return BadRequest();

            var payment = message.Object;

            var r = true;
            if (message.Event == Event.PaymentWaitingForCapture && payment.Paid)
            {
                var type = PaymentTypesExtensions.FromString(payment.Metadata[Payment.TypeMetadataName]);
                if (type == null) return BadRequest();
                
                var dbPayment = await _kassaPaymentService.CaptureAsync(payment);
                
                r = type switch
                {
                    PaymentTypes.SubscriptionContinuation => await _saleService.SubscriptionContinuationHandle(dbPayment.User,
                        int.TryParse(payment.Metadata["planId"], out var x) ? (int?) x : null, dbPayment.Id),
                    PaymentTypes.SaleableEntityBuy => await _saleService.SaleableEntityBuyHandle(dbPayment.User,
                        int.TryParse(payment.Metadata["entityId"], out var x) ? (int?) x : null, dbPayment.Id),
                    _ => false
                };
            }

            if (r) return Ok();
            return BadRequest();
        }
    }
}