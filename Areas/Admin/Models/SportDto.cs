using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Areas.Admin.Models
{
  public class SportEditDto
  {
    [Required]
    public Sport Sport { get; set; }
    [Required]
    public List<SportTranslation> SportTranslations { get; set; }
    
    public IFormFile ImageSrc { get; set; }
  }
}