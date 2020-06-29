using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  [MetadataType(typeof(ITranslatableEntity<Movie, MovieTranslation>))]
  public class Movie : EntityLikeable, ITranslatableEntity<Movie, MovieTranslation>
  {
    public int SportId { get; set; }
    [JsonIgnore]
    public virtual Sport Sport { get; set; }

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<MovieTranslation> Translations { get; set; }
    [NotMapped]
    public MovieTranslation Content { get; set; }
  }
  
  public class MovieTranslation : TranslatableEntityTranslation<Movie>, IDefaultTranslatableContent, ITranslationWithUrl, ITranslationWithImage
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
    
    public string Url { get; set; }
  }
}