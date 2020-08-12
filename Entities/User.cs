using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

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
    [JsonIgnore]
    public virtual List<Sale> Sales { get; set; }
    [JsonIgnore]
    public virtual List<ConfirmationCode> ConfirmationCodes { get; set; }
    [JsonIgnore]
    public virtual List<Subscription> Subscriptions { get; set; }
    [JsonIgnore]
    public virtual List<Payment> Payments { get; set; }
    [JsonIgnore]
    public virtual List<PromoCodeUser> PromoCodes { get; set; }

    [JsonRequired]
    public Subscription Subscription => Subscriptions.Count > 0 && Subscriptions.LastOrDefault().DateEnd > DateTime.UtcNow ? Subscriptions.LastOrDefault() : null;
    
    [JsonIgnore]
    public int CultureId { get; set; }
    public virtual Culture Culture { get; set; }
    [JsonIgnore]
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    [NotMapped]
    public bool EmailVerified =>
      ConfirmationCodes.Any(x => x.Type == ConfirmationCode.Types.EmailConfirmation && x.IsConfirmed == true);
    [NotMapped]
    public EntityIdLists LikeIds { get; set; }
    [NotMapped] 
    public EntityIdLists FavoriteIds { get; set; }
    [NotMapped] 
    public EntityIdLists SaleIds { get; set; }
    [NotMapped] 
    public string Token { get; set; }


    public User()
    {
      DateSignUp = DateTime.UtcNow;
    }

    public User WithoutSensitive(bool token = false, bool useLikeIds = false, bool useFavoriteIds = false, bool useSaleIds = false)
    {
      Password = null;
      Token = token ? Token : null;

      if (useLikeIds)
      {
        LikeIds = new EntityIdLists
        {
          Videos = Likes.Where(x => x.Entity is Video).Select(x => x.EntityId),
          Movies = Likes.Where(x=> x.Entity is Movie).Select(x => x.EntityId)
        };
      }

      if (useFavoriteIds)
      {
        FavoriteIds = new EntityIdLists
        {
          Videos = Favorites.Where(x => x.Entity is Video).Select(x => x.EntityId),
          Movies = Favorites.Where(x=> x.Entity is Movie).Select(x => x.EntityId),
          Sports = Favorites.Where(x => x.Entity is Sport).Select(x => x.EntityId),
          Playlists = Favorites.Where(x=> x.Entity is Playlist).Select(x => x.EntityId),
        };
      }

      if (useSaleIds)
      {
        SaleIds = new EntityIdLists
        {
          Videos = Sales.Where(x => x.Entity is Video).Select(x => x.EntityId),
          Movies = Sales.Where(x=> x.Entity is Movie).Select(x => x.EntityId),
          Playlists = Sales.Where(x=> x.Entity is Playlist).Select(x => x.EntityId),
        };
        
      }

      return this;
    }
  }
  
  public class EntityIdLists
  {
    public IEnumerable<int> Sports { get; set; } = null;
    public IEnumerable<int> Playlists { get; set; } = null;
    public IEnumerable<int> Videos { get; set; } = null;
    public IEnumerable<int> Movies { get; set; } = null;
  }
}