using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaintPortal.Data;
using PaintPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ColorApiService>(client =>
{
    client.BaseAddress = new Uri("https://www.thecolorapi.com/");
    client.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    PaintSeeder.Seed(db);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/NotFound");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
    
app.MapGet("/sitemap.xml", async (HttpContext ctx, ApplicationDbContext db) =>
{
    var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    var paints = await db.Paints
        .AsNoTracking()
        .Select(p => new { p.Id, p.Brand, p.Name })
        .ToListAsync();

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
    sb.AppendLine($"  <url><loc>{baseUrl}/</loc></url>");
    sb.AppendLine($"  <url><loc>{baseUrl}/Catalog</loc></url>");

    foreach (var p in paints)
    {
        var slug = PaintPortal.Slug.From(p.Brand + " " + p.Name);
        sb.AppendLine($"  <url><loc>{baseUrl}/Catalog/Details/{p.Id}/{slug}</loc></url>");
    }

    sb.AppendLine("</urlset>");
    return Results.Content(sb.ToString(), "application/xml");
});

app.Run();
