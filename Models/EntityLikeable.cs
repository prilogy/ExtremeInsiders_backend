using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Models
{
  public class EntityLikeable : EntityBase 
  {
    [ForeignKey("EntityId")]
    public virtual List<Like> Likes { get; set; }

    public int LikesAmount => Likes.Count;
  }
}