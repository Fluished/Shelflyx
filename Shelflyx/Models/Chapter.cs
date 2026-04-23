using System.ComponentModel.DataAnnotations;

namespace Shelflyx.Models
{
    public class Chapter
    {
        public int ChapterId { get; set; }

        public int SeriesId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public int ChapterNumber { get; set; }

        public decimal Price { get; set; } = 0; // 0 = free

        public DateTime DatePublished { get; set; } = DateTime.UtcNow;

        // Navigation
        public Series? Series { get; set; }
        public ICollection<Page> Pages { get; set; } = new List<Page>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
