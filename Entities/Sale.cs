using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Sale
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public int EntityId { get; set; }
    public virtual EntitySaleable Entity { get; set; }
  }
}