using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class SubscriptionPlan : ITranslatableEntity<SubscriptionPlan, SubscriptionPlanTranslation>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public TimeSpan Duration { get; set; }
    public int CostInRub { get; set; }
    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SubscriptionPlanTranslation> Translations { get; set; }
    [NotMapped]
    public SubscriptionPlanTranslation Content { get; set; }
  }

  public class SubscriptionPlanTranslation : TranslatableEntityTranslation<SubscriptionPlan>, IDefaultTranslatableContent
  {
    public string Name { get; set; }
    public string Description { get; set; }
  }
}