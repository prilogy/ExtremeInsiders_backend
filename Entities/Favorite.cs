using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Favorite
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public int EntityId { get; set; }
    public virtual EntityBase Entity { get; set; }
  }
}