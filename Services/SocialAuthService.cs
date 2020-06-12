using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Auth;
using GoogleAuth.Entities;
using GoogleAuth.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ApplicationContext = GoogleAuth.Data.ApplicationContext;

namespace GoogleAuth.Services
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

    protected readonly ApplicationContext _db;
    protected readonly AppSettings _appSettings;

    protected SocialAuthService(ApplicationContext db, IOptions<AppSettings> appSettings)
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
        Provider = await _db.SocialAccountProviders.FirstOrDefaultAsync(r =>
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
        Provider = await _db.SocialAccountProviders.FirstOrDefaultAsync(r =>
          r.Name == ProviderName)
      };
    }
  }

  public class FacebookSocialAuthService : SocialAuthService
  {
    public override string ProviderName => SocialAccountProvider.Providers.Facebook;

    public FacebookSocialAuthService(ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
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

    public GoogleSocialAuthService(ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
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
        Console.WriteLine($"Google: {payload.Email}, {payload.Name}, {payload.Subject}");
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

    public VkSocialAuthService(ApplicationContext db, IOptions<AppSettings> appSettings) : base(db, appSettings)
    {
    }

    public override async Task<SocialAuthIdentity> GetIdentity(string code)
    {
      var url =
        $"https://oauth.vk.com/access_token?client_id={_appSettings.VkClientId}&client_secret={_appSettings.VkSecret}&redirect_uri={_appSettings.VkRedirect}&code={code}";

      try
      {
        using var client = new HttpClient();
        var result = await client.GetAsync(url);
        var identity = JsonConvert.DeserializeObject<VkIdentity>(await result.Content.ReadAsStringAsync());
        var socialIdentity = new SocialAuthIdentity
        {
          Id = identity.user_id,
          Email = identity.email ?? ""
        };

        if (socialIdentity.Email == "")
          return null;

        url = $"https://api.vk.com/method/users.get?user_ids={socialIdentity.Id}&access_token={identity.access_token}&v=5.110";
        result = await client.GetAsync(url);
        var str = await result.Content.ReadAsStringAsync();
        
        const string toDeleteFirst = "{\"response\":[";
        const string toDeleteLast = "]}";
        str = str.Remove(str.IndexOf("{\"response\":["), toDeleteFirst.Length);
        str = str.Remove(str.IndexOf("]}"), toDeleteLast.Length);
        
        var profile = JsonConvert.DeserializeObject<VkIdentity>(str);
        
        socialIdentity.Name = profile.first_name + " " + profile.last_name;
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
      public string user_id { get; set; }
      public string email { get; set; }
      public string first_name { get; set; }
      public string last_name { get; set; }
    }
  }
}