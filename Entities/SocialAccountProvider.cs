using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtremeInsiders.Entities
{
  public class SocialAccountProvider
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }

    public static readonly List<SocialAccountProvider> All = new List<SocialAccountProvider>
    {
      new SocialAccountProvider {Id = 1, Name = Providers.Google},
      new SocialAccountProvider {Id = 2, Name = Providers.Vk},
      new SocialAccountProvider {Id = 3, Name = Providers.Facebook},
      new SocialAccountProvider {Id = 4, Name = Providers.Apple}
    };

    public static class Providers
    {
      public const string Google = "google";
      public const string Vk = "vk";
      public const string Facebook = "facebook";
      public const string Apple = "apple";
    }
  }
}