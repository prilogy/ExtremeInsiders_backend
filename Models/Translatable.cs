using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Models
{
  
  public abstract class TranslatableEntity<T, TR>
    where T: TranslatableEntity<T, TR>
    where TR: TranslatableEntityTranslation
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [JsonIgnore]
    public virtual List<TR> Translations { get; set; }
    [NotMapped]
    public TR Content { get; set; }

    public T OfCulture(Culture culture)
    {
      Content = Translations.FirstOrDefault(tr => tr.Culture.Key == culture.Key) ?? Translations.First();
      Translations = null;
      return this as T;
    }
    
    public T OfCulture(string culture)
    {
      var cultureObj = Culture.AllCultures.FirstOrDefault();
      return cultureObj == null ? OfCulture(Culture.AllCultures.First()) : OfCulture(cultureObj);
    }
  }
  
  public abstract class TranslatableEntityTranslation
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonIgnore]
    public int CultureId { get; set; }
    public virtual Culture Culture { get; set; }
  }

}