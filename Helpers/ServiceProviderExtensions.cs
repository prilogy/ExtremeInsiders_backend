using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ExtremeInsiders.Helpers
{
  public static class ServiceProviderExtensions
  {
    public static void AddSocialAuthService(this IServiceCollection services)
    {
      services.AddScoped<SocialAuthService, FacebookSocialAuthService>();
      services.AddScoped<SocialAuthService, GoogleSocialAuthService>();
      services.AddScoped<SocialAuthService, VkSocialAuthService>();
    }

    public static void AddCustomAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
      
      var key = services.ConfigureAppSettings(configuration);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(x =>
        {
          x.RequireHttpsMetadata = false;
          x.SaveToken = true;
          x.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
          };
        })
        .AddCookie(x =>
        {
          x.LoginPath = new PathString("/admin/auth/login");
          // x.LogoutPath = new PathString("/admin/auth/logout");
        });
    }

    public static void AddCustomAuthorizationService(this IServiceCollection services)
    {
      services.AddTransient<IAuthorizationHandler, SubscriptionHandler>();
      services.AddAuthorization(options =>
      {
        options.AddPolicy(SubscriptionHandler.POLICY_NAME, policy => policy.Requirements.Add(new SubscriptionRequirement()));
      });
    }

    public static void AddHelperServices(this IServiceCollection services)
    {
      services.AddSingleton<IPasswordHasher<User>, PasswordHasherService>();
      services.AddTransient<ImageService>();
      services.AddScoped<UserService>();
      services.AddScoped<EmailService>();
      services.AddScoped<ConfirmationService>();
      services.AddScoped<PaymentService>();
    }
  }
}