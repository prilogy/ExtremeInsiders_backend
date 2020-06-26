using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ExtremeInsiders.Entities
{
  public class ConfirmationCode
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Code { get; set; }
    public Types Type { get; set; }
    public bool IsConfirmed { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public DateTime DateValidUntil { get; set; }

    public enum Types
    {
      EmailConfirmation,
      PasswordReset
    }

    /* Helper functionality */
    public ConfirmationCode()
    {
      Code = GenerateCode();
      IsConfirmed = false;
      DateValidUntil = DateTime.UtcNow + TimeSpan.FromDays(1);
    }
    
    private readonly Random _random = new Random();

    private string GenerateCode()
    {
      return _random.Next(0, 99999).ToString("D5");
    }

    public static Expression<Func<ConfirmationCode, bool>> CanBeUsed(string code, ConfirmationCode.Types type)
    {
      return x => x.Code == code && x.Type == type && x.IsConfirmed == false && DateTime.UtcNow < x.DateValidUntil;
    }
   
    public static bool CanBeUsed(ConfirmationCode x, string code, ConfirmationCode.Types type)
    {
      return  x.Code == code && x.Type == type && x.IsConfirmed == false && DateTime.UtcNow < x.DateValidUntil;
    }

  }
}