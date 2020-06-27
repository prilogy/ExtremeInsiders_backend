using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtremeInsiders.Entities
{
  public class Currency
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Key { get; set; }

    public static readonly Currency RUB = new Currency
    {
      Id = 1,
      Key = "RUB"
    };
    
    public static readonly Currency EUR = new Currency
    {
      Id = 2,
      Key = "EUR"
    };
    
    public static readonly Currency USD = new Currency
    {
      Id = 3,
      Key = "USD"
    };

    public static Currency Default => USD;
    
    public static readonly List<Currency> All = new List<Currency>
    {
      RUB,
      EUR,
      USD
    }; 
  }
}