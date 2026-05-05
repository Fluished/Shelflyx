using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelflyx.Data;
using Shelflyx.Models;

namespace Shelflyx.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalSeries = await _db.Series.CountAsync();
            ViewBag.TotalChapters = await _db.Chapters.CountAsync();
            ViewBag.TotalPurchases = await _db.Purchases.CountAsync();
            ViewBag.TotalRevenue = (decimal)(
                await _db.Purchases.SumAsync(p => (double?)p.AmountPaid) ?? 0
            );
            return View();
        }

        // ─── SERIES ───────────────────────────────────────────────────────────────

        public async Task<IActionResult> SeriesList() =>
            View(await _db.Series.Include(s => s.Chapters).ToListAsync());

        [HttpGet]
        public IActionResult CreateSeries() => View();

        [HttpPost]
        public async Task<IActionResult> CreateSeries(Series model, IFormFile? coverImage)
        {
            if (!ModelState.IsValid) return View(model);

            if (coverImage != null && coverImage.Length > 0)
                model.CoverImage = await SaveImage(coverImage, "covers");

            _db.Series.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Series created!";
            return RedirectToAction("SeriesList");
        }

        [HttpGet]
        public async Task<IActionResult> EditSeries(int id)
        {
            var series = await _db.Series.FindAsync(id);
            if (series == null) return NotFound();
            return View(series);
        }

        [HttpPost]
        public async Task<IActionResult> EditSeries(Series model, IFormFile? coverImage)
        {
            if (!ModelState.IsValid) return View(model);

            var series = await _db.Series.FindAsync(model.SeriesId);
            if (series == null) return NotFound();

            series.Title = model.Title;
            series.Genre = model.Genre;
            series.Author = model.Author;
            series.Description = model.Description;
            series.IsActive = model.IsActive;

            if (coverImage != null && coverImage.Length > 0)
                series.CoverImage = await SaveImage(coverImage, "covers");

            await _db.SaveChangesAsync();
            TempData["Success"] = "Series updated!";
            return RedirectToAction("SeriesList");
        }

        // ─── CHAPTERS ─────────────────────────────────────────────────────────────

        public async Task<IActionResult> ChapterList(int seriesId)
        {
            var series = await _db.Series.Include(s => s.Chapters.OrderBy(c => c.ChapterNumber))
                .FirstOrDefaultAsync(s => s.SeriesId == seriesId);
            if (series == null) return NotFound();
            return View(series);
        }

        [HttpGet]
        public async Task<IActionResult> CreateChapter(int seriesId)
        {
            ViewBag.Series = await _db.Series.FindAsync(seriesId);
            return View(new Chapter { SeriesId = seriesId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter(Chapter model, List<IFormFile> pages)
        {
            _db.Chapters.Add(model);
            await _db.SaveChangesAsync();

            // Upload pages
            int pageNum = 1;
            foreach (var page in pages.Where(p => p.Length > 0))
            {
                var path = await SaveImage(page, $"chapters/{model.ChapterId}");
                _db.Pages.Add(new Page
                {
                    ChapterId = model.ChapterId,
                    PageNumber = pageNum++,
                    ImagePath = path
                });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Chapter {model.ChapterNumber} created with {pageNum - 1} pages!";
            return RedirectToAction("ChapterList", new { seriesId = model.SeriesId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChapter(int chapterId)
        {
            var chapter = await _db.Chapters.FindAsync(chapterId);
            if (chapter == null) return NotFound();
            var seriesId = chapter.SeriesId;
            _db.Chapters.Remove(chapter);
            await _db.SaveChangesAsync();
            return RedirectToAction("ChapterList", new { seriesId });
        }

        // ─── USERS ────────────────────────────────────────────────────────────────

        public async Task<IActionResult> Users() =>
            View(await _db.Users.OrderBy(u => u.Username).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();
            user.Role = user.Role == "Admin" ? "User" : "Admin";
            await _db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        // ─── HELPERS ──────────────────────────────────────────────────────────────

        private async Task<string> SaveImage(IFormFile file, string folder)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            var dir = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(dir);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(dir, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{folder}/{fileName}";
        }
    }
}
