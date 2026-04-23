using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelflyx.Data;
using Shelflyx.Models;
using Shelflyx.Models.ViewModels;
using System.Security.Claims;

namespace Shelflyx.Controllers
{
    public class SeriesController : Controller
    {
        private readonly AppDbContext _db;

        public SeriesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Details(int id)
        {
            var series = await _db.Series
                .Include(s => s.Chapters.OrderBy(c => c.ChapterNumber))
                .Include(s => s.Ratings)
                .FirstOrDefaultAsync(s => s.SeriesId == id && s.IsActive);

            if (series == null) return NotFound();

            var userId = GetUserId();
            var vm = new SeriesDetail
            {
                Series = series,
                Chapters = series.Chapters.OrderBy(c => c.ChapterNumber).ToList(),
                AverageRating = series.Ratings.Any() ? series.Ratings.Average(r => r.Stars) : 0,
                TotalRatings = series.Ratings.Count,
                IsFavorited = userId > 0 && await _db.Favorites.AnyAsync(f => f.UserId == userId && f.SeriesId == id),
                UserRating = userId > 0 ? (await _db.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.SeriesId == id))?.Stars : null
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int seriesId)
        {
            var userId = GetUserId();
            var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.SeriesId == seriesId);

            if (existing != null)
                _db.Favorites.Remove(existing);
            else
                _db.Favorites.Add(new Favorite { UserId = userId, SeriesId = seriesId });

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = seriesId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rate(int seriesId, int stars)
        {
            if (stars < 1 || stars > 5) return BadRequest();
            var userId = GetUserId();

            var existing = await _db.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.SeriesId == seriesId);
            if (existing != null)
                existing.Stars = stars;
            else
                _db.Ratings.Add(new Rating { UserId = userId, SeriesId = seriesId, Stars = stars });

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = seriesId });
        }

        private int GetUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return val != null ? int.Parse(val) : 0;
        }
    }
}
