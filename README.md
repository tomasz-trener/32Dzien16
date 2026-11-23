# 32Dzien16 – aplikacje webowe: Razor Pages, MVC (Database First) i MVC przez API

Edycja 32 kursu C#/.NET, dzień 16. Kod końcowy dnia oraz instrukcja krok po kroku.

[Instrukcja.md](Instrukcja.md) – instrukcja dnia – teoria, kroki, kod, punkty kontrolne

## Punkt startowy

Cała solucja z repozytorium `32Dzien15` (API z EF Core i migracjami, biblioteka wspólna, klient WPF, mowa). Baza `MySuperShop` (LocalDB) utworzona migracjami z `P05Shop.API` i zasiana 100 produktami z `P07Shop.DataSeeder`.

## Co dochodzi dzisiaj

| Projekt | Typ | Co pokazuje | Porty (launchSettings) |
|---|---|---|---|
| `P09ShopWebApp.Client` | ASP.NET Core **Razor Pages** | `Calculator` (GET/POST, ręczne `Request.Query`/`Request.Form`), `CalculatorBinding` (`[BindProperty]`, handlery `OnPostAdd/Subtract/Multiply`, `[TempData]`), **Database First** (`Data/ShopContext`, `Models/Product` ze scaffoldu), `ProductsList` (ręczna lista), `Pages/ProductsAutoGen` (CRUD z generatora), `UseRequestLocalization` en-US | https **7201** / http 5201 |
| `P10ShopWebAppMVC.Client` | ASP.NET Core **MVC** na bazie | `CalculatorController` (`IFormCollection`, `ViewBag`, `int.TryParse`), `NewCalculatorController` + `OperationModel` + `asp-for`, scaffold bazy jak w P09, `ProductsController` + `Views/Products` z generatora | https **7202** / http 5202 |
| `P11ShopWebAppMVCApi.Client` | ASP.NET Core **MVC** przez API | **bez bazy**: referencja do `P06Shop.Shared`, `AppSettings:ApiBaseUrl` w `appsettings.json`, `AddHttpClient<IProductService, ProductHttpService>`, ręczny `ProductsController` sprawdzający `ServiceResponse.Success` i pokazujący `Message` (m.in. odmowa usunięcia produktu > 1000 zł na stronie Delete, „Brak połączenia z API” na liście) | https **7203** / http 5203 |

Projekty z poprzednich dni (`P03`, `P05`, `P06`, `P07`, `P08`) są bez zmian. API: `https://localhost:7127` / `http://localhost:5008`, trasa `api/products`.

```
                ┌──────────────────── baza MySuperShop (LocalDB) ────────────────────┐
                │                                                                     │
   P09 Razor Pages ──EF Core──┘        P10 MVC ──EF Core──┘         P05 API ──EF Core──┘
                                                                       ▲
                                                           HTTP (JSON) │
                                             P03 WPF ──────────────────┤
                                             P11 MVC ──────────────────┘
                              (P03 i P11 używają tego samego ProductHttpService z P06Shop.Shared)
```

## Narzędzia i pakiety

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet tool install --global dotnet-aspnet-codegenerator --version 10.0.2
```

W P09 i P10 (`dotnet add package ...`): `Microsoft.EntityFrameworkCore.SqlServer 10.0.12`, `Microsoft.EntityFrameworkCore.Design 10.0.12`, `Microsoft.EntityFrameworkCore.Tools 10.0.12`, `Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2`. P11 nie ma pakietów EF – ma tylko referencję do `P06Shop.Shared`.

## Polecenia użyte na zajęciach

Database First (w katalogu P09 albo P10; connection string `DefaultConnection` jest w `appsettings.Development.json`):

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
```

CRUD Razor Pages (P09):

```powershell
dotnet aspnet-codegenerator razorpage -m Product -dc ShopContext -udl -outDir Pages/ProductsAutoGen --referenceScriptLibraries
```

CRUD MVC (P10):

```powershell
dotnet aspnet-codegenerator controller -name ProductsController -m Product -dc ShopContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

## Uruchomienie

1. `P09` i `P10`: wymagają bazy `MySuperShop` w LocalDB (z dnia 14/15: `dotnet ef database update` w `P05Shop.API`). Ustaw projekt startowy i F5.
2. `P11`: **Configure Startup Projects → Multiple startup projects**: `P05Shop.API` (profil https, port 7127) + `P11ShopWebAppMVCApi.Client`. Adres API jest w `P11ShopWebAppMVCApi.Client/appsettings.json` (`AppSettings:ApiBaseUrl`, z końcowym `/`).
3. Dane testowe: produkt `Klawiatura mechaniczna` / `590123456789` / `249.99` (P09, P10); `Monitor 27 cali` / `590987654321` / `1299.00` → Edit `1199.00` → Delete odrzucone (> 1000 zł) → Edit `999.00` → Delete OK (P11).

## Co dalej

Dzień 17 (`32Dzien17`): Blazor WebAssembly (`P12BlazorWebAssembly.Client`) – ten sam `ProductHttpService` z `P06Shop.Shared`, tym razem uruchamiany w przeglądarce.
