using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Entities
{
  public class User
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Email { get; set; }
    public string Name { get; set; }
    [JsonIgnore] public string Password { get; set; }

    [JsonIgnore] public int? AvatarId { get; set; }
    public virtual Image Avatar { get; set; }

    public DateTime DateBirthday { get; set; }
    public DateTime DateSignUp { get; set; }
    public string PhoneNumber { get; set; }

    [JsonIgnore] public int RoleId { get; set; }
    [JsonIgnore] public virtual Role Role { get; set; }
    public virtual List<SocialAccount> SocialAccounts { get; set; }
    [JsonIgnore]
    public virtual List<Like> Likes { get; set; }
    [JsonIgnore]
    public virtual List<Favorite> Favorites { get; set; } 


    [NotMapped]
    public object LikesIds { get; set; }
    [NotMapped] 
    public object FavoriteIds { get; set; }
    [NotMapped] 
    public string Token { get; set; }


    public User()
    {
      DateSignUp = DateTime.UtcNow;
    }
    
    public User WithoutSensitive(bool token = false, bool useLikeIds = false, bool useFavoriteIds = false)
    {
      Password = null;
      Token = token ? Token : null;

      if (useLikeIds)
      {
        LikesIds = new
        {
          Videos = Likes.Where(x => x.Entity is Video).Select(x => x.EntityId),
          Movies = Likes.Where(x=> x.Entity is Movie).Select(x => x.EntityId)
        };
      }

      if (useFavoriteIds)
      {
        FavoriteIds = new
        {
          Videos = Favorites.Where(x => x.Entity is Video).Select(x => x.EntityId),
          Movies = Favorites.Where(x=> x.Entity is Movie).Select(x => x.EntityId),
          Sports = Favorites.Where(x => x.Entity is Sport).Select(x => x.EntityId),
          Playlists = Favorites.Where(x=> x.Entity is Playlist).Select(x => x.EntityId),
        };
        Likes = null;
      }

      return this;
    }
  }
}