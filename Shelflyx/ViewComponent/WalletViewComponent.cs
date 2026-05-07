using Microsoft.AspNetCore.Mvc;
using Shelflyx.Data;
using System.Security.Claims;

public class WalletViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public WalletViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        decimal balance = 0;

        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userId, out int id))
        {
            var user = await _context.Users.FindAsync(id);
            balance = user?.Balance ?? 0;
        }

        return View(balance);
    }
}