using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Data;
using PaintPortal.Models;

namespace PaintPortal.Pages.Palette;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<UserPaint> Items { get; private set; } = new();

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task OnGetAsync()
    {
        Items = await _db.UserPaints
            .AsNoTracking()
            .Include(up => up.Paint)
            .Where(up => up.UserId == CurrentUserId)
            .OrderBy(up => up.Paint.Brand)
            .ThenBy(up => up.Paint.Name)
            .ToListAsync();
    }

    // delta = +1 или -1; если количество стало 0, краска удаляется
    public async Task<IActionResult> OnPostChangeAsync(int id, int delta)
    {
        var item = await _db.UserPaints
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == CurrentUserId);

        if (item != null)
        {
            item.Quantity += delta;
            if (item.Quantity <= 0)
            {
                _db.UserPaints.Remove(item);
            }
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id)
    {
        var item = await _db.UserPaints
            .Include(up => up.Paint)
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == CurrentUserId);

        if (item != null)
        {
            _db.UserPaints.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Message"] = $"\"{item.Paint.Name}\" was removed from your palette.";
        }

        return RedirectToPage();
    }
}