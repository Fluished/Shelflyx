using System.Transactions;

namespace Shelflyx.Models
{
    public class TopUpTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public required string PaymentReference { get; set; }  // from payment gateway
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
