using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class SubscriptionPlan : ITranslatableEntity<SubscriptionPlan, SubscriptionPlanTranslation>, ISaleable<SubscriptionPlanPrice>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public TimeSpan Duration { get; set; }
    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SubscriptionPlanTranslation> Translations { get; set; }
    [NotMapped]
    public SubscriptionPlanTranslation Content { get; set; }


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
    public int Id { get; set; }
    public decimal Value { get; set; }
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    public int EntityId { get; set; }
    public virtual SubscriptionPlan Entity { get; set; }
  }
}