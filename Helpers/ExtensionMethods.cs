using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Microsoft.EntityFrameworkCore;

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
  
  
  public static class SearchExtensions
  {
    public static async Task<List<T>> SearchAtWithQueryAsync<T, TR>(this DbSet<T> set, string query)
      where T : EntityBase, ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>, IDefaultTranslatableContent
    {
      query = query.ToLower();
      return await set.Where(x => x.Translations.Any(y => y.Name.ToLower().Contains(query))).ToListAsync();
    }
    
    public static async Task<List<string>> PredictWithQueryAsync<T, TR>(this DbSet<T> set,string query, Culture culture)
      where T: EntityBase, ITranslatableEntity<T, TR>
      where TR: TranslatableEntityTranslation<T>, IDefaultTranslatableContent
    {
      query = query.ToLower();
      return (await set.Where(x => x.Translations.Any(y => y.Name.ToLower().Contains(query))).ToListAsync()).Select(x =>
        x.Translations.FirstOrDefault(y => y.Culture.Key == culture.Key)?.Name).ToList();
    }
  }
}