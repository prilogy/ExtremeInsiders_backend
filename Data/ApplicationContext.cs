using System.Collections.Generic;
using ExtremeInsiders.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Data
{
  public class ApplicationContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<SocialAccount> SocialAccounts { get; set; }
    public DbSet<SocialAccountProvider> SocialAccountProviders { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Culture> Cultures { get; set; }
    
    /* Sports */
    public DbSet<Sport> Sports { get; set; }
    public DbSet<SportTranslation> SportsTranslations { get; set; }
    
    /* Playlists */
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTranslation> PlaylistsTranslations { get; set; }
    
    /* Videos and Movies */
    public DbSet<Video> Videos { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<VideoTranslation> VideoTranslations { get; set; }
    
    public DbSet<Like> Likes { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<SocialAccountProvider>().HasData(SocialAccountProvider.AllProviders);
      modelBuilder.Entity<Role>().HasData(Role.User, Role.Admin);
      modelBuilder.Entity<Culture>().HasData(Culture.Russian, Culture.English);

      // unuique pair of props
      modelBuilder.Entity<Like>().HasIndex(p => new {p.UserId, p.VideoId}).IsUnique();
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
      //Database.EnsureCreated();
    }
  }
}