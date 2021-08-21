using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Culture
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    public string Key { get; set; }
    
    public static readonly Culture Russian = new Culture
    {
      Id = 1, Key = "ru"
    };

    public static readonly Culture English = new Culture
    {
      Id = 2, Key = "en"
    };

    public static Culture Fallback => English;
    
    public static readonly List<Culture> All = new List<Culture>
    {
      Russian,
      English
    };

    public static Culture Default => English;
  }
}