using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using Payment = Yandex.Checkout.V3.Payment;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppleNotificationController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserService _userService;
        private readonly PaymentService _paymentService;

        public AppleNotificationController(ApplicationContext db, UserService userService, PaymentService paymentService)
        {
            _db = db;
            _userService = userService;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Notification([FromBody] object body)
        {
            foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(body))
            {
                string name=descriptor.Name;
                object value=descriptor.GetValue(body);
                Console.WriteLine("{0}={1}",name,value);
            }
            return Ok();
        }

        private async Task SaleableEntityBuyHandle(Entities.Payment payment)
        {
            var user = payment.User;
            var entityId = int.Parse(payment.Metadata["entityId"]);

            var sale = new Sale
            {
                EntityId = entityId,
                UserId = user.Id
            };

            await _db.Sales.AddAsync(sale);
            await _db.SaveChangesAsync();
        }

        private async Task SubscriptionContinuationHandle(Entities.Payment payment)
        {
            var user = payment.User;
            var planId = int.Parse(payment.Metadata["planId"]);
            var plan = await _db.SubscriptionsPlans.FirstOrDefaultAsync(x => x.Id == planId);
            if (plan == null) return;

            var subscription = Subscription.Create(plan, user, payment);

            await _db.Subscriptions.AddAsync(subscription);
            await _db.SaveChangesAsync();
        }
    }
}