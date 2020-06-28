using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class SubscriptionController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly PaymentService _paymentService;

    public SubscriptionController(ApplicationContext db, UserService userService, PaymentService paymentService)
    {
      _db = db;
      _userService = userService;
      _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlans()
    {
      return Ok((await _db.SubscriptionsPlans.ToListAsync()).OfCulture(_userService.Culture));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentUrl(int id)
    {
      var plan = await _db.SubscriptionsPlans.FirstOrDefaultAsync(x => x.Id == id);
      if (plan == null) return NotFound();
      plan = (SubscriptionPlan)plan.OfCurrency(_userService.Currency);

      var metadata = new Dictionary<string, string>
      {
        {"planId", plan.Id.ToString()}
      };
      
      
      var url = await _paymentService.CreatePayment(user: _userService.User, currency: plan.Price.Currency, type: Payment.Types.SubscriptionContinuation, metadata: metadata, value: plan.Price.Value);

      return Ok(url);
    }
  }
}