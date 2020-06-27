using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Mvc;
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
      var message = Client.ParseMessage(Request.Method, Request.ContentType, Request.Body);
      var payment = message?.Object;
      if (message?.Event == Event.PaymentWaitingForCapture && payment.Paid)
      {
        var dbPayment = await _paymentService.CapturePayment(payment);
      }

      return Ok();
    }
  }
}