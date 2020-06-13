using System.ComponentModel.Design;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExtremeInsiders.Helpers
{
  public class AppSettings
  {
    public string Secret { get; set; }
    public string GoogleClientId { get; set; }
    public string VkClientId { get; set; }
    public string VkSecret { get; set; }
    public string VkRedirect { get; set; }
  }
  
  public static class AppSettingsExtensions {
    public static byte[] ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
      var appSettingsSection = configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);
      var appSettings = appSettingsSection.Get<AppSettings>();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);
      return key;
    }
  }
}