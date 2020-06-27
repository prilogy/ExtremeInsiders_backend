using System;
using System.IO;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using Message = Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging.Message;
using Payment = ExtremeInsiders.Entities.Payment;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class PaymentController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly PaymentService _paymentService;

    public PaymentController(ApplicationContext db, UserService userService, PaymentService paymentService)
    {
      _db = db;
      _userService = userService;
      _paymentService = paymentService;
    }

    public async Task<IActionResult> Notification()
    {
      Console.WriteLine("sssssssssssss");
      var message = Client.ParseMessage(Request.Method, Request.ContentType, await new StreamReader(HttpContext.Request.Body).ReadToEndAsync());
      if (message == null) return BadRequest();
      
      var payment = message?.Object;
      Console.WriteLine(payment.Status.ToString() + payment.Paid.ToString());
      if (message.Event == Event.PaymentWaitingForCapture && payment.Paid)
      {
        var dbPayment = await _paymentService.CapturePayment(payment);

        if (!Enum.TryParse(dbPayment.Metadata[Payment.TypeMetadataName], out Payment.Types type)) return Ok();
        
        switch (type)
        {
          case Payment.Types.SubscriptionContinuation:
            await SubscriptionContinuationHandle(dbPayment);
            break;
          default:
            return BadRequest();
        }
      }

      return Ok();
    }

    private async Task SubscriptionContinuationHandle(Entities.Payment payment)
    {
      var user = payment.User;
      var planId = int.Parse(payment.Metadata["planId"]);
      var plan = await _db.SubscriptionsPlans.FirstOrDefaultAsync(x => x.Id == planId);
      if(plan == null) return;

      var subscription = new Subscription
      {
        PlanId = plan.Id,
        UserId = user.Id,
        DateStart = DateTime.UtcNow,
      };
      
      if (user.Subscription == null)
        subscription.DateEnd = DateTime.UtcNow + plan.Duration;
      else
      {
        subscription.DateStart = user.Subscription.DateEnd;
        subscription.DateEnd = user.Subscription.DateEnd + plan.Duration;
      }

      await _db.Subscriptions.AddAsync(subscription);
      await _db.SaveChangesAsync();
    }
  }
}