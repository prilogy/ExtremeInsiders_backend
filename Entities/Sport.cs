using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Entities
{
  public class Sport : TranslatableEntity<Sport, SportTranslation>
  {
    [JsonIgnore] public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }

  public class SportTranslation : TranslatableEntityTranslation
  {
    public string Name { get; set; }

    [JsonIgnore] 
    public int SportId { get; set; }
    [JsonIgnore]
    public virtual Sport Sport { get; set; }
  }
}