using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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

    public ConfirmationCode()
    {
      Code = GenerateCode();
      IsConfirmed = false;
    }

    /* Helper functionality */
    private readonly Random _random = new Random();
    
    private string GenerateCode()
    {
      return _random.Next(0, 99999).ToString("D5");
    }

    public enum Types
    {
      EmailConfirmation,
      PasswordReset
    } 
  }
}