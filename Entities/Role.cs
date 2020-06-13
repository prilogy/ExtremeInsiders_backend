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

    public static readonly List<Role> AllRoles = new List<Role>
    {
      new Role {Id = 1, Name = Role.User},
      new Role {Id = 2, Name = Role.Admin}
    };
    
    public const string Admin = "admin";
    public const string User = "user";
  }
}