using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using P10ShopWebAppMVC.Client.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database First: ShopContext wygenerowany z bazy – dokładnie ta sama rejestracja co w P09 (Razor Pages).
// EF Core i DI działają tak samo niezależnie od tego, czy robimy Razor Pages, czy MVC.
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Jedna kultura dla całej aplikacji (kropka jako separator dziesiętny) – patrz P09/Program.cs
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

// Trasa domyślna: /Products/Edit/5 → ProductsController.Edit(5)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
