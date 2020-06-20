using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
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
    private User _user { get; set; } = null;

    public User User => _user ??= _db.Users.SingleOrDefault(u => u.Id == int.Parse(_httpContextAccessor.HttpContext.User.Identity.Name));

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

    public async Task<User> Authenticate(string email, string password)
    {
      var user = await VerifyUser(email, password);
      if (user == null) return null;
      
      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenDescriptor = GenerateTokenDescriptor(user);
      
      var token = tokenHandler.CreateToken(tokenDescriptor);
      user.Token = tokenHandler.WriteToken(token);
      return user.WithoutPassword();
    }
    
    public User Authenticate(User user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenDescriptor = GenerateTokenDescriptor(user);
      
      var token = tokenHandler.CreateToken(tokenDescriptor);
      user.Token = tokenHandler.WriteToken(token);
      return user.WithoutPassword();
    }

    public async Task<User> AuthenticateCookies(string email, string password)
    {
      var user = await VerifyUser(email, password);
      if (user == null) return null;
      
      var claimsIdentity = GenerateClaimsIdentity(user);
      var authProperties = new AuthenticationProperties
      {
        AllowRefresh = true,
        ExpiresUtc = DateTimeOffset.Now.AddDays(7),
      };

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
        BirthDate = DateTime.ParseExact(model.BirthDate, "dd.MM.yyyy", null),
      };

      if (model.Avatar != null)
      {
        user.Avatar = await _imageService.AddImage(model.Avatar);
      }

      user.Password = _passwordHasherService.HashPassword(user, user.Password);
      
      return user;
    }
    
    private async Task<User> VerifyUser(string email, string password)
    {
      email = email.ToLower();

      var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
      if (user == null) return null;

      var passwordVerify = _passwordHasherService.VerifyHashedPassword(user, user.Password, password);
      return passwordVerify == PasswordVerificationResult.Failed ? null : user;
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user)
    {
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = GenerateClaimsIdentity(user),
        Expires = DateTime.Now.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      return tokenDescriptor;
    }

    private ClaimsIdentity GenerateClaimsIdentity(User user)
    {
      return new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role.Name),
      }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
  }
}