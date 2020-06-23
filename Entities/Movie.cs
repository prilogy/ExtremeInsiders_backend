using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class Movie : TranslatableEntity<Movie, MovieTranslation>
  {
    [JsonIgnore]
    public int SportId { get; set; }
    [JsonIgnore]
    public virtual Sport Sport { get; set; }
    
    [JsonIgnore]
    public virtual List<LikeMovie> Likes { get; set; }

    public int LikesAmount => Likes.Count;
  }
  
  public class MovieTranslation : TranslatableEntityTranslation<Movie>
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}