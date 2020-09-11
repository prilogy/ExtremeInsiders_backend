using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Entities;
using Newtonsoft.Json;

namespace ExtremeInsiders.Models
{
  public interface ISaleable<T>
  {
    List<T> Prices { get; set; }
    T Price { get; set; }
  }

  public interface ISaleablePrice
  {
    public decimal Value { get; set; }
    
    int CurrencyId { get; set; }
    Currency Currency { get; set; }
    
    int EntityId { get; set; }
  }
  
  public class EntitySaleable : EntityBase, ISaleable<EntitySaleablePrice>
  {
    [ForeignKey("EntityId")]
    [JsonIgnore]
    public virtual List<Sale> Sales { get; set; }
    [ForeignKey("EntityId")]
    [JsonIgnore]
    public virtual List<EntitySaleablePrice> Prices { get; set; }
    
    [NotMapped]
    public EntitySaleablePrice Price { get; set; }

    public bool IsPaid => Prices?.Count > 0;

    public int? SalesAmount => Sales?.Count;
  }

  public class EntitySaleablePrice : ISaleablePrice
  {
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public decimal Value { get; set; }
    
    [JsonIgnore]
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    
    [JsonIgnore]
    public int EntityId { get; set; }
    [JsonIgnore]
    public virtual EntitySaleable Entity { get; set; }
  }
}