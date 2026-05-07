using System.Collections.Generic;

namespace Shelflyx.Models.ViewModels
{
    public class Search
    {
        public string Query { get; set; }
        public List<SearchResult> Results { get; set; }
    }

    public class SearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}