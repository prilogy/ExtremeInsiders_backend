using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ExtremeInsiders.Entities
{
  public class Culture
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
  }

  interface ITranslatable<T,TR> where T: class
  {
    public List<TR> Translations { get; set; }
    public TR Content { get; set; }
    public T OfCulture(Culture culture);
    public T OfCulture(string culture);
  }

  public static class TranslatableExtensions
  {
    public static List<Sport> OfCulture(this IEnumerable<Sport> sports, Culture culture)
    {
      return sports.Select(s => s.OfCulture(culture)).ToList();
    }
  }
}