using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace PaintPortal.Services;

public record ColorInfo(string Name, string Rgb, string Hsl, bool ExactMatch);
public record SchemeColor(string Hex, string Name);

public class ColorApiService
{
    private static readonly Regex HexPattern = new("^[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ColorApiService> _logger;

    public ColorApiService(HttpClient http, IMemoryCache cache, ILogger<ColorApiService> logger)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
    }

    // "#B22222" -> "B22222"; если формат неверный, вернёт null
    private static string? Clean(string hex)
    {
        var clean = hex.TrimStart('#');
        return HexPattern.IsMatch(clean) ? clean.ToUpperInvariant() : null;
    }

    public async Task<ColorInfo?> GetColorInfoAsync(string hex)
    {
        var clean = Clean(hex);
        if (clean == null) return null;
        return await CachedAsync($"id:{clean}", () => FetchInfoAsync(clean));
    }

    public async Task<List<SchemeColor>?> GetSchemeAsync(string hex, string mode = "analogic", int count = 5)
    {
        var clean = Clean(hex);
        if (clean == null) return null;
        return await CachedAsync($"scheme:{clean}:{mode}:{count}", () => FetchSchemeAsync(clean, mode, count));
    }

    // Кэш: повторный запрос той же краски не идёт во внешний API
    private async Task<T?> CachedAsync<T>(string key, Func<Task<T?>> fetch) where T : class
    {
        if (_cache.TryGetValue(key, out T? cached))
        {
            return cached;
        }

        var result = await fetch();
        if (result != null)
        {
            _cache.Set(key, result, TimeSpan.FromDays(1));
        }
        return result;
    }

    private async Task<ColorInfo?> FetchInfoAsync(string clean)
    {
        try
        {
            using var response = await _http.GetAsync($"id?hex={clean}");
            if (!response.IsSuccessStatusCode) return null;

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = doc.RootElement;
            var name = root.GetProperty("name");

            return new ColorInfo(
                name.GetProperty("value").GetString() ?? "",
                root.GetProperty("rgb").GetProperty("value").GetString() ?? "",
                root.GetProperty("hsl").GetProperty("value").GetString() ?? "",
                name.GetProperty("exact_match_name").GetBoolean());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException)
        {
            _logger.LogWarning(ex, "TheColorAPI /id request failed for {Hex}", clean);
            return null;
        }
    }

    private async Task<List<SchemeColor>?> FetchSchemeAsync(string clean, string mode, int count)
    {
        try
        {
            using var response = await _http.GetAsync($"scheme?hex={clean}&mode={mode}&count={count}");
            if (!response.IsSuccessStatusCode) return null;

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var result = new List<SchemeColor>();

            foreach (var color in doc.RootElement.GetProperty("colors").EnumerateArray())
            {
                result.Add(new SchemeColor(
                    color.GetProperty("hex").GetProperty("value").GetString() ?? "",
                    color.GetProperty("name").GetProperty("value").GetString() ?? ""));
            }
            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException)
        {
            _logger.LogWarning(ex, "TheColorAPI /scheme request failed for {Hex}", clean);
            return null;
        }
    }
}