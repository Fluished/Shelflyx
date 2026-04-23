namespace Shelflyx.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public required string Title { get; set; }
        public int ChapterNumber { get; set; }
        public decimal Price { get; set; } = 0;           // 0 = free/preview
        public bool IsFree { get; set; } = false;
        public DateTime PublishedDate { get; set; }
        public int PageCount { get; set; }

        // Navigation
        public Series Series { get; set; } = null!;
        public ICollection<ChapterPage> Pages { get; set; } = [];
        public ICollection<Purchase> Purchases { get; set; } = [];
    }
}
