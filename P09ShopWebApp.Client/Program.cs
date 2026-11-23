using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Database First: ShopContext wygenerowany z bazy (dotnet ef dbcontext scaffold).
// Provider i connection string podajemy tutaj, nie w OnConfiguring – tak samo jak DataContext w P05.
// AddDbContext = Scoped: jeden ShopContext na jedno żądanie HTTP.
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Jedna kultura dla całej aplikacji (kropka jako separator dziesiętny).
// Bez tego na polskim Windowsie cena "249.99" nie przechodzi walidacji na serwerze (pl-PL chce przecinka),
// a "249,99" nie przechodzi walidacji w przeglądarce (jQuery Validation chce kropki).
var appCulture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(appCulture),
    SupportedCultures = new List<CultureInfo> { appCulture },
    SupportedUICultures = new List<CultureInfo> { appCulture }
});

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
