using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Entities
{
  public enum LikeForEntityType
  {
    Video,
    Movie
  }

  // TODO: refactor to generic
  public interface ILikeFor<T> where T: class
  {
    
    LikeForEntityType EntityType { get; }
    int EntityId { get; set; }
    T Entity { get; set; }
  }
  
  public class Like
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
  }

  public class LikeVideo : Like , ILikeFor<Video>
  {
    public LikeForEntityType EntityType => LikeForEntityType.Video;
    public int EntityId { get; set; }
    [JsonIgnore]
    public virtual Video Entity { get; set; }
  }

  public class LikeMovie : Like, ILikeFor<Movie>
  {
    public LikeForEntityType EntityType => LikeForEntityType.Movie;
    public int EntityId { get; set; }
    public virtual Movie Entity { get; set; }
  }
}