using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Migrations;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class PromoCode
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Code { get; set; }
    
    public int? SubscriptionPlanId { get; set; }
    public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    
    public int? EntitySaleableId { get; set; }
    public virtual EntitySaleable EntitySaleable {get; set; }
    
    public bool IsInfinite { get; set; }
    public bool IsValid { get; set; }
  }

  public class PromoCodeUser
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    public int PromoCodeId { get; set; }
    public virtual PromoCode PromoCode { get; set; }
  }
}