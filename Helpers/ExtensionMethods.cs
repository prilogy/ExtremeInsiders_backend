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
    public static List<T> OfCulture<T, TR>(this IEnumerable<TranslatableEntity<T, TR>> sports, Culture culture) 
      where T : TranslatableEntity<T, TR>
      where TR: TranslatableEntityTranslation<T>
    {
      return sports.Select(s => s.OfCulture(culture)).ToList();
    }
  }
}