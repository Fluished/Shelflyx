namespace Shelflyx.Models
{
    public class WalletTransaction
    {
        public int WalletTransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}
