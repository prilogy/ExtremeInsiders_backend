using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class Video : TranslatableEntity<Video, VideoTranslation>
  {
    [JsonIgnore]
    public int PlaylistId { get; set; }
    [JsonIgnore]
    public virtual Playlist Playlist { get; set; }
    
    [JsonIgnore]
    public virtual List<LikeVideo> Likes { get; set; }

    public int LikesAmount => Likes.Count;
  }

  public class VideoTranslation : TranslatableEntityTranslation<Video>
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}