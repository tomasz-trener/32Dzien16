using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using P06Shop.Shared.Services;
using P11ShopWebAppMVCApi.Client.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Sekcja "AppSettings" z appsettings.json → klasa AppSettings (dostępna przez IOptions<AppSettings>)
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));

// 2. Klient naszego API: ta sama klasa ProductHttpService z P06Shop.Shared, której używa WPF (P03).
// AddHttpClient<TInterface, TImpl> = typed client: HttpClient tworzy IHttpClientFactory i wstrzykuje go do serwisu.
// (sp, client) => ... pozwala sięgnąć do kontenera po IOptions<AppSettings> w momencie tworzenia klienta.
// P11 nie ma DbContext ani connection stringa – o bazie wie tylko API.
builder.Services.AddHttpClient<IProductService, ProductHttpService>((sp, client) =>
{
    AppSettings settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    client.BaseAddress = new Uri(settings.ApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
