using System.ComponentModel.DataAnnotations;

namespace PaintPortal.Models;

public class PaintEquivalent
{
    public int Id { get; set; }

    public int PaintId { get; set; }
    public Paint Paint { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(7)]
    public string HexColor { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string MatchLevel { get; set; } = string.Empty;
}