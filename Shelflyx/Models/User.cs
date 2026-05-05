using System.ComponentModel.DataAnnotations;

namespace Shelflyx.Models
{
    public class User
    {
        public string Biography { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string ProfilePic { get; set; } = "/uploads/profiles/default.png";
        public decimal Balance { get; set; } = 0;

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    }
}
