using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Sport : EntityBase, ITranslatableEntity<Sport, SportTranslation>
  {
    
    [JsonIgnore]
    public virtual List<Playlist> Playlists { get; set; }
    [JsonIgnore]
    public virtual List<Movie> Movies { get; set; }

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SportTranslation> Translations { get; set; }
    [NotMapped]
    public SportTranslation Content { get; set; }
  }

  public class SportTranslation : TranslatableEntityTranslation<Sport>
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}