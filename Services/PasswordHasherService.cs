using System;
using System.Linq;
using System.Security.Cryptography;
using GoogleAuth.Entities;
using GoogleAuth.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoogleAuth.Services
{
  public sealed class PasswordHasherService : IPasswordHasher<User>
  {
    private const int SaltSize = 16; // 128 bit 
    private const int KeySize = 32; // 256 bit
    
    private const int Iterations = 10000;
    
    public string HashPassword(User user, string password)
    {
      using var algorithm = new Rfc2898DeriveBytes(
        password,
        SaltSize,
        Iterations,
        HashAlgorithmName.SHA512);
      var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
      var salt = Convert.ToBase64String(algorithm.Salt);

      return $"{Iterations}.{salt}.{key}";
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
      var parts = hashedPassword.Split('.', 3);

      if (parts.Length != 3) return PasswordVerificationResult.Failed;
      

      var iterations = Convert.ToInt32(parts[0]);
      var salt = Convert.FromBase64String(parts[1]);
      var key = Convert.FromBase64String(parts[2]);

      var needsUpgrade = iterations != Iterations;

      using var algorithm = new Rfc2898DeriveBytes(
        providedPassword,
        salt,
        iterations,
        HashAlgorithmName.SHA512);
      var keyToCheck = algorithm.GetBytes(KeySize);

      var verified = keyToCheck.SequenceEqual(key);

      return verified
        ? needsUpgrade ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Success
        : PasswordVerificationResult.Failed;
    }
  }
}