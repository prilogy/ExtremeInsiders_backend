using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Models
{
  
  public interface ITranslatableEntity<T, TR>
    where T: ITranslatableEntity<T, TR>
    where TR: TranslatableEntityTranslation<T> 
  {
    List<TR> Translations { get; set; }
    TR Content { get; set; }
  }

  public abstract class TranslatableEntityTranslation<T>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonIgnore]
    public int BaseEntityId { get; set; }
    [JsonIgnore]
    public virtual T BaseEntity { get; set; }
    
    [JsonIgnore]
    public int CultureId { get; set; }
    public virtual Culture Culture { get; set; }
  }

}