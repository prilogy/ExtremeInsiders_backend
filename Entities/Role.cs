using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ExtremeInsiders.Entities
{
  public class Role
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }

    public static Role User = new Role
    {
      Id = 1,
      Name = UserRole
    };

    public static Role Admin = new Role
    {
      Id = 2,
      Name = AdminRole
    };
    
    public const string AdminRole = "admin";
    public const string UserRole = "user";
  }
}