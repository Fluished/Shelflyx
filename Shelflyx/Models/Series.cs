using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Shelflyx.Models
{
    public class Series
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Author { get; set; }
        public required string[] Genre { get; set; }     // consider a Genre entity instead
        public DateTime ReleaseDate { get; set; }
        public SeriesStatus Status { get; set; }          // Ongoing, Completed, Hiatus

        // Navigation
        public ICollection<Chapter> Chapters { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
