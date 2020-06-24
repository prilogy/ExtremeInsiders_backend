using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class Like
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public int EntityId { get; set; }
    public virtual EntityLikeable Entity { get; set; }
  }
}