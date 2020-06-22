using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ExtremeInsiders.Entities
{
  // TODO: refactor to generic
  public class Like
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public int VideoId { get; set; }
    [JsonIgnore]
    public virtual Video Video { get; set; }
  }
    
}