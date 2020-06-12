using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GoogleAuth.Entities
{
  public class SocialAccount
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Key { get; set; }
    [JsonIgnore]
    public int ProviderId { get; set; }
    public virtual SocialAccountProvider Provider { get; set; }
    
    [JsonIgnore]
    public int? UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } 
  }
}