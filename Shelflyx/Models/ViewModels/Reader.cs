namespace Shelflyx.Models.ViewModels
{
    public class Reader
    {
        public Chapter Chapter { get; set; } = new();
        public List<Page> Pages { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public Chapter? PrevChapter { get; set; }
        public Chapter? NextChapter { get; set; }
        public bool HasAccess { get; set; }
    }
}
