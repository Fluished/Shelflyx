using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Shelflyx.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string UserId { get; set; }       // external auth ID (e.g. ASP.NET Identity)
        public required string Username { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public decimal Balance { get; set; } = 0;

        // Navigation
        public ICollection<Purchase> Purchases { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<ReadingProgress> ReadingProgresses { get; set; } = [];
    }
}
