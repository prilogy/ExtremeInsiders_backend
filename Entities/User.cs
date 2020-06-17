using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExtremeInsiders.Entities
{
  public class User
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
    
    [JsonIgnore]
    public int RoleId { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; }
    
    [NotMapped]
    public string Token { get; set; }
    [JsonIgnore]
    public virtual List<SocialAccount> SocialAccounts { get; set; }
  }
}