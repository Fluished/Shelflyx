using System.ComponentModel.DataAnnotations;

namespace Shelflyx.Models
{
    public class Series
    {
        public int SeriesId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Genre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CoverImage { get; set; } = "/images/default-cover.png";

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
