using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ApplicationContext = ExtremeInsiders.Data.ApplicationContext;

namespace ExtremeInsiders.Services
{
  public class SocialAuthIdentity
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
  }

  public abstract class SocialAuthService
  {
    public abstract string ProviderName { get; }

    protected readonly Data.ApplicationContext _db;
    protected readonly AppSettings _appSettings;

    protected SocialAuthService(Data.ApplicationContext db, IOptions<AppSettings> appSettings)
    {
      _db = db;
      _appSettings = appSettings.Value;
    }

    public abstract Task<SocialAuthIdentity> GetIdentity(string token);

    public async Task<User> FindUser(string token)
    {
      var identity = await GetIdentity(token);

      if (identity == null)
        return null;

      return await _db.Users.SingleOrDefaultAsync(u =>
        u.SocialAccounts.Any(a => a.Provider.Name == ProviderName && a.Key == identity.Id));
    }

    public async Task<SocialAccount> CreateAccount(string token)
    {
      var identity = await GetIdentity(token);
      if (identity == null || await _db.SocialAccounts.AnyAsync(a => a.Key == identity.Id && a.UserId != null))
        return null;

      return new SocialAccount
      {
        Key = identity.Id,
        Provider = await _db.SocialAccountsProviders.FirstOrDefaultAsync(r =>
          r.Name == ProviderName)
      };
    }
    
    public async Task<SocialAccount> CreateAccount(SocialAuthIdentity identity)
    {
      if (identity == null || await _db.SocialAccounts.AnyAsync(a => a.Key == identity.Id && a.UserId != null))
        return null;

      return new SocialAccount
      {
        Key = identity.Id,
        Provider = await _db.SocialAccountsProviders.FirstOrDefaultAsync(r =>
          r.Name == ProviderName)
      };
    }
  }

  public class FacebookSocialAuthService : SocialAuthService
  {
    public override string ProviderName => SocialAccountProvider.Providers.Facebook;

    public FacebookSocialAuthService(Data.ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
    {
    }

    public override async Task<SocialAuthIdentity> GetIdentity(string token)
    {
      var url = $"https://graph.facebook.com/v7.0/me?fields=email%2Cname&access_token={token}";

      try
      {
        using var client = new HttpClient();
        var result = await client.GetAsync(url);
        var identity = JsonConvert.DeserializeObject<SocialAuthIdentity>(await result.Content.ReadAsStringAsync());
        return identity.Id == null ? null : identity;
      }
      catch
      {
        return null;
      }
    }
  }

  public class GoogleSocialAuthService : SocialAuthService
  {
    public override string ProviderName => SocialAccountProvider.Providers.Google;

    public GoogleSocialAuthService(Data.ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
    {
    }

    public override async Task<SocialAuthIdentity> GetIdentity(string token)
    {
      try
      {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
        {
          Audience = new[] {_appSettings.GoogleClientId},
        });
        
        return new SocialAuthIdentity
        {
          Id = payload.Subject,
          Email = payload.Email,
          Name = payload.Name
        };
      }
      catch
      {
        return null;
      }
    }
  }

  public class VkSocialAuthService : SocialAuthService
  {
    public override string ProviderName => SocialAccountProvider.Providers.Vk;

    public VkSocialAuthService(Data.ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
    {
    }

    public override async Task<SocialAuthIdentity> GetIdentity(string code)
    {
      try
      {
        using var client = new HttpClient();
        
        var url = $"https://api.vk.com/method/users.get?access_token={code}&v=5.110";
        var result = await client.GetAsync(url);
        var str = await result.Content.ReadAsStringAsync();
        
        const string toDeleteFirst = "{\"response\":[";
        const string toDeleteLast = "]}";
        str = str.Remove(str.IndexOf("{\"response\":["), toDeleteFirst.Length);
        str = str.Remove(str.IndexOf("]}"), toDeleteLast.Length);
        
        var profile = JsonConvert.DeserializeObject<VkIdentity>(str);
        var socialIdentity = new SocialAuthIdentity
        {
          Id = profile.id,
          Email = profile.email,
          Name = profile.first_name + " " + profile.last_name
        };

        return socialIdentity;
      }
      catch
      {
        return null;
      }
    }

    private class VkIdentity
    {
      public string access_token { get; set; }
      public string expires_in { get; set; }
      public string id { get; set; }
      public string email { get; set; }
      public string first_name { get; set; }
      public string last_name { get; set; }
    }
  }
}