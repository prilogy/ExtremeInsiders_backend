using System;

namespace ExtremeInsiders.Enums
{
    public enum PaymentTypes
    {
        SubscriptionContinuation,
        SaleableEntityBuy
    }

    public static class PaymentTypesExtensions
    {
        public static PaymentTypes? FromString(string v) =>
            Enum.TryParse(v, out PaymentTypes type) ? (PaymentTypes?) type : null;
    }
}