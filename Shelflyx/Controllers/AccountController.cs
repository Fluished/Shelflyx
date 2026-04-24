using Shelflyx.Data;
using Shelflyx.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Shelflyx.Models;

namespace Shelflyx.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AccountController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }

            if (await _db.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already taken.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            var user = await _db.Users
                .Include(u => u.Purchases).ThenInclude(p => p.Chapter).ThenInclude(c => c!.Series)
                .Include(u => u.WalletTransactions)
                .Include(u => u.Favorites).ThenInclude(f => f.Series)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();
            return View(user);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(IFormFile profilePic)
        {
            var userId = GetUserId();
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (profilePic != null && profilePic.Length > 0)
            {
                var ext = Path.GetExtension(profilePic.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = "Only image files are allowed.";
                    return RedirectToAction("Profile");
                }

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"user_{userId}_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await profilePic.CopyToAsync(stream);

                // Delete old pic if not default
                if (!string.IsNullOrEmpty(user.ProfilePic) && !user.ProfilePic.Contains("default"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, user.ProfilePic.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                user.ProfilePic = $"/uploads/profiles/{fileName}";
                await _db.SaveChangesAsync();

                // Update claim
                await RefreshClaims(user);
            }

            return RedirectToAction("Profile");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> TopUpWallet(decimal amount)
        {
            if (amount <= 0 || amount > 10000)
            {
                TempData["Error"] = "Invalid amount. Enter between 1 and 10,000 coins.";
                return RedirectToAction("Profile");
            }

            var userId = GetUserId();
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Balance += amount;
            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = userId,
                Amount = amount,
                Type = "TopUp",
                Description = $"Wallet top-up of {amount} coins"
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Successfully added {amount} coins to your wallet!";
            return RedirectToAction("Profile");
        }

        public IActionResult AccessDenied() => View();

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new("ProfilePic", user.ProfilePic)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        private async Task RefreshClaims(User user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await SignInUser(user);
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
