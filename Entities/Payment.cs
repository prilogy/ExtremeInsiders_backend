using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Enums;
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
    public PaymentTypes Type { get; set; }
    public PaymentProviderTypes ProviderType { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    [NotMapped]
    public Dictionary<string, string> Metadata { get; set; }

    [NotMapped] public const string TypeMetadataName = "type";

    public Payment()
    {
      DateCreated = DateTime.UtcNow;
    }
  }
}