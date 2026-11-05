using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Data;
using PaintPortal.Models;

namespace PaintPortal.Pages.Catalog;

public class IndexModel : PageModel
{
    private const int PageSize = 24;
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Brand { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public List<Paint> Paints { get; private set; } = new();
    public List<string> Brands { get; private set; } = new();
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync()
    {
        Brands = await _db.Paints
            .Select(p => p.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();

        var query = _db.Paints.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Brand))
        {
            query = query.Where(p => p.Brand == Brand);
        }

        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(p => p.Name.Contains(Search));
        }

        TotalCount = await query.CountAsync();
        TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);

        Paints = await query
            .OrderBy(p => p.Brand)
            .ThenBy(p => p.Name)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}