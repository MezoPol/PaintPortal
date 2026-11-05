using System.ComponentModel.DataAnnotations;

namespace PaintPortal.Models;

public class Paint
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(7)]
    public string HexColor { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Range { get; set; } = string.Empty;

    public List<PaintEquivalent> Equivalents { get; set; } = new();
}