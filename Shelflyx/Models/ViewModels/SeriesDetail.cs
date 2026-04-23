namespace Shelflyx.Models.ViewModels
{
    public class SeriesDetail
    {
        public Series Series { get; set; } = new();
        public List<Chapter> Chapters { get; set; } = new();
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public bool IsFavorited { get; set; }
        public int? UserRating { get; set; }
    }
}
