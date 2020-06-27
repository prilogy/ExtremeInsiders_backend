using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Yandex.Checkout.V3;

namespace ExtremeInsiders.Entities
{
  public class Payment
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Key { get; set; }
    public decimal Value { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime DateCreated { get; set; }
    public Types Type { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; }
    [NotMapped]
    public Dictionary<string, string> Metadata { get; set; }

    public enum Types
    {
      SubscriptionContinuation
    }
    
    public Payment()
    {
      DateCreated = DateTime.Now;
    }
  }
}