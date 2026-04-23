namespace Shelflyx.Models
{
    public class ReadingProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChapterId { get; set; }
        public int LastPageRead { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Chapter Chapter { get; set; } = null!;
    }
}
