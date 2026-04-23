using Microsoft.EntityFrameworkCore;
using Shelflyx.Models;

namespace Shelflyx.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Decimal precision
            modelBuilder.Entity<User>().Property(u => u.Balance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Chapter>().Property(c => c.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Purchase>().Property(p => p.AmountPaid).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<WalletTransaction>().Property(w => w.Amount).HasColumnType("decimal(18,2)");

            // Unique constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Favorite>().HasIndex(f => new { f.UserId, f.SeriesId }).IsUnique();
            modelBuilder.Entity<Rating>().HasIndex(r => new { r.UserId, r.SeriesId }).IsUnique();

            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@shelflyx.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                Balance = 99999,
                CreatedAt = DateTime.UtcNow
            });

            // Seed sample series
            modelBuilder.Entity<Series>().HasData(
                new Series { SeriesId = 1, Title = "Shadow Blade Chronicles", Genre = "Action", Author = "Kenji Tanaka", Description = "A young warrior discovers a legendary blade that grants immense power — but at a terrible cost.", CoverImage = "/images/cover1.png", DateCreated = DateTime.UtcNow },
                new Series { SeriesId = 2, Title = "Crimson Petal", Genre = "Romance", Author = "Yuki Hayashi", Description = "Two rivals find themselves falling for each other against the backdrop of a prestigious music academy.", CoverImage = "/images/cover2.png", DateCreated = DateTime.UtcNow },
                new Series { SeriesId = 3, Title = "Void Walker", Genre = "Fantasy", Author = "Ren Mishima", Description = "An explorer crosses into a parallel dimension where magic is science and monsters rule the skies.", CoverImage = "/images/cover3.png", DateCreated = DateTime.UtcNow }
            );
        }
    }
}
