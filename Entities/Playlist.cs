using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Playlist : TranslatableEntity<Playlist, PlaylistTranslation>
  {
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
    
    [JsonIgnore]
    public int SportId { get; set; }
    public virtual Sport Sport { get; set; }
    
    [ForeignKey("PlaylistId")]
    public virtual List<Video> Videos { get; set; }
    public virtual List<Movie> Movies { get; set; } 
  }
  
  public class PlaylistTranslation : TranslatableEntityTranslation
  {
    public string Name { get; set; }
    public string Description { get; set; }
  }
}