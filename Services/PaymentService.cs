using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Yandex.Checkout.V3;
using Payment = Yandex.Checkout.V3.Payment;

namespace ExtremeInsiders.Services
{
  public class PaymentService
  {
    private readonly ApplicationContext _db;
    private readonly Yandex.Checkout.V3.AsyncClient _client;
    private readonly UserService _userService;


    public PaymentService(ApplicationContext db, IOptions<AppSettings> appSettings, UserService userService)
    {
      _db = db;
      _userService = userService;
      _client = new Yandex.Checkout.V3.Client(
        shopId: appSettings.Value.KassaShopId,
        secretKey: appSettings.Value.KassaSecret).MakeAsync();
    }

    public async Task<string> CreatePayment(
      decimal value,
      Entities.Payment.Types type,
      Dictionary<string, string> metadata = null, 
      User user = null, 
      Currency currency = null)
    {
      if (user == null) user = _userService.User;
      if (currency == null) currency = user.Currency;

      metadata[Entities.Payment.TypeMetadataName] = type.ToString();

      var newPayment = new NewPayment
      {
        Amount = new Amount {Value = value, Currency = currency.Key},
        Metadata = metadata,
        Confirmation = new Confirmation
        {
          Type = ConfirmationType.Redirect,
          ReturnUrl = "localhost"
        }
      };

      var payment = await _client.CreatePaymentAsync(newPayment);
      await SavePaymentToDb(payment, user, currency, type);
      
      return payment.Confirmation.ConfirmationUrl;
    }

    public async Task<Entities.Payment> CapturePayment(Yandex.Checkout.V3.Payment payment)
    {
      var dbPayment = await _db.Payments.FirstOrDefaultAsync(x => x.Key == payment.Id);
      if (dbPayment == null) return null;
      
      payment = await _client.CapturePaymentAsync(payment);
      dbPayment.Status = PaymentStatus.Succeeded;
      await _db.SaveChangesAsync();

      dbPayment.Metadata = payment.Metadata;
      return dbPayment;
    }

    private async Task<Entities.Payment> SavePaymentToDb(Yandex.Checkout.V3.Payment payment, User user, Currency currency, Entities.Payment.Types type)
    {
      var dbPayment = new Entities.Payment
      {
        Key = payment.Id,
        Value = payment.Amount.Value,
        Status = payment.Status,
        UserId = user.Id,
        CurrencyId = currency.Id,
        Type = type
      };
      
      await _db.Payments.AddAsync(dbPayment);
      await _db.SaveChangesAsync();
      
      return dbPayment;
    }
  }
}