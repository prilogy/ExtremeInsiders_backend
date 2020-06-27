using System;
using System.ComponentModel.DataAnnotations.Schema;
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
    [JsonIgnore]
    public virtual SubscriptionPlan Plan { get; set; }

    public static Subscription Demo(User user)
    {
      return new Subscription
      {
        User =  user,
        DateStart = DateTime.UtcNow,
        DateEnd = DateTime.Now + TimeSpan.FromDays(31)
      };
    }
  }
}