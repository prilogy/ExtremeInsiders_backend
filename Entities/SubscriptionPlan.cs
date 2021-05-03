using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class SubscriptionPlan : ITranslatableEntity<SubscriptionPlan, SubscriptionPlanTranslation>, ISaleable<SubscriptionPlanPrice>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    public TimeSpan Duration { get; set; }
    public string AppleInAppPurchaseKey { get; set; }
    public string GoogleInAppPurchaseKey { get; set; }

    public string Color { get; set; }
    
    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SubscriptionPlanTranslation> Translations { get; set; }
    [NotMapped]
    public SubscriptionPlanTranslation Content { get; set; }
    
    [JsonIgnore]
    public virtual List<SubscriptionPlanPrice> Prices { get; set; }
    [NotMapped]
    public SubscriptionPlanPrice Price { get; set; }
  }

  public class SubscriptionPlanTranslation : TranslatableEntityTranslation<SubscriptionPlan>, IDefaultTranslatableContent
  {
    public string Name { get; set; }
    public string Description { get; set; }
  }

  public class SubscriptionPlanPrice : ISaleablePrice
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    public decimal Value { get; set; }
    [AllowNull]
    public decimal DiscountValue { get; set; }
    [JsonIgnore]
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    [JsonIgnore]
    public int EntityId { get; set; }
    public virtual SubscriptionPlan Entity { get; set; }
  }
}