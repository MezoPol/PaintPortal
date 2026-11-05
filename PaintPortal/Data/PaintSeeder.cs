using System.Text.Json;
using PaintPortal.Models;

namespace PaintPortal.Data;

public static class PaintSeeder
{
    private static readonly string[] Files =
    {
        "citadel.json",
        "vallejo.json",
        "army_painter.json",
        "ak_interactive.json"
    };

    public static void Seed(ApplicationDbContext db)
    {
        // Если краски уже загружены, ничего не делаем
        if (db.Paints.Any())
        {
            return;
        }

        var folder = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var file in Files)
        {
            var json = File.ReadAllText(Path.Combine(folder, file));
            var items = JsonSerializer.Deserialize<List<PaintJson>>(json, options)
                        ?? new List<PaintJson>();

            // Дубли убираем: для каждой пары "бренд + название" берём первую запись
            var unique = items
                .GroupBy(p => (p.Brand, p.Name))
                .Select(g => g.First());

            foreach (var item in unique)
            {
                var paint = new Paint
                {
                    Brand = item.Brand,
                    Name = item.Name,
                    HexColor = item.HexColor,
                    Type = item.Type,
                    Range = item.Range
                };

                foreach (var eq in item.Equivalents)
                {
                    paint.Equivalents.Add(new PaintEquivalent
                    {
                        Brand = eq.Brand,
                        Name = eq.Name,
                        HexColor = eq.HexColor,
                        MatchLevel = eq.MatchLevel
                    });
                }

                db.Paints.Add(paint);
            }
        }

        db.SaveChanges();
    }

    // Классы только для чтения JSON, в базе их нет
    private class PaintJson
    {
        public string Brand { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string HexColor { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public List<EquivalentJson> Equivalents { get; set; } = new();
    }

    private class EquivalentJson
    {
        public string Brand { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string HexColor { get; set; } = string.Empty;
        public string MatchLevel { get; set; } = string.Empty;
    }
}