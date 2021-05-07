using System;
using System.Threading.Tasks;
using AppleReceiptVerifierCore;
using AppleReceiptVerifierCore.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Enums;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExtremeInsiders.Services
{
    public class ApplePaymentService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationContext _dbContext;
        private readonly SaleService _saleService;

        public ApplePaymentService(ApplicationContext dbContext, SaleService saleService,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _saleService = saleService;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> HandleAsync(ApplePayment payment, User user)
        {
            Console.WriteLine(payment.Dump("Apple payment for userId=" + user.Id));
            if (payment.TransactionReceipt == null || (payment.EntityId == null && payment.PlanId == null))
                return false;

            var receipt = await GetReceiptAsync(payment.TransactionReceipt);
            Console.WriteLine(receipt == null ? "NO APPLE RECEIPT GOT;" : receipt.Dump("Apple receipt"));
            if (receipt == null || receipt.status != 0) return false;

            PaymentTypes? type = payment switch
            {
                var x when x.PlanId != null => PaymentTypes.SubscriptionContinuation,
                var x when x.EntityId != null => PaymentTypes.SaleableEntityBuy,
                _ => null
            };

            Console.WriteLine("payment type: " + type);
            if (type == null) return false;

            var dbPayment = CreateAsync(payment, user, type.Value);

            return type switch
            {
                PaymentTypes.SubscriptionContinuation => await _saleService.SubscriptionContinuationHandle(user,
                    payment.PlanId, dbPayment.Id),
                PaymentTypes.SaleableEntityBuy => await _saleService.SaleableEntityBuyHandle(user, payment.EntityId,
                    dbPayment.Id),
                _ => false
            };
        }

        private async Task<Payment> CreateAsync(ApplePayment payment, User user, PaymentTypes type)
        {
            var dbPayment = new Payment
            {
                Key = payment.TransactionReceipt,
                UserId = user.Id,
                Type = type,
                CurrencyId = user.CurrencyId,
                ProviderType = PaymentProviderTypes.Apple
            };

            await _dbContext.Payments.AddAsync(dbPayment);
            await _dbContext.SaveChangesAsync();

            return dbPayment;
        }

        private async Task<AppleReceiptResponse> GetReceiptAsync(string receipt)
        {
            var manager = new ReceiptManager();
            
            Console.WriteLine("Sandbox mode: " + _appSettings.ApplePurchaseSandboxMode);
            
            return await manager.ValidateReceipt(
                _appSettings.ApplePurchaseSandboxMode ? Environments.Sandbox : Environments.Production, receipt);
        }
    }
}