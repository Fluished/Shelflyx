namespace Shelflyx.Models
{
    public class Page
    {
        public int PageId { get; set; }
        public int ChapterId { get; set; }
        public int PageNumber { get; set; }
        public string ImagePath { get; set; } = "";

        // Navigation
        public Chapter? Chapter { get; set; }
    }
}
