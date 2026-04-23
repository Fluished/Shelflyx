using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelflyx.Data;
using Shelflyx.Models;
using Shelflyx.Models.ViewModels;
using System.Security.Claims;

namespace Shelflyx.Controllers
{
    public class ReaderController : Controller
    {
        private readonly AppDbContext _db;

        public ReaderController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Read(int chapterId)
        {
            var chapter = await _db.Chapters
                .Include(c => c.Pages.OrderBy(p => p.PageNumber))
                .Include(c => c.Series)
                .FirstOrDefaultAsync(c => c.ChapterId == chapterId);

            if (chapter == null) return NotFound();

            var userId = GetUserId();
            bool hasAccess = chapter.Price == 0;

            if (!hasAccess && userId > 0)
                hasAccess = await _db.Purchases.AnyAsync(p => p.UserId == userId && p.ChapterId == chapterId);

            var allChapters = await _db.Chapters
                .Where(c => c.SeriesId == chapter.SeriesId)
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync();

            var idx = allChapters.FindIndex(c => c.ChapterId == chapterId);

            var comments = await _db.Comments
                .Include(c => c.User)
                .Where(c => c.ChapterId == chapterId)
                .OrderByDescending(c => c.DatePosted)
                .ToListAsync();

            var vm = new Reader
            {
                Chapter = chapter,
                Pages = chapter.Pages.OrderBy(p => p.PageNumber).ToList(),
                Comments = comments,
                PrevChapter = idx > 0 ? allChapters[idx - 1] : null,
                NextChapter = idx < allChapters.Count - 1 ? allChapters[idx + 1] : null,
                HasAccess = hasAccess
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Purchase(int chapterId)
        {
            var userId = GetUserId();
            var chapter = await _db.Chapters.Include(c => c.Series).FirstOrDefaultAsync(c => c.ChapterId == chapterId);
            if (chapter == null) return NotFound();

            // Already purchased?
            if (await _db.Purchases.AnyAsync(p => p.UserId == userId && p.ChapterId == chapterId))
                return RedirectToAction("Read", new { chapterId });

            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (user.Balance < chapter.Price)
            {
                TempData["Error"] = "Insufficient balance. Please top up your wallet.";
                return RedirectToAction("Read", new { chapterId });
            }

            user.Balance -= chapter.Price;

            _db.Purchases.Add(new Purchase
            {
                UserId = userId,
                ChapterId = chapterId,
                AmountPaid = chapter.Price
            });

            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = userId,
                Amount = -chapter.Price,
                Type = "Purchase",
                Description = $"Purchased: {chapter.Series?.Title} - Ch.{chapter.ChapterNumber}"
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Chapter unlocked!";
            return RedirectToAction("Read", new { chapterId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostComment(int chapterId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Read", new { chapterId });

            var userId = GetUserId();
            _db.Comments.Add(new Comment
            {
                UserId = userId,
                ChapterId = chapterId,
                Content = content.Trim()
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Read", new { chapterId });
        }

        private int GetUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return val != null ? int.Parse(val) : 0;
        }
    }
}
