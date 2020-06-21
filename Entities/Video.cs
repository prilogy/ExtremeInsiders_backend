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

  public class VideoTranslation : TranslatableEntityTranslation
  {
    public string Name { get; set; }
    public string Description { get; set; }
  }
}