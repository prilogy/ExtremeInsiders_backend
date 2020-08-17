using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Sport : EntityBase, ITranslatableEntity<Sport, SportTranslation>
  {
    [JsonIgnore] public virtual List<Playlist> Playlists { get; set; }
    [JsonIgnore] public virtual List<Movie> Movies { get; set; }

    [NotMapped] public int? BestPlaylistId => Playlists.OrderByDescending(x => x.LikesAmount).FirstOrDefault()?.Id;

    [NotMapped]
    public int? BestVideoId => Playlists.Select(x => x.Videos.OrderByDescending(v => v.LikesAmount).First())
      .OrderByDescending(x => x.LikesAmount).FirstOrDefault()?.Id;

    public List<int> PlaylistsIds => Playlists?.Count > 0 ? Playlists.Select(x => x.Id).ToList() : null;
    public List<int> MoviesIds => Movies?.Count > 0 ? Movies.Select(x => x.Id).ToList() : null;

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SportTranslation> Translations { get; set; }

    [NotMapped] public SportTranslation Content { get; set; }
  }

  public class SportTranslation : TranslatableEntityTranslation<Sport>, IDefaultTranslatableContent,
    ITranslationWithImage
  {
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonIgnore] public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}