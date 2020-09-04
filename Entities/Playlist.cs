using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Playlist : EntitySaleable, ITranslatableEntity<Playlist, PlaylistTranslation>
  {
    public int SportId { get; set; }
    [JsonIgnore] public virtual Sport Sport { get; set; }

    [JsonIgnore] public virtual List<Video> Videos { get; set; }

    public List<int> VideosIds => Videos?.Count > 0 ? Videos.Select(x => x.Id).ToList() : null;
    [NotMapped] public int? BestVideoId => Videos.OrderByDescending(v => v.LikesAmount).FirstOrDefault()?.Id;

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<PlaylistTranslation> Translations { get; set; }

    [NotMapped] public PlaylistTranslation Content { get; set; }

    [NotMapped] public int LikesAmount => Videos.Aggregate(0, (acc, x) => acc + x.LikesAmount);
  }

  public class PlaylistTranslation : TranslatableEntityTranslation<Playlist>, IDefaultTranslatableContent,
    ITranslationWithImage
  {
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonIgnore] public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}