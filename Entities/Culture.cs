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
    
    public static Culture Russian = new Culture
    {
      Id = 1, Key = "ru"
    };

    public static Culture English = new Culture
    {
      Id = 2, Key = "en"
    };
    
    public static List<Culture> AllCultures = new List<Culture>
    {
      Russian,
      English
    }; 
  }
}