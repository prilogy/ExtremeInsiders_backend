using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class BannerEntity : ITranslatableEntity<BannerEntity, BannerEntityTranslation>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<BannerEntityTranslation> Translations { get; set; }
    [NotMapped]
    public BannerEntityTranslation Content { get; set; }
    public int? EntityId { get; set; }
    public virtual EntityBase Entity { get; set; }
  }

  public class BannerEntityTranslation : TranslatableEntityTranslation<BannerEntity>, IDefaultTranslatableContent, ITranslationWithUrl, ITranslationWithImage
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}