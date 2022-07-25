using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CuBuy.Models;

namespace CuBuy.Data
{  
    public class AppDbContext : IdentityDbContext 
    {
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Notification> Notifications {get; set;}
        // public DbSet<Notification> Notifications {get; set;}
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.User)
                .WithMany()
                .OnDelete(DeleteBehavior.ClientCascade);            
        }
    }
}