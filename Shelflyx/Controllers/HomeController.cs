using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelflyx.Data;
using Shelflyx.Models.ViewModels;

namespace Shelflyx.Controllers
{
    public class HomeController : Controller
    {   
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var series = await _db.Series
                .Where(s => s.IsActive)
                .Include(s => s.Chapters)
                .Include(s => s.Ratings)
                .OrderByDescending(s => s.DateCreated)
                .Take(12)
                .ToListAsync();

            return Json(series);
        }

        public async Task<IActionResult> Search(Search model)
        {
            var query = _db.Series.Where(s => s.IsActive).Include(s => s.Chapters).Include(s => s.Ratings).AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                var q = model.Query.ToLower();
                query = query.Where(s =>
                    s.Title.ToLower().Contains(q) ||
                    s.Author.ToLower().Contains(q) ||
                    s.Description.ToLower().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(model.Genre))
                query = query.Where(s => s.Genre == model.Genre);

            if (!string.IsNullOrWhiteSpace(model.Author))
                query = query.Where(s => s.Author.ToLower().Contains(model.Author.ToLower()));

            if (model.Filter == "Free")
                query = query.Where(s => s.Chapters.All(c => c.Price == 0));
            else if (model.Filter == "Paid")
                query = query.Where(s => s.Chapters.Any(c => c.Price > 0));
            else if (model.Filter == "Popular")
                query = query.OrderByDescending(s => s.Ratings.Count);

            model.Results = await query.ToListAsync();
            return View(model);
        }

        public IActionResult Error() => View();
    }
}
