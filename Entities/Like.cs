using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Entities
{
  public enum LikeEntityType
  {
    Video,
    Movie
  }

  // TODO: refactor
  
  public class Like
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public int EntityId { get; set; }
    public LikeEntityType EntityType { get; set; }
  }

  
}