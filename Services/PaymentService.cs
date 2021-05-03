using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Yandex.Checkout.V3;
using Payment = ExtremeInsiders.Entities.Payment;

namespace ExtremeInsiders.Services
{
    public class PaymentService
    {
        private readonly ApplicationContext _db;
        private readonly AsyncClient _client;
        private readonly UserService _userService;


        public PaymentService(ApplicationContext db, IOptions<AppSettings> appSettings, UserService userService)
        {
            _db = db;
            _userService = userService;
            _client = new Client(
                appSettings.Value.KassaShopId,
                appSettings.Value.KassaSecret).MakeAsync();
        }

        public async Task<string> CreatePayment(
            decimal value,
            Payment.Types type,
            Dictionary<string, string> metadata = null,
            User user = null,
            Currency currency = null)
        {
            user ??= _userService.User;
            currency ??= user.Currency;

            metadata[Payment.TypeMetadataName] = type.ToString();

            if (currency.Key != "RUB")
            {
                using var client = new HttpClient();
                const string url = "https://www.cbr-xml-daily.ru/daily_json.js";
                var result = await client.GetAsync(url);
                var rate = JsonConvert.DeserializeObject<CurrenciesRate>(await result.Content.ReadAsStringAsync());

                if (Equals(currency, Currency.EUR))
                    value *= Convert.ToDecimal(rate.Valute.EUR.Value);
                else if (Equals(currency, Currency.USD))
                    value *= Convert.ToDecimal(rate.Valute.USD.Value);
            }

            var newPayment = new NewPayment
            {
                Amount = new Amount { Value = value, Currency = "RUB" },
                Metadata = metadata,
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = "localhost",
                    Locale = user.Culture.Key == Culture.Russian.Key ? "ru_RU" : "en_US"
                },
            };

            var payment = await _client.CreatePaymentAsync(newPayment);
            await SavePaymentToDb(payment, user, currency, type);

            return payment.Confirmation.ConfirmationUrl;
        }

        public async Task<Payment> CapturePayment(Yandex.Checkout.V3.Payment payment)
        {
            var dbPayment = await _db.Payments.FirstOrDefaultAsync(x => x.Key == payment.Id);
            if (dbPayment == null) return null;

            payment = await _client.CapturePaymentAsync(payment);
            dbPayment.Status = PaymentStatus.Succeeded;
            await _db.SaveChangesAsync();

            dbPayment.Metadata = payment.Metadata;
            return dbPayment;
        }

        private async Task<Payment> SavePaymentToDb(Yandex.Checkout.V3.Payment payment, User user, Currency currency,
            Payment.Types type)
        {
            var dbPayment = new Payment
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

        private class CurrenciesRate
        {
            public CurrencyRate Valute { get; set; }
        }

        private class CurrencyRate
        {
            public CurrencyObject USD { get; set; }
            public CurrencyObject EUR { get; set; }

            public class CurrencyObject
            {
                public double Value { get; set; }
                public string CharCode { get; set; }
            }
        }
    }
}