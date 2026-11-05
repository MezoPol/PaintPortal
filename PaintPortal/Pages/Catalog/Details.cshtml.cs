using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Data;
using PaintPortal.Models;
using PaintPortal.Services;

namespace PaintPortal.Pages.Catalog;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly ColorApiService _colors;

    public DetailsModel(ApplicationDbContext db, ColorApiService colors)
    {
        _db = db;
        _colors = colors;
    }

    public Paint Paint { get; private set; } = null!;
    public ColorInfo? Info { get; private set; }
    public List<SchemeColor>? Scheme { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var paint = await _db.Paints
            .AsNoTracking()
            .Include(p => p.Equivalents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (paint == null)
        {
            return NotFound();
        }

        Paint = paint;

        // Два запроса к внешнему API выполняются одновременно
        var infoTask = _colors.GetColorInfoAsync(paint.HexColor);
        var schemeTask = _colors.GetSchemeAsync(paint.HexColor, "analogic", 5);
        await Task.WhenAll(infoTask, schemeTask);

        Info = infoTask.Result;
        Scheme = schemeTask.Result;

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Challenge();
        }

        var paint = await _db.Paints.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (paint == null)
        {
            return NotFound();
        }

        var existing = await _db.UserPaints
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PaintId == id);

        if (existing == null)
        {
            _db.UserPaints.Add(new UserPaint { UserId = userId, PaintId = id, Quantity = 1 });
        }
        else
        {
            existing.Quantity++;
        }

        await _db.SaveChangesAsync();
        TempData["Message"] = $"\"{paint.Name}\" was added to your palette.";
        return RedirectToPage("/Palette/Index");
    }
}