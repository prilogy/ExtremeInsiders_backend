using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Entities
{
  public class Sport : TranslatableEntity<Sport, SportTranslation>
  {
    [JsonIgnore] public int? ImageId { get; set; }
    public virtual Image Image { get; set; }

    public override Sport OfCulture(Culture culture)
    {
      Content = Translations.SingleOrDefault(tr => tr.Culture.Key == culture.Key) ?? Translations.First();
      Translations = null;
      return this;
    }
  }

  public interface ITranslatableEntityTranslation
  {
    public Culture Culture { get; set; }
  }

  public abstract class TranslatableEntity<T, TR>
    where TR: ITranslatableEntityTranslation
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [JsonIgnore]
    public virtual List<TR> Translations { get; set; }
    [NotMapped]
    public TR Content { get; set; }

    public abstract T OfCulture(Culture culture);

    // public TranslatableEntity<TR> OfCulture(Culture culture)
    // {
    //   Content = Translations.SingleOrDefault(tr => tr.Culture.Key == culture.Key) ?? Translations.First();
    //   return this;
    // }
    //
    // public TranslatableEntity<TR> OfCulture(string culture)
    // {
    //   Content = Translations.SingleOrDefault(tr => tr.Culture.Key == culture) ?? Translations.First();
    //   return this;
    // }
  }

  public class SportTranslation : ITranslatableEntityTranslation
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    [JsonIgnore] 
    public int SportId { get; set; }
    [JsonIgnore]
    public virtual Sport Sport { get; set; }

    [JsonIgnore] public int CultureId { get; set; }
    public virtual Culture Culture { get; set; }
  }
}