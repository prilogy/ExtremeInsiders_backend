using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExtremeInsiders.Services
{
  public interface IUserService
  {
    User User { get; }
    Task<User> Authenticate(string email, string password);
    User Authenticate(User user);
    Task<User> Create(AuthenticationModels.SignUp model);
  }

  public class UserService : IUserService
  {
    private readonly AppSettings _appSettings;
    private readonly ApplicationContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher<User> _passwordHasherService;
    private readonly ImageService _imageService;
    
    public UserService(IOptions<AppSettings> appSettings, ApplicationContext db, IHttpContextAccessor httpContextAccessor, IPasswordHasher<User> passwordHasherService, ImageService imageService)
    {
      _appSettings = appSettings.Value;
      _db = db;
      _httpContextAccessor = httpContextAccessor;
      _passwordHasherService = passwordHasherService;
      _imageService = imageService;
    }
    
    private User _user { get; set; } = null;

    public User User => _user ??= _db.Users.SingleOrDefault(u => u.Id == int.Parse(_httpContextAccessor.HttpContext.User.Identity.Name));
    public int UserId => int.Parse(_httpContextAccessor.HttpContext.User.Identity.Name);
    public Culture Culture => Culture.All.FirstOrDefault(x => x.Key == CultureKey) ?? _db.Cultures.FirstOrDefault(x => x.Key == CultureKey) ?? Culture.Default;

    public DateTime DateSubscriptionEnd
    {
      get
      {
        if (_httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == ClaimTypes.Expiration))
          return DateTime.Parse(
            _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Expiration)?.Value);
        
        return default;
      }
    }
    private string CultureKey
    {
      get
      {
        if(_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
          return _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Locality).Value;
        if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Culture"))
          return _httpContextAccessor.HttpContext.Request.Headers["Culture"];
        
        return null;
      }
    }

    public async Task<User> Authenticate(string email, string password)
    {
      var user = await VerifyUser(email, password);
      if (user == null) return null;
      
      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenDescriptor = GenerateTokenDescriptor(user, GetCultureHeader());
      var token = tokenHandler.CreateToken(tokenDescriptor);
      user.Token = tokenHandler.WriteToken(token);
      return user;
    }
    
    public User Authenticate(User user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenDescriptor = GenerateTokenDescriptor(user, GetCultureHeader());
      var token = tokenHandler.CreateToken(tokenDescriptor);
      user.Token = tokenHandler.WriteToken(token);
      return user;
    }

    public async Task<User> AuthenticateCookies(string email, string password, bool adminOnly)
    {
      var user = await VerifyUser(email, password);
      if (user == null) return null;
      
      var claimsIdentity = GenerateClaimsIdentity(user, null);
      var authProperties = new AuthenticationProperties
      {
        AllowRefresh = true,
        ExpiresUtc = DateTimeOffset.Now.AddDays(7),
      };

      if (user.Role.Name != Role.AdminRole || adminOnly != true)
        return null;
      
      await _httpContextAccessor.HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

      return user;
    }

    public async Task<User> Create(AuthenticationModels.SignUp model)
    {
      model.Email = model.Email.ToLower();
      
      if (await _db.Users.AnyAsync(u => u.Email == model.Email))
        return null;

      var user = new User
      {
        Email = model.Email,
        Name = model.Name, 
        Password = model.Password, 
        Role = await _db.Roles.SingleAsync(r => r.Name == Role.UserRole),
        PhoneNumber = model.PhoneNumber,
        DateBirthday = DateTime.ParseExact(model.DateBirthday, "dd.MM.yyyy", null),
      };

      if (model.Avatar != null)
      {
        user.Avatar = await _imageService.AddImage(model.Avatar);
      }
      user.Password = HashPassword(user, model.Password);
      
      return user;
    }

    public string HashPassword(User user, string password) => _passwordHasherService.HashPassword(user, password);
    
    private async Task<User> VerifyUser(string email, string password)
    {
      email = email.ToLower();

      var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
      if (user == null) return null;

      var passwordVerify = _passwordHasherService.VerifyHashedPassword(user, user.Password, password);
      return passwordVerify == PasswordVerificationResult.Failed ? null : user;
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user, Culture culture)
    {
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = GenerateClaimsIdentity(user,culture),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      return tokenDescriptor;
    }

    private ClaimsIdentity GenerateClaimsIdentity(User user, Culture culture)
    {
      var claims = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role.Name),
        new Claim(ClaimTypes.Expiration, user.Subscription != null ? user.Subscription.DateEnd.ToString(CultureInfo.InvariantCulture) : ""), 
      }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
      
      if(culture != null)
        claims.AddClaim(new Claim(ClaimTypes.Locality,culture.Key));
      return claims;
    }
    
    private Culture GetCultureHeader()
    {
      var cultureHeader = (string)_httpContextAccessor.HttpContext.Request.Headers["Culture"];
      var culture = Culture.All.FirstOrDefault(c => c.Key == cultureHeader);

      return culture ?? Culture.Russian;
    }
  }
}