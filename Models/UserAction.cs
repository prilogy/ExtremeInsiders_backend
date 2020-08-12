using System;
using Newtonsoft.Json;

namespace ExtremeInsiders.Models
{
  public class UserAction
  {
    public int Id { get; set; }
    public bool Status { get; set; }
    [JsonIgnore]
    public EntityBase Entity { get; set; }

    public string EntityType => Entity.EntityType;
  }
}