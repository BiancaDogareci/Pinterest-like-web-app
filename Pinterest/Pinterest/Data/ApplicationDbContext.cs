
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pinterest.Models;
using System.Net.NetworkInformation;

namespace Pinterest.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Pin> Pins { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PinCategory> PinCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<PinCategory>()
                .HasKey(ab => new {
                    ab.Id,
                    ab.PinId,
                    ab.CategoryId
                });

            // definire relatii cu modelele Category si Pin (FK)
            modelBuilder.Entity<PinCategory>()
                .HasOne(ab => ab.Pin)
                .WithMany(ab => ab.PinCategories)
                .HasForeignKey(ab => ab.PinId);

            modelBuilder.Entity<PinCategory>()
                .HasOne(ab => ab.Category)
                .WithMany(ab => ab.PinCategories)
                .HasForeignKey(ab => ab.CategoryId);
        }

    }
}