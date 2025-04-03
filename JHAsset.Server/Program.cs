using JHAsset.Shared.Data;
using JHAsset.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// EF Core (SQLite)
builder.Services.AddDbContextFactory<JHAssetContext>(options =>
{
    options.UseSqlite("Data Source=JHAsset.db");
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAssetService, AssetService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
