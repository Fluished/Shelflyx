using Microsoft.AspNetCore.Mvc;
using Shelflyx.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Shelflyx.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index(string query)
        {
            var model = new Search
            {
                Query = query,
                Results = GetSearchResults(query)
            };

            return View("Search", model);
        }

        private List<SearchResult> GetSearchResults(string query)
        {
            if (string.IsNullOrEmpty(query))
                return new List<SearchResult>();

            // Mock data - replace with actual database search
            var allResults = new List<SearchResult>
            {
                new SearchResult { Id = 1, Title = "Series One", Description = "First series" },
                new SearchResult { Id = 2, Title = "Series Two", Description = "Second series" },
                new SearchResult { Id = 3, Title = "Series Three", Description = "Third series" }
            };

            return allResults
                .Where(r => r.Title.ToLower().Contains(query.ToLower()) ||
                           r.Description.ToLower().Contains(query.ToLower()))
                .ToList();
        }
    }
}