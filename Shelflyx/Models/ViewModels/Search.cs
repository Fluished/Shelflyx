namespace Shelflyx.Models.ViewModels
{
    public class Search
    {
        public string? Query { get; set; }
        public string? Genre { get; set; }
        public string? Author { get; set; }
        public string? Filter { get; set; }
        public List<Series> Results { get; set; } = new();
    }
}
