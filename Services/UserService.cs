using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GoogleAuth.Data;
using GoogleAuth.Entities;
using GoogleAuth.Helpers;
using GoogleAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoogleAuth.Services
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

    public UserService(IOptions<AppSettings> appSettings, ApplicationContext db, IHttpContextAccessor httpContextAccessor, IPasswordHasher<User> passwordHasherService)
    {
      _appSettings = appSettings.Value;
      _db = db;
      _httpContextAccessor = httpContextAccessor;
      _passwordHasherService = passwordHasherService;
    }

    public async Task<User> Authenticate(string email, string password)
    {
      email = email.ToLower();
      
      var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
      if (user == null) return null;
      
      var passwordVerify = _passwordHasherService.VerifyHashedPassword(user, user.Password, password);
      if (passwordVerify == PasswordVerificationResult.Failed)
        return null;
      
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
        Role = await _db.Roles.SingleAsync(r => r.Name == Role.User)
      };

      user.Password = _passwordHasherService.HashPassword(user, user.Password);
      
      return user;
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user)
    {
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
          new Claim(ClaimTypes.Name, user.Id.ToString()),
          new Claim(ClaimTypes.Role, user.Role.Name), 
        }),
        Expires = DateTime.Now.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      return tokenDescriptor;
    }
  }
}