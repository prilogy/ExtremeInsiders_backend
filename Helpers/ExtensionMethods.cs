using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoogleAuth.Entities;

namespace GoogleAuth.Helpers
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
          user.SocialAccounts,
          user.Token
        }
        : new
        {
          user.Id,
          user.Email,
          user.Name,
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
}