using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
    public int? AvatarId { get; set; }
    public virtual Image Avatar { get; set; }
    
    public DateTime DateBirthday { get; set; }
    public DateTime DateSignUp { get; set; }
    public string PhoneNumber { get; set; }
    
    [JsonIgnore]
    public int RoleId { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; }
    
    [NotMapped]
    public string Token { get; set; }
    public virtual List<SocialAccount> SocialAccounts { get; set; }
    [JsonIgnore]
    public virtual List<Like> Likes { get; set; }

    [NotMapped]
    public object LikesIds => new
    {
      Videos = Likes.Select(x => x.Id).ToList() 
    };

    public User()
    {
      DateSignUp = DateTime.UtcNow;
    }

    public User(User u)
    {
      Id = u.Id;
      Email = u.Email;
      Name = u.Name;
      Password = u.Password;
      AvatarId = u.AvatarId;
      Avatar = u.Avatar;
      DateBirthday = u.DateBirthday;
      DateSignUp = u.DateSignUp;
      PhoneNumber = u.PhoneNumber;
      RoleId = u.RoleId;
      Role = u.Role;
      Token = u.Token;
      SocialAccounts = u.SocialAccounts;
      Likes = u.Likes;
    }

    public User WithoutSensitive(bool token)
    {
      var r = new User(this) {Password = null};
      // if (token == false)
      //   r.Token = null;
      r.Token = token ? r.Token : null;
      return r;
    }
  }
}