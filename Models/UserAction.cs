using System;
using ExtremeInsiders.Entities;
using Newtonsoft.Json;

namespace ExtremeInsiders.Models
{
  public class UserAction
  {
    public EntityIdItem EntityIdItem;
    public bool Status { get; set; }
    [JsonIgnore]
    public EntityBase Entity { get; set; }

    public string EntityType => Entity.EntityType;
  }
}