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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<SocialAccountProvider>().HasData(SocialAccountProvider.AllProviders);
      modelBuilder.Entity<Role>().HasData(Role.AllRoles);
      
      //modelBuilder.Entity<UserSocialAccount>()
       // .HasKey(x => new {x.UserId, x.SocialAccountId});
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
      Database.EnsureCreated();
    }
  }
}