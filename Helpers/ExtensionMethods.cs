using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Helpers
{
  public static class PagingExtensions
  {
    //used by LINQ to SQL
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
    {
      return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    //used by LINQ
    public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
    {
      return source.Skip((page - 1) * pageSize).Take(pageSize);
    }
  }

  public static class TranslatableExtensions
  {
    public static T OfCulture<T, TR>(this ITranslatableEntity<T, TR> entity, Culture culture)
      where T : ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>
    {
      if (entity.Translations.Count > 0)
      {
        entity.Content = entity.Translations.FirstOrDefault(tr => tr.Culture.Key == culture.Key) ??
                         entity.Translations.First();
        entity.Translations = null;
      }

      return (T)entity;
    }
    
    public static T OfCulture<T, TR>(this ITranslatableEntity<T, TR> entity, string culture)
      where T : ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>
    {
      var cultureObj = Culture.AllCultures.FirstOrDefault(x => x.Key == culture);
      return cultureObj == null ? entity.OfCulture(Culture.AllCultures.First()) : entity.OfCulture(cultureObj);
    }
    public static List<T> OfCulture<T, TR>(this IEnumerable<ITranslatableEntity<T, TR>> list, Culture culture) 
      where T : ITranslatableEntity<T, TR>
      where TR: TranslatableEntityTranslation<T>
    {
      return list.Select(s => s.OfCulture(culture)).ToList();
    }
  }
}