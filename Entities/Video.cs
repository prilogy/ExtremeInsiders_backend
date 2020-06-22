using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Video : TranslatableEntity<Video, VideoTranslation>
  {
    [JsonIgnore]
    public int PlaylistId { get; set; }
    public virtual Playlist Playlist { get; set; }
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