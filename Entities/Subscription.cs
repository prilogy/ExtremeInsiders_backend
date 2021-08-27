using System;
using System.ComponentModel.DataAnnotations.Schema;
using Castle.Components.DictionaryAdapter;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Subscription
  {
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    [JsonIgnore]
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    
    [JsonIgnore]
    public int? PlanId { get; set; }
    public virtual SubscriptionPlan Plan { get; set; }
    [JsonIgnore]
    public int? PaymentId { get; set; }
    [JsonIgnore]
    public virtual Payment Payment { get; set; }

    public static Subscription Demo(User user)
    {
      return new Subscription
      {
        User =  user,
        DateStart = DateTime.UtcNow,
        DateEnd = DateTime.UtcNow + TimeSpan.FromDays(14)
      };
    }

    public static Subscription Create(SubscriptionPlan plan, User user, int? paymentId=null)
    {
      var subscription = new Subscription
      {
        PlanId = plan.Id,
        UserId = user.Id,
        DateStart = DateTime.UtcNow,
        PaymentId = paymentId
      };
      
      if (user.Subscription == null)
        subscription.DateEnd = DateTime.UtcNow + plan.Duration;
      else
      {
        subscription.DateStart = user.Subscription.DateEnd;
        subscription.DateEnd = user.Subscription.DateEnd + plan.Duration;
      }

      return subscription;
    }
  }
}