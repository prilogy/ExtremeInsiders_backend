using System.Collections.Generic;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Models;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Data
{
  public class ApplicationContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<SocialAccount> SocialAccounts { get; set; }
    public DbSet<SocialAccountProvider> SocialAccountsProviders { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Culture> Cultures { get; set; }
    
    /* Entities */
    public DbSet<EntityBase> EntitiesBase { get; set; }
    public DbSet<EntityLikeable> EntitiesLikeable { get; set; }

    /* Banner entity */
    public DbSet<BannerEntity> BannerEntities { get; set; }
    public DbSet<BannerEntityTranslation> BannerEntitiesTranslations { get; set; }
    
    public DbSet<Sale> Sales { get; set; }
    public DbSet<EntitySaleable> EntitiesSaleable { get; set; }
    public DbSet<EntitySaleablePrice> EntitySaleablePrices { get; set; }
    
    /* Sports */
    public DbSet<Sport> Sports { get; set; }
    public DbSet<SportTranslation> SportsTranslations { get; set; }
    
    /* Playlists */
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTranslation> PlaylistsTranslations { get; set; }
    
    /* Videos and Movies */
    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoTranslation> VideosTranslations { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieTranslation> MoviesTranslations { get; set; }
    
    
    public DbSet<Like> Likes { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionsPlans { get; set; }
    public DbSet<SubscriptionPlanTranslation> SubscriptionsPlansTranslations { get; set; }
    public DbSet<SubscriptionPlanPrice> SubscriptionsPlansPrices { get; set; }
    
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<SocialAccountProvider>().HasData(SocialAccountProvider.All);
      modelBuilder.Entity<Role>().HasData(Role.User, Role.Admin);
      modelBuilder.Entity<Culture>().HasData(Culture.Russian, Culture.English);
      
      var sport = new Sport {Id = 1 };
      var playlist = new Playlist { Id = 2, SportId = sport.Id };
      var video = new Video { Id = 3, PlaylistId = playlist.Id};
      var movie = new Movie { Id = 4, SportId = sport.Id};
      
      modelBuilder.Entity<Sport>().HasData(sport);
      modelBuilder.Entity<Playlist>().HasData(playlist);
      modelBuilder.Entity<Video>().HasData(video);
      modelBuilder.Entity<Movie>().HasData(movie);

      modelBuilder.Entity<SportTranslation>().HasIndex(x => new {x.CultureId, x.BaseEntityId}).IsUnique();
      modelBuilder.Entity<PlaylistTranslation>().HasIndex(x => new {x.CultureId, x.BaseEntityId}).IsUnique();
      modelBuilder.Entity<MovieTranslation>().HasIndex(x => new {x.CultureId, x.BaseEntityId}).IsUnique();
      modelBuilder.Entity<VideoTranslation>().HasIndex(x => new {x.CultureId, x.BaseEntityId}).IsUnique();
      modelBuilder.Entity<BannerEntityTranslation>().HasIndex(x => new {x.CultureId, x.BaseEntityId}).IsUnique();
      
      modelBuilder.Entity<Like>().HasIndex(x => new {x.UserId, x.EntityId}).IsUnique();
      modelBuilder.Entity<EntitySaleablePrice>().HasIndex(x => new {x.CurrencyId, x.EntityId}).IsUnique();
      
      modelBuilder.Entity<Currency>().HasData(Currency.All);
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
      //Database.EnsureCreated();
    }
  }
}