namespace Shelflyx.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChapterId { get; set; }
        public decimal PricePaid { get; set; }             // snapshot at time of purchase
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public Chapter Chapter { get; set; } = null!;
    }
}
