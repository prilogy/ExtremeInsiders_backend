using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Entities;
using Newtonsoft.Json;

namespace ExtremeInsiders.Models
{
  public class  EntityLikeable : EntitySaleable
  {
    [ForeignKey("EntityId")]
    [JsonIgnore]
    public virtual List<Like> Likes { get; set; }

    public int LikesAmount => Likes.Count;
  }
}