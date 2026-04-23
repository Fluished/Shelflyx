namespace Shelflyx.Models
{
    public class ChapterPage
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int PageNumber { get; set; }
        public required string ImageUrl { get; set; }

        public Chapter Chapter { get; set; } = null!;
    }
}
