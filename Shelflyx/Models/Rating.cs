namespace Shelflyx.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int SeriesId { get; set; }
        public int Stars { get; set; }
        public DateTime DateRated { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Series? Series { get; set; }
    }
}
