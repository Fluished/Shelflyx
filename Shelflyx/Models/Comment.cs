namespace Shelflyx.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int ChapterId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Chapter? Chapter { get; set; }
    }
}
