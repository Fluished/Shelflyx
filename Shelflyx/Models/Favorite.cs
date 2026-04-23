namespace Shelflyx.Models
{
    public class Favorite
    {
        public int FavoriteId { get; set; }
        public int UserId { get; set; }
        public int SeriesId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Series? Series { get; set; }
    }
}
