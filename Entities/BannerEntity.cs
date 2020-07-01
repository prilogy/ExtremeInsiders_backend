using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;

namespace ExtremeInsiders.Entities
{
  public class BannerEntity
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int EntityId { get; set; }
    public virtual EntityBase Entity { get; set; }
  }
}