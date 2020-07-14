using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class PromoCode
  {
    public int Id { get; set; }
    public string Code { get; set; }
    
    public int? SubscriptionPlanId { get; set; }
    public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    
    public int? EntitySaleableId { get; set; }
    public virtual EntitySaleable EntitySaleable {get; set; }
    
    public bool IsInfinite { get; set; }
    public bool IsValid { get; set; }
  }
}