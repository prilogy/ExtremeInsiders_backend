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
    
    public DbSet<Sport> Sports { get; set; }
    public DbSet<SportTranslation> SportsTranslations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<SocialAccountProvider>().HasData(SocialAccountProvider.AllProviders);
      modelBuilder.Entity<Role>().HasData(Role.User, Role.Admin);
      modelBuilder.Entity<Culture>().HasData(Culture.Russian, Culture.English);
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
      //Database.EnsureCreated();
    }
  }
}