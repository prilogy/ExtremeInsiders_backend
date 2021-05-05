using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Services
{
    public class SaleService
    {
        private readonly ApplicationContext _dbContext;

        public SaleService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaleableEntityBuyHandle(User user, int? entityId, int? paymentId = null)
        {
            if (entityId == null) return false;

            var sale = new Sale
            {
                EntityId = entityId.Value,
                UserId = user.Id,
                PaymentId = paymentId
            };

            await _dbContext.Sales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SubscriptionContinuationHandle(User user, int? planId, int? paymentId = null)
        {
            if (planId == null) return false;
            var plan = await _dbContext.SubscriptionsPlans.FirstOrDefaultAsync(x => x.Id == planId);
            if (plan == null) return false;

            var subscription = Subscription.Create(plan, user, paymentId);

            await _dbContext.Subscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}