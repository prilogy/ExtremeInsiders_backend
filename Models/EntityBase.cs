using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Models
{
  public class EntityBase
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime DateCreated {get;set;}

    public EntityBase()
    {
      DateCreated = DateTime.UtcNow;
    }
  }
}