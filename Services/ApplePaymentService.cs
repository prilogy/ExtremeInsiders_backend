﻿using System;
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
            if (payment.TransactionReceipt == null || payment.EntityId == null || payment.PlanId == null)
                return false;

            var receipt = await GetReceiptAsync(payment.TransactionReceipt);
            if (receipt == null || receipt.status != 0) return false;

            PaymentTypes? type = payment switch
            {
                var x when x.PlanId != null => PaymentTypes.SubscriptionContinuation,
                var x when x.EntityId != null => PaymentTypes.SaleableEntityBuy,
                _ => null
            };

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
                ProviderType = PaymentProviderTypes.Apple
            };

            await _dbContext.Payments.AddAsync(dbPayment);
            await _dbContext.SaveChangesAsync();

            return dbPayment;
        }

        private async Task<AppleReceiptResponse> GetReceiptAsync(string receipt)
        {
            var manager = new ReceiptManager();
            return await manager.ValidateReceipt(
                _appSettings.ApplePurchaseSandboxMode ? Environments.Sandbox : Environments.Production, receipt);
        }
    }
}