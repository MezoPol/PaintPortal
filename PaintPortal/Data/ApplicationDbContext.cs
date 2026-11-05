using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Models;

namespace PaintPortal.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paint> Paints => Set<Paint>();
    public DbSet<PaintEquivalent> PaintEquivalents => Set<PaintEquivalent>();
    public DbSet<UserPaint> UserPaints => Set<UserPaint>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Paint>()
            .HasIndex(p => new { p.Brand, p.Name })
            .IsUnique();

        builder.Entity<UserPaint>()
            .HasIndex(up => new { up.UserId, up.PaintId })
            .IsUnique();
    }
}