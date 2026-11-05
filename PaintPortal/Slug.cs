using System.Text.RegularExpressions;

namespace PaintPortal;

public static class Slug
{
    // "Mephiston Red" -> "mephiston-red"
    public static string From(string text)
    {
        var lower = text.ToLowerInvariant();
        var clean = Regex.Replace(lower, "[^a-z0-9]+", "-");
        return clean.Trim('-');
    }
}