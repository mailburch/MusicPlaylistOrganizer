using Microsoft.EntityFrameworkCore;
using MusicPlaylistOrganizer.Data;
using MusicPlaylistOrganizer.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext: point EF Core at your connection string in appsettings.json
builder.Services.AddDbContext<MusicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicContext")));

// Repositories: Repository pattern for Sprint 2 requirement
builder.Services.AddScoped<IArtistRepository, EfArtistRepository>();
builder.Services.AddScoped<ITrackRepository, EfTrackRepository>();
builder.Services.AddScoped<IPlaylistRepository, EfPlaylistRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve css/js/img from wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
