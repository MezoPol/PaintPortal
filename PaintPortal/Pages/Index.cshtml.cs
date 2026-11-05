using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Data;

namespace PaintPortal.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int PaintCount { get; private set; }
    public List<string> Brands { get; private set; } = new();

    public async Task OnGetAsync()
    {
        PaintCount = await _db.Paints.CountAsync();
        Brands = await _db.Paints
            .Select(p => p.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();
    }
}