using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Helpers
{
  public static class UserExtensions
  {
    public static User WithoutPassword(this User user)
    {
      if (user == null) return null;

      user.Password = null;
      return user;
    }

    public static object WithoutSensitive(this User user, bool token = false)
    {
      return token
        ? (object) new
        {
          user.Id,
          user.Email,
          user.Name,
          user.BirthDate,
          user.PhoneNumber,
          user.Avatar,
          user.SocialAccounts,
          user.Token
        }
        : new
        {
          user.Id,
          user.Email,
          user.Name,
          user.BirthDate,
          user.PhoneNumber,
          user.Avatar,
          user.SocialAccounts
        };
    }
  }

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
      where TR: TranslatableEntityTranslation
    {
      return sports.Select(s => s.OfCulture(culture)).ToList();
    }
  }
}