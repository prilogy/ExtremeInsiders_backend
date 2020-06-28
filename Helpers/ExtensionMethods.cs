using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
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

  public static class FormatExtensions
  {
    public static T OfCulture<T, TR>(this ITranslatableEntity<T, TR> entity, Culture culture)
      where T : ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>
    {
      if (entity.Translations.Count > 0)
      {
        entity.Content = entity.Translations.FirstOrDefault(tr => tr.Culture.Key == culture.Key) ??
                         entity.Translations.FirstOrDefault(x => x.Culture.Key == Culture.Default.Key);
      }

      return (T)entity;
    }
    
    public static List<T> OfCulture<T, TR>(this IEnumerable<ITranslatableEntity<T, TR>> list, Culture culture) 
      where T : ITranslatableEntity<T, TR>
      where TR: TranslatableEntityTranslation<T>
    {
      return list.Select(s => s.OfCulture(culture)).ToList();
    }
    
    public static ISaleable<T> OfCurrency<T>(this ISaleable<T> entity, Currency currency)
    where T: ISaleablePrice
    {
      if (entity.Prices.Count > 0)
      {
        entity.Price = entity.Prices.FirstOrDefault(x => x.Currency.Key == currency.Key) ??
                       entity.Prices.FirstOrDefault(x => x.Currency.Key == Currency.Default.Key);
      }

      return entity;
    }
    
    public static List<ISaleable<T>> OfCurrency<T>(this IEnumerable<ISaleable<T>> list, Currency currency) 
      where T : ISaleablePrice
    {
      return list.Select(s => s.OfCurrency(currency)).ToList();
    }

    public static T OfFormat<T, TR>(this ITranslatableEntity<T, TR> entity, UserService userService)
      where T : EntityBase, ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>
    {
      entity = entity.OfCulture(userService.Culture);
      
      if (entity is EntitySaleable saleable && saleable.IsPaid)
      {
        saleable.OfCurrency(userService.Currency);
        if (userService.User.Sales.All(x => x.EntityId != saleable.Id))
        {
          if (entity.Content is IWithUrl withUrl)
            withUrl.Url = null;
        }
      }

      return (T)entity;
    }
    
    public static List<T> OfFormat<T, TR>(this IEnumerable<ITranslatableEntity<T, TR>> list, UserService userService) 
      where T : EntityBase, ITranslatableEntity<T, TR>
      where TR: TranslatableEntityTranslation<T>
    {
      return list.Select(s => s.OfFormat(userService)).ToList();
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
    
    public static List<T> SearchAtWithQueryAsync<T, TR>(this IEnumerable<T> set, string query)
      where T : EntityBase, ITranslatableEntity<T, TR>
      where TR : TranslatableEntityTranslation<T>, IDefaultTranslatableContent
    {
      query = query.ToLower();
      return set.Where(x => x.Translations.Any(y => y.Name.ToLower().Contains(query))).ToList();
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