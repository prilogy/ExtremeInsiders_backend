using System.Collections.Generic;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Enums;
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
  public class SaleController : Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly KassaPaymentService _kassaPaymentService;

    public SaleController(ApplicationContext db, UserService userService, KassaPaymentService kassaPaymentService)
    {
      _db = db;
      _userService = userService;
      _kassaPaymentService = kassaPaymentService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentUrl(int id)
    {
      var entity = (await _db.EntitiesSaleable.FirstOrDefaultAsync(x => x.Id == id));
      if (entity == null || !entity.IsPaid) return NotFound();
      entity = (EntitySaleable)entity.OfCurrency(_userService.Currency);
      
      var metadata = new Dictionary<string, string>
      {
        {"entityId", entity.Id.ToString()}
      };
      
      var url = await _kassaPaymentService.CreateAsync(
        user: _userService.User, 
        currency: entity.Price.Currency, 
        type: PaymentTypes.SaleableEntityBuy, 
        metadata: metadata, 
        value: entity.Price.Value);

      return Ok(url);
    }
  }
}