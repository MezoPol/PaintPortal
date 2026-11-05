using Microsoft.AspNetCore.Identity;

namespace PaintPortal.Models;

public class UserPaint
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public IdentityUser User { get; set; } = null!;

    public int PaintId { get; set; }
    public Paint Paint { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}