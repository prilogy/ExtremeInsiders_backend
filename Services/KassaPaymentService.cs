using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Enums;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Yandex.Checkout.V3;
using Payment = ExtremeInsiders.Entities.Payment;

namespace ExtremeInsiders.Services
{
    public class KassaPaymentService
    {
        private readonly ApplicationContext _db;
        private readonly AsyncClient _client;


        public KassaPaymentService(ApplicationContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _client = new Client(
                appSettings.Value.KassaShopId,
                appSettings.Value.KassaSecret).MakeAsync();
        }

        public async Task<string> CreateAsync(
            decimal value,
            PaymentTypes type,
            User user,
            Dictionary<string, string> metadata,
            Currency currency = null)
        {
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
            await SaveAsync(payment, user, currency, type);

            return payment.Confirmation.ConfirmationUrl;
        }

        public async Task<Payment> CaptureAsync(Yandex.Checkout.V3.Payment payment)
        {
            var dbPayment = await _db.Payments.FirstOrDefaultAsync(x => x.Key == payment.Id);
            if (dbPayment == null) return null;

            payment = await _client.CapturePaymentAsync(payment);
            dbPayment.Status = PaymentStatus.Succeeded;
            await _db.SaveChangesAsync();

            dbPayment.Metadata = payment.Metadata;
            return dbPayment;
        }

        private async Task<Payment> SaveAsync(Yandex.Checkout.V3.Payment payment, User user, Currency currency,
            PaymentTypes type)
        {
            var dbPayment = new Payment
            {
                Key = payment.Id,
                Value = payment.Amount.Value,
                Status = payment.Status,
                UserId = user.Id,
                CurrencyId = currency.Id,
                Type = type,
                ProviderType = PaymentProviderTypes.Kassa
            };

            await _db.Payments.AddAsync(dbPayment);
            await _db.SaveChangesAsync();

            return dbPayment;
        }

        public async Task<PaymentStatus?> CheckStatusAsync(string url)
        {
            var id = url.Contains("orderId=") ? url.Split("orderId=").LastOrDefault() : null;
            if (id == null) return null;
            
            var p = await _db.Payments.FirstOrDefaultAsync(x => x.Key == id);

            Console.WriteLine(p?.Dump("Kassa payment check") ?? "KASSA PAYMENY NULL");
            
            return p?.Status;
        }
    }
}