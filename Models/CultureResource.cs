using System.Collections.Generic;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Models
{
  public class CultureResource
  {
    private Culture Culture { get; }

    public CultureResource(Culture culture)
    {
      Culture = culture;
    }

    public string this[string key] => Resources[Culture.Key][key] ?? null;

    public Dictionary<string, Dictionary<string, string>> Resources { get; set; }
  }

  public static class CultureResources
  {
    public static CultureResource EmailConfirmation(Culture culture) => new CultureResource(culture)
    {
      Resources = new Dictionary<string, Dictionary<string, string>>
      {
        {
          Culture.English.Key, new Dictionary<string, string>
          {
            {"subject", "Email verification code"},
            {"body_before_code", "Your secret code to verify this Email address"},
            {"body_after_code", "Use this code in Profile page of ExtremeInsiders application."}
          }
        },
        {
          Culture.Russian.Key, new Dictionary<string, string>
          {
            {"subject", "Код подтверждения Email"},
            {"body_before_code", "Ваш секретный код для подтверждения Email"},
            {"body_after_code", "Используйте этот код на странице вашего профиля в приложении ExtremeInsiders."}
          }
        }
      }
    };
  }
}