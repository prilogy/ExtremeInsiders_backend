using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  [MetadataType(typeof(ITranslatableEntity<Video, VideoTranslation>))]
  public class Video : EntityLikeable, ITranslatableEntity<Video, VideoTranslation>
  {
    public int PlaylistId { get; set; }
    [JsonIgnore]
    public virtual Playlist Playlist { get; set; }

    public bool IsInPaidPlaylist => Playlist?.IsPaid ?? false;
    
    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<VideoTranslation> Translations { get; set; }
    [NotMapped]
    public VideoTranslation Content { get; set; }
  }

  public class VideoTranslation : TranslatableEntityTranslation<Video>, IDefaultTranslatableContent, ITranslationWithUrl, ITranslationWithImage
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
    
    public string Url { get; set; }
  }
}