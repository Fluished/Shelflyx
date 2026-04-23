namespace Shelflyx.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SeriesId { get; set; }
        public int Rating { get; set; }                    // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Series Series { get; set; } = null!;
    }
}
