# Dzień 16: aplikacje webowe – Razor Pages, MVC (Database First) i MVC przez API

> **Technologia:** .NET 10 (LTS), C# 14, **ASP.NET Core Razor Pages** i **ASP.NET Core MVC**, Entity Framework Core 10 (**Database First**, `dotnet ef dbcontext scaffold`), generator kodu `dotnet-aspnet-codegenerator` 10.0.2, SQL Server LocalDB, Visual Studio 2026, solucja `32Dzien16.slnx`.
>
> **Punkt startowy:** cała solucja z dnia 15 (`32Dzien15`). Baza `MySuperShop` w LocalDB jest utworzona migracjami z `P05Shop.API` i zasiana 100 produktami.
>
> **Cel dnia:** trzy aplikacje **webowe** (otwierane w przeglądarce), od najprostszej do najbardziej „architektonicznej”:
> 1. **P09ShopWebApp.Client** (Razor Pages, prosto na bazie przez EF Core) – dwa kalkulatory (GET/POST, `Request.Form`, model binding, `TempData`), klasy wygenerowane z istniejącej bazy (**Database First**), ręczna lista produktów i pełny **CRUD** z generatora.
> 2. **P10ShopWebAppMVC.Client** (MVC, prosto na bazie) – kontroler, akcja i widok na przykładzie kalkulatorów, scaffold bazy jak w P09, kontroler CRUD z generatora.
> 3. **P11ShopWebAppMVCApi.Client** (MVC przez API) – aplikacja, która **nie zna bazy** i rozmawia z naszym API tym samym `ProductHttpService`, co WPF: `AppSettings` z adresem API, `AddHttpClient<IProductService, ProductHttpService>`, ręczny kontroler i widoki z komunikatami z API (reguła 1000 zł, brak połączenia, walidacja z `P06Shop.Shared`).
>
> Po drodze: formularze, GET/POST, bezstanowość HTTP, model binding, Tag Helpery (Tag Helpers), Database First i generowanie ekranów CRUD (scaffolding).
> Jutro (dzień 17) ten sam `ProductHttpService` uruchomimy **w przeglądarce** (Blazor WebAssembly); API dostanie wyszukiwanie i CORS.

**Punkt startowy (stan solucji po dniu 15) i co się z nim dziś dzieje:**

| Projekt / plik | Co zawiera | Co się z nim dziś dzieje |
|---|---|---|
| `P05Shop.API` | kontroler `api/products` (GET/POST/PUT/DELETE), `ProductService` na `DataContext`, reguła `MaxDeletablePrice = 1000`, migracje | **bez zmian**; uruchamiane w sekcji 9 (P11 rozmawia z nim przez HTTP) |
| `P06Shop.Shared/Models/Product.cs` | `Id`, `Title`, `Description`, `Barcode`, `Price` (decimal), `ReleaseDate` + atrybuty `[Required]`, `[MaxLength]`, `[Range]` z polskimi komunikatami | **bez zmian**; model widoków w P11 (walidacja MVC czyta te same atrybuty) |
| `P06Shop.Shared/Services/ProductHttpService.cs` | klient HTTP z `SendAsync<T>` (brak połączenia, 404/400, ProblemDetails) | **bez zmian**; rejestrowany w P11 przez `AddHttpClient` |
| `P06Shop.Shared/Services/IProductService.cs`, `ServiceResponse.cs` | kontrakt CRUD, koperta `Ok`/`Fail` | **bez zmian** |
| `P03WeatherForecastWPF.Client` | okno **Sklep** z CRUD, `Configuration/AppSettings.cs` (`SectionName`, `ApiBaseUrl`) | **bez zmian**; wzór dla `AppSettings` w P11; opcjonalnie drugi klient na tym samym API |
| `P07Shop.DataSeeder`, `P08Speech` | dane, mowa | **bez zmian** |
| **nowy:** `P09ShopWebApp.Client` | – | Razor Pages: kalkulatory, scaffold bazy, lista, CRUD z generatora (sekcje 1–6) |
| **nowy:** `P10ShopWebAppMVC.Client` | – | MVC: kalkulatory, scaffold bazy, kontroler CRUD z generatora (sekcje 7–8) |
| **nowy:** `P11ShopWebAppMVCApi.Client` | – | MVC przez API: `AppSettings`, `AddHttpClient`, ręczny kontroler, widoki skopiowane z P10 (sekcja 9) |
| `32Dzien16.slnx` | 5 projektów | 8 projektów (sekcja 10) |

## Jak czytać tę instrukcję

| Znacznik | Znaczenie |
|---|---|
| 💡 **Wyjaśnienie** | co i dlaczego się dzieje; szersze omówienie |
| ⚠️ **Uwaga** | pułapka, ograniczenie albo rzecz łatwa do przeoczenia |
| ✅ **Punkt kontrolny** | co ma działać, zanim przejdziemy dalej |
| 🔧 **Zmiana względem poprzednich edycji** | tu robimy **inaczej niż w edycjach 26/31**; tamto działało, to jest poprawka lub nowsza praktyka |
| 🟩 **Co dopisujemy** | podgląd zmian w istniejącym pliku: linie `+` dopisujemy, `-` usuwamy, `⋯` to pominięty kod; pod spodem zawsze jest pełny plik |
| 🧪 **Dane testowe** / **Eksperyment** | konkretne wartości do wpisania i oczekiwany wynik; eksperyment to krok opcjonalny, po którym przywracamy kod |

Zasady dotyczące kodu:

- Kod w blokach jest **gotowy do skopiowania w całości**. W pierwszej linii bloku (komentarz `// …`, `@* … *@` albo `<!-- … -->`) jest ścieżka pliku. Zastępujemy całą zawartość tego pliku. Komentarz ze ścieżką może zostać w pliku.
- Znaków `+`/`-` z bloków 🟩 nie kopiujemy. Pod blokiem 🟩 zawsze jest pełny plik.
- Pliki z dopiskiem **wersja tymczasowa** nadpiszemy w późniejszej sekcji.
- Fragmenty **Dlaczego tak** wyjaśniają decyzje; na nich opierają się dni 17–18.
- Gotowy kod jest w repozytorium `tomasz-trainer/32Dzien16`, ale najwięcej uczymy się, pisząc go samodzielnie. Numeracja sekcji (0–10) odpowiada blokom planu dnia.

Dziś występują trzy rodzaje plików:

| Rodzaj | Przykład | Co z nim robimy |
|---|---|---|
| **z szablonu** `dotnet new` (Visual Studio: **Add → New Project**) | `Pages/Index.cshtml`, `Error.cshtml`, `Privacy.cshtml`, `wwwroot/lib/*`, `_ValidationScriptsPartial.cshtml`, `site.css`, `HomeController.cs`, `ErrorViewModel.cs` | tylko wymieniamy i opisujemy; nie przepisujemy i nie zmieniamy |
| **wygenerowane narzędziem** (`dotnet ef dbcontext scaffold`, `dotnet aspnet-codegenerator`) | `Models/Product.cs`, `Data/ShopContext.cs`, `Pages/ProductsAutoGen/*`, `Controllers/ProductsController.cs` (P10) + `Views/Products/*` | **uruchamiamy polecenie**, a zacytowany plik (oznaczony „wygenerowane – nie przepisujemy, porównujemy”) służy do porównania; jeśli wynik narzędzia różni się drobiazgiem (kolejność `using`, komentarz nagłówkowy), obowiązuje wynik narzędzia |
| **ręczne** | kalkulatory, `ProductsList`, `AppSettings`, `ProductsController` (P11), zmiany w `Program.cs`, `_Layout.cshtml`, `appsettings*.json`, `.csproj` | wklejamy w całości; zmiany w plikach z szablonu poprzedza blok 🟩 |

## Plan dnia (orientacyjnie)

| Blok | Temat | Czas |
|---|---|---|
| 0 | Przygotowanie (LocalDB, narzędzia, API) + wstęp: aplikacje webowe server‑side vs SPA vs desktop; Razor Pages vs MVC vs Blazor; HTTP, GET/POST, formularze, model binding, PRG, antiforgery; Database First vs Code First; scaffolding; architektura dnia | 25 min |
| 1 | P09: nowy projekt Razor Pages, porty, struktura projektu, `Program.cs`, routing po plikach | 20 min |
| 2 | P09: `Calculator` – formularz GET, potem POST, `Request.Query`/`Request.Form`, bezstanowość HTTP, token antiforgery, link w menu | 40 min |
| 3 | P09: `CalculatorBinding` – `[BindProperty]`, handlery `OnPostAdd/Subtract/Multiply`, `[TempData]` | 30 min |
| 4 | P09: Database First – pakiety, connection string, `dotnet ef dbcontext scaffold --no-onconfiguring`, `AddDbContext` | 45 min |
| 5 | P09: ręczna lista produktów (`ShopContext` przez DI, `@foreach`, SQL w konsoli) | 20 min |
| 6 | P09: CRUD z generatora (`razorpage`), przegląd kodu, PRG, test, problem z przecinkiem i `UseRequestLocalization` | 40 min |
| 7 | P10: MVC – kontroler, akcja, widok; `Calculator` (`IFormCollection`, `ViewBag`) i `NewCalculator` (model + `asp-for`) | 45 min |
| 8 | P10: scaffold bazy jak w P09 + kontroler CRUD z CLI (`controller`), `[Bind]`, `[ValidateAntiForgeryToken]`, `ActionName` | 35 min |
| 9 | P11: MVC przez API – referencja do Shared, `AppSettings`, `AddHttpClient`, ręczny kontroler na `IProductService`, widoki, komunikaty z API, reguła 1000 zł, API wyłączone, walidacja z atrybutów | 75 min |
| 10 | Podsumowanie, pytania, commit, zapowiedź dnia 17 | 15 min |

Przerwy po blokach 3, 6 i 8.

## 🧪 Dane testowe (zawsze te same)

Do tej tabeli wracamy w każdym punkcie kontrolnym ✅.

| Gdzie | Pole / żądanie | Wartość | Oczekiwany wynik |
|---|---|---|---|
| P09 `/Calculator` (GET) | adres | `/Calculator?Number1=2&Number2=3` | `Result: 5` |
| P09 `/Calculator` (POST) | Number 1 / Number 2 | `2` / `3` | `Result: 5`, pola nadal wypełnione |
| P09 `/Calculator` | Number 1 / Number 2 | `abc` / `3` | `Result: 0` + czerwone `Podaj dwie liczby całkowite.` |
| P09 `/CalculatorBinding` | Number 1 / Number 2 | `7` / `3` | **Calculate sum** → `10 / 0 / 0`; **subtraction** → `10 / 4 / 0`; **multiplication** → `0 / 4 / 21` |
| P09 `/ProductsList` | – | – | 100 wierszy, `Produktów: 100`, w konsoli `SELECT … FROM [Products]` |
| P09 `/ProductsAutoGen` → **Create New** | Title / Description | `Klawiatura mechaniczna` / `Klawiatura z podświetleniem RGB` | po zapisie lista ma 101 wierszy |
| | Barcode / Price / ReleaseDate | `590123456789` / `249.99` (z **kropką**) / `2024-05-01 10:00` | |
| P09 `/ProductsAutoGen/Create` | Price | `249,99` (z przecinkiem) | przeglądarka: `The field Price must be a number.` (formularz nie wychodzi) |
| P09 **Edit** → **Details** → **Delete** | Price | `199.99` | Details pokazuje `199.99`; po Delete lista ma 100 wierszy |
| P10 `/Calculator/Add` | number1 / number2 | `2` / `3`; potem puste pole | `Result: 5`; `Podaj dwie liczby całkowite.` (bez strony błędu) |
| P10 `/NewCalculator` | – | wartości startowe `8` / `10` → `7` / `3` | `Result: 18`, potem `Result: 10` |
| P10 `/Products` | jak P09 | `Klawiatura mechaniczna`, `249.99` | Create → Edit `199.99` → Details → Delete; `/Products/Details/101` po usunięciu → `404` |
| P11 `/Products` → **Create New** | Title / Description | `Monitor 27 cali` / `Monitor IPS 2560x1440` | `Produktów: 101`, w konsoli API `INSERT` |
| | Barcode / Price | `590987654321` / `1299.00` | |
| P11 **Edit** → **Delete** | Price | `1199.00` | na stronie Delete czerwone: `Nie można usunąć produktu droższego niż 1000 zł (cena: 1199,00 zł).` |
| P11 **Edit** → **Delete** | Price | `999.00` | produkt znika, `Produktów: 100` |
| P11 **Create New** | Title pusty, Barcode 13 znaków | `5909876543210` | `Nazwa jest wymagana.`, `Kod kreskowy może mieć maksymalnie 12 znaków.` (z atrybutów w `P06Shop.Shared`) |
| P11 z wyłączonym API | `/Products` | – | `Produktów: 0` + czerwony alert `Brak połączenia z API: …`, bez strony z wyjątkiem |

⚠️ Format ceny w komunikacie z API zależy od kultury procesu API: na polskim Windows `1199,00 zł`, w środowisku z kulturą invariant `1199.00 zł`. Daty na stronach P09–P11 są w formacie amerykańskim (`5/1/2024 10:00:00 AM`), bo aplikacje działają w kulturze `en-US` (sekcja 6.4).

---

## 0. Wymagania, przygotowanie i teoria wstępna

### 0.1 Wymagania

- Visual Studio 2026 z workloadem **ASP.NET and web development** (zawiera LocalDB) i **.NET desktop development** (dla P03).
- .NET 10 SDK: `dotnet --version` → `10.0.x`.
- **LocalDB z bazą `MySuperShop`** z dnia 14/15.
- Narzędzia `dotnet-ef` i `dotnet-aspnet-codegenerator` (krok 3 w 0.2).
- Wolne porty **7127/5008** (API), **7201/5201** (P09), **7202/5202** (P10), **7203/5203** (P11).
- **Kod z dnia 15**: cała solucja `32Dzien15` (bez `bin` i `obj`). Jeśli jej brak, pobieramy ją z repozytorium `tomasz-trainer/32Dzien15`.

### 0.2 Przygotowanie (10 min)

1. **Kopia solucji.** Kopiujemy folder `32Dzien15` jako `32Dzien16` (bez `bin`/`obj`, bez `.git`). Zmieniamy nazwę `32Dzien15.slnx` na `32Dzien16.slnx`. W terminalu (PowerShell) w folderze `32Dzien16`:

```powershell
dotnet build 32Dzien16.slnx
dotnet run --project P05Shop.API --launch-profile https
```

   Otwieramy `https://localhost:7127/api/products` → `"message": "Pobrano 100 produktów."`, pierwszy produkt `Fantastic Fresh Gloves`. Zatrzymujemy API (`Ctrl+C`). Repozytorium `tomasz-trainer/32Dzien16` ma gotowy kod, ale trzy nowe projekty tworzymy od zera.

2. **LocalDB i baza.** `sqllocaldb start mssqllocaldb`, potem w Visual Studio **View → SQL Server Object Explorer → (localdb)\MSSQLLocalDB → Databases → MySuperShop → Tables → dbo.Products → View Data**: 100 wierszy. Jeśli bazy nie ma (nowy komputer): `cd P05Shop.API` → `dotnet ef database update` (migracje z dnia 14 tworzą bazę i 100 produktów).

3. **Narzędzia** (raz na komputer, w terminalu; `dotnet-ef` jest już z dnia 14):

```powershell
dotnet --version
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet tool install --global dotnet-aspnet-codegenerator --version 10.0.2
dotnet ef --version
dotnet aspnet-codegenerator --help
```

   Jeśli narzędzie jest już zainstalowane w innej wersji: `dotnet tool update --global dotnet-ef --version 10.0.12` (tak samo dla generatora: `dotnet tool update --global <nazwa> --version <wersja>`). Wersje narzędzi mają zgadzać się z pakietami w projektach: `Microsoft.EntityFrameworkCore.Design 10.0.12` i `Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2`.

4. ⚠️ **Connection string API po dniu 15.** W dniu 15 connection string do Azure SQL trafił do **User Secrets** P05 i **nadpisuje** `DefaultConnection` z `appsettings.Development.json`. Dziś P09 i P10 wejdą do bazy **bezpośrednio** (LocalDB), a P11 przez API, więc API też ma pracować na LocalDB. Inaczej P09 pokaże inne dane niż P11. Sprawdzamy: prawy przycisk na `P05Shop.API` → **Manage User Secrets**. Jeśli jest tam `ConnectionStrings:DefaultConnection` z Azure, na dziś komentujemy ten wpis (albo zmieniamy nazwę klucza). Tak samo `ApiBaseUrl` w `P03…/appsettings.json` ma być `https://localhost:7127/`.

5. **Pakiety NuGet** (przy słabym internecie warto pobrać je wcześniej): `Microsoft.EntityFrameworkCore.SqlServer 10.0.12`, `Microsoft.EntityFrameworkCore.Design 10.0.12`, `Microsoft.EntityFrameworkCore.Tools 10.0.12`, `Microsoft.VisualStudio.Web.CodeGeneration.Design 10.0.2`. Generator ciągnie sporo zależności (kilkadziesiąt MB). Pomaga `dotnet restore` na komputerze z dobrym łączem i kopia folderu `%USERPROFILE%\.nuget\packages`.

6. **Porty.** W `launchSettings.json` nowych projektów szablon losuje porty; zmienimy je ręcznie (sekcja 1), żeby wszędzie były te same adresy. Sprawdzenie, czy port jest wolny: `netstat -ano | findstr :7201`.

7. Przydają się dwa okna terminala (API; polecenia `dotnet ef` i generatora) i przeglądarka z **F12 → Network** (różnica GET/POST w sekcji 2).

8. Otwieramy `32Dzien16.slnx` w Visual Studio: 5 projektów. Na koniec dnia będzie 8.

✅ **Punkt kontrolny:** `dotnet build 32Dzien16.slnx` bez błędów, API zwraca 100 produktów, `dotnet ef --version` i `dotnet aspnet-codegenerator --help` działają.

### 0.3 Teoria wstępna (20 min)

Do tej pory nasz sklep miał dwa końce: API z bazą (P05) i klienta desktopowego w WPF (P03). Użytkownik musiał **zainstalować** program. Dziś robimy klienta, którego otwiera się w przeglądarce: wpisujemy adres i mamy aplikację. Robimy to na trzy sposoby, bo każdy pokazuje coś innego:

- **Razor Pages** – najprostszy model: strona sama obsługuje swoje żądania;
- **MVC** (Model‑View‑Controller) – kontroler, widok, model: podział na warstwy;
- **MVC przez API** – aplikacja webowa nie zna bazy, tylko pyta API, dokładnie jak WPF.

Po drodze poznajemy, jak przeglądarka rozmawia z serwerem, jak wygenerować klasy z **istniejącej** bazy i jak wygenerować gotowe ekrany CRUD w minutę.

#### Gdzie wykonuje się kod?

| | Desktop (WPF, dni 11–15; MAUI, dzień 18) | **Web server‑side** (Razor Pages, MVC – dziś) | SPA w przeglądarce (Blazor WebAssembly – jutro; React/Angular) |
|---|---|---|---|
| Gdzie działa logika | na komputerze użytkownika | **na serwerze**; przeglądarka dostaje gotowy HTML | w przeglądarce (JavaScript albo .NET w WebAssembly) |
| Instalacja | tak (exe/msix) | nie: adres w przeglądarce | nie |
| Dostęp do bazy | przez API | **może bezpośrednio** (kod jest na serwerze) albo przez API | tylko przez API (kod jest u klienta) |
| Interakcja | natychmiastowa | każda akcja = żądanie HTTP i nowa strona (lub fragment) | płynna, jak desktop |
| Kiedy | narzędzia lokalne, sprzęt, offline | formularze, panele administracyjne, strony z treścią, SEO | aplikacje „jak programy”, dużo interakcji |

Aplikacja server‑side to prosty model: przeglądarka wysyła **żądanie** (request), serwer uruchamia kod C#, buduje HTML i odsyła **odpowiedź** (response). Przeglądarka tylko wyświetla. Dlatego nasz kod webowy może sięgnąć do bazy przez EF Core (P09, P10). Zobaczymy jednak, dlaczego zwykle lepiej iść przez API (P11).

#### Razor Pages, MVC czy Blazor?

| | **Razor Pages** (P09) | **MVC** (P10, P11) | **Blazor** (dzień 17) |
|---|---|---|---|
| Jednostka | strona = plik `.cshtml` + klasa `PageModel` (`.cshtml.cs`) | kontroler (klasa z akcjami) + widoki `.cshtml` + model | komponent `.razor` (HTML + C# w jednym) |
| Adres → kod | ze ścieżki pliku: `Pages/Calculator.cshtml` → `/Calculator` | z trasy: `/Products/Edit/5` → `ProductsController.Edit(5)` | z `@page "/products"` w komponencie |
| Obsługa żądania | `OnGet()`, `OnPost()`, `OnPostAdd()` | `[HttpGet] Index()`, `[HttpPost] Create(...)` | zdarzenia C# (`@onclick`), bez przeładowania strony |
| Zalety | mało plików, wszystko dla strony w jednym miejscu, szybki start | jedna klasa na „temat” (wszystkie operacje na produkcie), łatwo testować, dojrzały wzorzec | C# zamiast JavaScriptu, interaktywność |
| Wady | przy wielu stronach logika rozproszona po `PageModel`ach | więcej plików i folderów (`Controllers`, `Views/Products/…`) | większe pobranie (WASM), inny model myślenia |
| Kiedy | małe/średnie aplikacje, formularze, CRUD | większe aplikacje, zespoły, wiele operacji na jednym zasobie | aplikacje interaktywne, zespół zna C#, a nie JS |
| Alternatywy spoza .NET | – | Django, Rails, Spring MVC, Laravel | React, Angular, Vue |

Wszystkie trzy używają tej samej składni **Razor** (`@Model.Title`, `@foreach`, `@if`) i tego samego `Program.cs` z DI, który znamy z API.

#### Jak przeglądarka rozmawia z serwerem

```text
   przeglądarka                                        serwer (Kestrel + nasz kod)
   ─────────────                                       ───────────────────────────
   GET /Calculator?Number1=2&Number2=3  ─────────────► OnGet()  → HTML
   ◄──────────────────────────── 200 OK + HTML ────────
   (użytkownik wypełnia formularz, klika Submit)
   POST /Calculator  (treść: Number1=2&Number2=3
                      + __RequestVerificationToken) ──► OnPost() → HTML   (albo 302 → GET /Index: wzorzec PRG)
   ◄──────────────────────────── 200 OK + HTML ────────
   ...serwer NIC nie pamięta między żądaniami (HTTP jest bezstanowe)...
```

| Pojęcie | Co to | Gdzie dziś |
|---|---|---|
| **GET** | „daj mi stronę”; dane w adresie (query string `?a=1&b=2`); można skopiować, dodać do zakładek; nie zmienia danych; do pobierania i wyszukiwania | lista, formularz pusty, `Details` |
| **POST** | „przyjmij dane”; dane w **treści** żądania; zmienia stan (zapis, usunięcie) | wysłanie formularza, `Create`, `Edit`, `Delete` |
| **bezstanowość** | serwer po odpowiedzi zapomina wszystko; każde żądanie to nowy obiekt `PageModel`/kontrolera | pola kalkulatora trzeba odesłać ręcznie (sekcja 2) |
| **formularz** `<form method="post">` | przeglądarka zbiera pola `<input name="…">` i wysyła je jako `name=value&…` | kalkulatory |
| **Tag Helpery** `asp-for`, `asp-page`, `asp-action`, `asp-validation-for` | atrybuty HTML przetwarzane **na serwerze**; generują `name`, `id`, `value`, `href`, atrybuty walidacji | od sekcji 2.5 wszędzie |
| **model binding** | framework sam mapuje pola formularza (tekst) na właściwości C# (`int`, `decimal`, `DateTime`) | `[BindProperty]` (sekcja 3), parametry akcji (sekcja 7) |
| **walidacja** | atrybuty `[Required]`, `[Range]` sprawdzane w przeglądarce (jQuery Validation) **i** na serwerze (`ModelState.IsValid`) | CRUD (sekcje 6, 8, 9) |
| **PRG** (Post‑Redirect‑Get) | po udanym POST serwer odpowiada `302 Redirect` na GET listy; F5 nie wysyła formularza drugi raz | `RedirectToPage`, `RedirectToAction` |
| **antiforgery (CSRF)** | ukryte pole `__RequestVerificationToken` w każdym formularzu POST; serwer przyjmie POST tylko z własnej strony | automatycznie w Razor Pages; `[ValidateAntiForgeryToken]` w MVC |
| **TempData** | schowek na jedno kolejne żądanie (cookie); np. komunikat „Zapisano” po przekierowaniu | sekcja 3 |

#### Database First i scaffolding

W dniu 14 robiliśmy **Code First**: klasy C# → migracje → baza. Dziś baza **już jest**, a P09 i P10 to dla niej kolejni klienci. Nie przepisujemy `Product` i `DbContext` ręcznie: EF potrafi je **wygenerować z bazy**. To **Database First** (scaffolding bazy).

| | **Code First** (dzień 14, P05) | **Database First** (dziś, P09/P10) |
|---|---|---|
| Źródło prawdy | klasy C# | baza danych |
| Kierunek | kod → `migrations add` → `database update` → baza | baza → `dbcontext scaffold` → kod |
| Kiedy | nowy projekt, sami projektujemy schemat | baza już istnieje: firmowa, od zespołu DBA, z innego systemu |
| Zalety | historia zmian w repo, jedna baza z kodu w każdym środowisku | zero ręcznego przepisywania, dokładne odwzorowanie schematu |
| Wady | trzeba pilnować migracji | po zmianie bazy ponowny scaffold (`--force`) nadpisuje pliki; własny kod tylko w `partial` |

Drugi generator to `dotnet aspnet-codegenerator` (w Visual Studio **Add → New Scaffolded Item**). Z klasy `Product` i `ShopContext` robi w kilkanaście sekund pięć ekranów: lista, szczegóły, dodawanie, edycja, usuwanie.

- Zalety: szybki start, poprawne wzorce (PRG, antiforgery, walidacja, `[Bind]`), spójny wygląd.
- Wady: kod generyczny (angielskie nagłówki, wszystkie kolumny). Trzeba go **przeczytać i zrozumieć**, bo od chwili wygenerowania to nasz kod. Ponowne generowanie nadpisuje zmiany.

Dlatego najpierw piszemy jedną stronę ręcznie, a potem generujemy resztę i porównujemy.

#### Architektura dnia

```text
                ┌──────────────────── baza MySuperShop (LocalDB) ────────────────────┐
                │                                                                     │
   P09 Razor Pages ──EF Core──┘        P10 MVC ──EF Core──┘         P05 API ──EF Core──┘
                                                                       ▲
                                                           HTTP (JSON) │
                                             P03 WPF ──────────────────┤
                                             P11 MVC ──────────────────┘
                              (P03 i P11 używają tego samego ProductHttpService z P06Shop.Shared)
```

P09 i P10 idą do bazy bezpośrednio, bo tak jest najprościej i tak wygląda wiele aplikacji wewnętrznych. P11 idzie przez API, bo tak wygląda system z wieloma klientami: jedna logika biznesowa (reguła 1000 zł) w jednym miejscu. Na końcu dnia porównujemy obie drogi.

Zaczynamy od Razor Pages i kalkulatora. Bez zrozumienia GET, POST i formularza trudno zrozumieć MVC i komunikację z API.

---

## 1. P09: nowy projekt Razor Pages i jego struktura

**Cel:** projekt `P09ShopWebApp.Client` działa na porcie 7201; wiemy, z czego składa się aplikacja Razor Pages, jak adres w przeglądarce trafia do pliku i gdzie są porty.

### Teoria

- Strona Razor to **para plików**: `.cshtml` (widok: HTML ze wstawkami C# po znaku `@`) i `.cshtml.cs` (**PageModel**: klasa z logiką). Jak XAML i code‑behind w WPF, tylko że tu code‑behind działa **na serwerze**.
- Dyrektywa `@page` robi z pliku stronę z adresem **ze ścieżki**: `Pages/Index.cshtml` → `/`, `Pages/Privacy.cshtml` → `/Privacy`, `Pages/ProductsAutoGen/Index.cshtml` → `/ProductsAutoGen`. Nie ma żadnej tabeli adresów.
- `Program.cs` ma ten sam schemat co w API: `builder.Services.AddRazorPages()` rejestruje Razor Pages w DI, `app.MapRazorPages()` kieruje adresy do stron z folderu `Pages`. Pomiędzy jest **pipeline** (potok obsługi żądania): `UseHttpsRedirection`, `UseRouting`, `UseAuthorization`. `app.MapStaticAssets()` to w .NET 10 serwowanie `wwwroot` z kompresją i cache (dawniej `UseStaticFiles()`).
- `Pages/Shared/_Layout.cshtml` to wspólny szablon (menu, stopka, Bootstrap, jQuery); `@RenderBody()` to miejsce na treść strony. `_ViewStart.cshtml` ustawia layout, `_ViewImports.cshtml` włącza Tag Helpery.

### Kroki

1. **Nowy projekt.** W Visual Studio: prawy przycisk na solucję `32Dzien16` → **Add → New Project…** → **ASP.NET Core Web App (Razor Pages)** (nie „Model‑View‑Controller”, nie „Web API”) → Project name `P09ShopWebApp.Client` → **Next** → Framework **.NET 10.0 (Long Term Support)**, Authentication type **None**, **Configure for HTTPS** zaznaczone, pozostałe odznaczone → **Create**.

   To samo z terminala (w folderze `32Dzien16`; tak powstało repozytorium):

```powershell
dotnet new webapp -n P09ShopWebApp.Client -o P09ShopWebApp.Client -f net10.0
dotnet sln 32Dzien16.slnx add P09ShopWebApp.Client/P09ShopWebApp.Client.csproj
```

2. **Porty.** Otwieramy `P09ShopWebApp.Client/Properties/launchSettings.json` i zmieniamy wylosowane porty na **7201/5201** (plik z szablonu, zmieniamy tylko liczby).

3. Prawy przycisk na `P09ShopWebApp.Client` → **Set as Startup Project** → F5 (profil **https**). Przeglądarka otwiera `https://localhost:7201/` ze stroną „Welcome”.

4. **Przegląd struktury** w Solution Explorer (wszystko z szablonu, nic nie przepisujemy):

| Element | Co to jest |
|---|---|
| `Program.cs` | rejestracja usług (`AddRazorPages`) i pipeline (`UseHttpsRedirection`, `UseRouting`, `UseAuthorization`, `MapStaticAssets`, `MapRazorPages`); w sekcji 4 dopiszemy tu `AddDbContext`, a w sekcji 6 kulturę |
| `Pages/Index.cshtml` + `Index.cshtml.cs` | strona to **para plików**: widok i PageModel |
| `@page` na górze `.cshtml` | robi z pliku stronę z adresem ze ścieżki |
| `Pages/Shared/_Layout.cshtml` | wspólny szablon: menu, stopka, Bootstrap, jQuery; `@RenderBody()` to miejsce na treść; dziś dopiszemy tu cztery linki |
| `Pages/_ViewStart.cshtml` | `Layout = "_Layout"` dla wszystkich stron |
| `Pages/_ViewImports.cshtml` | wspólne `@using` i `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`: włącza **Tag Helpery** (`asp-…`) |
| `Pages/Error.cshtml`, `Privacy.cshtml` | strona błędu (pokazywana poza Development) i przykładowa strona; zostają |
| `wwwroot/` | pliki statyczne: `css/site.css`, `js/site.js`, `lib/bootstrap`, `lib/jquery`, `lib/jquery-validation`; serwer oddaje je bez przetwarzania; walidacja w przeglądarce (sekcja 6) to właśnie `jquery-validation` |
| `appsettings.json`, `appsettings.Development.json` | konfiguracja jak w API; plik Development ma `DetailedErrors: true`; tu trafi connection string (sekcja 4) |

5. Klikamy **Privacy** w menu: adres `/Privacy`, plik `Pages/Privacy.cshtml`. W `_Layout.cshtml` link to `<a asp-page="/Privacy">`, a w źródle strony (Ctrl+U) widzimy zwykłe `href="/Privacy"`. Adres **to ścieżka pliku**. Gdy za chwilę dodamy `Pages/Calculator.cshtml`, adres `/Calculator` zacznie działać sam.

### Kod

```json
// P09ShopWebApp.Client/Properties/launchSettings.json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5201",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7201;http://localhost:5201",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### ✅ Punkt kontrolny

- `https://localhost:7201/` pokazuje stronę „Welcome”; `/Privacy` działa.
- `dotnet build 32Dzien16.slnx` → 6 projektów, bez błędów.

### Dlaczego tak

- 🔧 Porty jawne (`7201/7202/7203` dla P09/P10/P11), a nie wylosowane z szablonu. W edycjach 26/31 porty były z szablonu (u każdego inne, w 31 np. 7027/5233). Teraz wszyscy mają te same adresy („otwieramy 7203”), a przy uruchamianiu kilku projektów naraz nic nie koliduje z API (7127).
- Szablon **Razor Pages**, nie MVC: najprostszy model, w którym widać, jak adres trafia do pliku i jak strona obsługuje GET/POST.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| przeglądarka: „Your connection isn't private” | `dotnet dev-certs https --trust` w terminalu, restart przeglądarki |
| `Failed to bind to address https://localhost:7201: address already in use` | inny proces na porcie (`netstat -ano \| findstr :7201`); zamykamy go albo zmieniamy port w obu profilach |
| projektu nie widać w solucji | `dotnet sln 32Dzien16.slnx add …` albo **Add → Existing Project** |

Wiemy już, jak adres trafia do strony. Następne pytanie: jak strona **dostaje dane od użytkownika**? Najprostszy przykład: kalkulator, który dodaje dwie liczby.

---

## 2. P09: `Calculator` – GET i POST, ręczne czytanie formularza

**Cel:** strona `/Calculator` dodaje dwie liczby przez GET i POST, obsługuje błędne dane i odsyła wartości do pól; rozumiemy, jak formularz wysyła dane, czym różni się GET od POST, że HTTP jest bezstanowe i po co jest token antiforgery.

### Teoria

- Formularz `<form method="get">` wysyła pola w adresie (`?Number1=2&Number2=3`). Nazwą parametru jest atrybut **`name`**, nie `id`. `id` służy do `<label for>` i do skryptów.
- **GET** wysyła dane w adresie (query string). Adres można skopiować, zapisać w zakładkach, wysłać komuś. Dlatego GET nadaje się do **pobierania i wyszukiwania** (`/Products?search=monitor`), a nie do zmiany danych ani do haseł: trafiłyby do historii przeglądarki i logów serwera.
- `method="post"` wysyła pola w **treści** żądania; adres zostaje „czysty”.
- W PageModelu **nazwa metody decyduje**, które żądanie obsługuje: `OnGet()` obsługuje GET, `OnPost()` obsługuje POST. Dane czytamy z `Request.Query` (GET) albo `Request.Form` (POST). To zawsze **tekst**. Najpierw czytamy formularz **ręcznie**, żeby zobaczyć, co framework za chwilę zrobi za nas.
- **HTTP jest bezstanowe.** Serwer po każdym żądaniu zapomina wszystko i buduje stronę od nowa; każde żądanie dostaje **nowy** obiekt `CalculatorModel`. Wartości w polach musimy odesłać sami (`Number1 = number1`).
- **Antiforgery** (CSRF, Cross‑Site Request Forgery – fałszowanie żądań między witrynami): złośliwa strona mogłaby w przeglądarce użytkownika wysłać POST do naszej aplikacji, korzystając z jego zalogowania. Dlatego Tag Helper formularza dodaje do POST ukryte pole `__RequestVerificationToken`. Serwer przyjmie POST tylko z tokenem, który sam wystawił. Razor Pages sprawdza go **automatycznie**; w MVC trzeba dopisać atrybut `[ValidateAntiForgeryToken]` (sekcja 8).

### Kroki

1. **Pusta strona.** Prawy przycisk na folder `Pages` → **Add → Razor Page…** → **Razor Page – Empty** → **Add** → nazwa `Calculator.cshtml` → **Add**. Powstają `Calculator.cshtml` (z `@page` i `@model`) i `Calculator.cshtml.cs` z klasą `CalculatorModel : PageModel` i pustą metodą `OnGet()`. Kreator dodaje `using Microsoft.AspNetCore.Mvc;` i `using Microsoft.AspNetCore.Mvc.RazorPages;`.

   💡 Bez Visual Studio te dwa pliki tworzymy ręcznie (albo `dotnet new page -n Calculator -o Pages`). Nie ma w nich magii: klasa dziedzicząca po `PageModel` i plik z `@page`.

2. **Etap A: formularz wysyłany metodą GET.** Zastępujemy zawartość `Pages/Calculator.cshtml` wersją tymczasową:

```cshtml
@* P09ShopWebApp.Client/Pages/Calculator.cshtml  (wersja tymczasowa – etap A: GET; pełna w kroku 5) *@
@page
@model P09ShopWebApp.Client.Pages.CalculatorModel

<h1>Calculator</h1>

<form method="get">
    <label for="Number1">Number 1:</label>
    <input id="Number1" name="Number1" />
    <br />
    <label for="Number2">Number 2:</label>
    <input id="Number2" name="Number2" />
    <br />
    <input type="submit" value="Calculate sum" class="btn btn-primary mt-2" />
</form>
```

   F5, w pasku adresu `/Calculator`, wpisujemy `2` i `3`, klikamy **Calculate sum**. Adres zmienia się na `/Calculator?Number1=2&Number2=3`. Strona wygląda tak samo, bo nikt jeszcze tych danych nie czyta. Przeglądarka zbudowała adres z pól formularza.

3. **Etap B: zmiana na POST.** W `Calculator.cshtml` zamieniamy `method="get"` na `method="post"`, zapisujemy, odświeżamy `/Calculator`, wysyłamy `2` i `3`. Obserwujemy:
   1. Adres jest „czysty”: `/Calculator`.
   2. **F12 → Network** → żądanie `Calculator` → **Payload** (Chrome/Edge: „Form Data”): `Number1=2`, `Number2=3`, `__RequestVerificationToken=…`. Dane są w **treści** żądania.
   3. **Ctrl+U** (źródło strony): w formularzu jest ukryte pole `<input name="__RequestVerificationToken" type="hidden" value="CfDJ8…">`, którego nie pisaliśmy. Dodał je **Tag Helper formularza**, bo `<form method="post">` jest przetwarzany na serwerze (dzięki `@addTagHelper` w `_ViewImports`).

4. **Etap C: obsługa w PageModelu.** Dane lecą do serwera, ale nikt ich nie odbiera. Zastępujemy zawartość `Pages/Calculator.cshtml.cs` (kod niżej).

5. Zastępujemy zawartość `Pages/Calculator.cshtml` wersją końcową (kod niżej).

6. **Link w menu.** W `Pages/Shared/_Layout.cshtml` pod pozycją **Privacy** dopisujemy od razu **cztery** linki. Następne sekcje (3, 5, 6) tylko dodadzą strony, do których one prowadzą, więc nie wracamy do tego pliku. Do czasu utworzenia stron linki dają 404.

7. F5 → **Calculator**. Testujemy: `2` i `3` → `Result: 5`, pola nadal mają `2` i `3`. Potem `abc` i `3` → `Result: 0` i czerwone `Podaj dwie liczby całkowite.`. Na koniec wpisujemy ręcznie adres `/Calculator?Number1=2&Number2=3` → też `Result: 5` (to obsłużył `OnGet`).

8. W źródle strony zamiast `asp-page="/Calculator"` jest `href="/Calculator"`. Tag Helper zbudował adres z routingu. Gdy zmienimy adres strony (np. `@page "/kalkulator"`), link dostosuje się sam. Zwykłe `href="/Calculator"` trzeba by poprawiać ręcznie.

9. 🧪 **Eksperyment (2 min, opcjonalny):** w metodzie `Calculate` komentujemy linie `Number1 = number1;` i `Number2 = number2;` i wysyłamy formularz. Pola po wysłaniu są puste (`value="0"`). To dowód, że HTTP jest bezstanowe. Przywracamy obie linie.

### Kod

```csharp
// P09ShopWebApp.Client/Pages/Calculator.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Najprostszy kalkulator: formularz czytamy RĘCZNIE z Request.Form.
/// Celowo bez model bindingu – żeby zobaczyć, co framework robi za nas (patrz CalculatorBinding).
/// </summary>
public class CalculatorModel : PageModel
{
    public int Number1 { get; set; }

    public int Number2 { get; set; }

    public int Result { get; set; }

    // Komunikat zamiast pustego "else" – użytkownik widzi, co wpisał źle
    public string? ErrorMessage { get; set; }

    // GET: /Calculator – pusty formularz. Etap A (formularz method="get"): dane są w ADRESIE,
    // np. /Calculator?Number1=2&Number2=3, i czytamy je z Request.Query.
    public void OnGet()
    {
        if (Request.Query.ContainsKey("Number1"))
        {
            Calculate(Request.Query["Number1"], Request.Query["Number2"]);
        }
    }

    // POST: /Calculator – Etap B (formularz method="post"): dane są w TREŚCI żądania, czytamy je z Request.Form
    public void OnPost()
    {
        var form = Request.Form;
        Calculate(form["Number1"], form["Number2"]);
    }

    private void Calculate(string? text1, string? text2)
    {
        // Pola formularza to zawsze TEKST. TryParse zamiast Parse: użytkownik może wpisać cokolwiek.
        bool ok1 = int.TryParse(text1, out int number1);
        bool ok2 = int.TryParse(text2, out int number2);

        // HTTP jest bezstanowe: po każdym żądaniu serwer buduje stronę od nowa,
        // więc wartości do pól musimy odesłać sami, inaczej po wysłaniu pola będą puste.
        Number1 = number1;
        Number2 = number2;

        if (ok1 && ok2)
        {
            Result = number1 + number2;
        }
        else
        {
            ErrorMessage = "Podaj dwie liczby całkowite.";
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/Calculator.cshtml *@
@page
@model P09ShopWebApp.Client.Pages.CalculatorModel
@{
    ViewData["Title"] = "Calculator";
    // W widoku można pisać C#, ale większą logikę trzymamy w PageModelu (Calculator.cshtml.cs)
}

<h1>Calculator</h1>
<p>Ręczne czytanie formularza: <code>Request.Form</code> + <code>int.TryParse</code>.</p>

@* Etap A: method="get" – dane lecą w adresie (?Number1=2&Number2=3).
   Etap B: method="post" – dane lecą w treści żądania, Tag Helper dodaje token antiforgery. *@
<form method="post">
    <label for="Number1">Number 1:</label>
    <input id="Number1" name="Number1" value="@Model.Number1" />
    <br />
    <label for="Number2">Number 2:</label>
    <input id="Number2" name="Number2" value="@Model.Number2" />
    <br />
    <input type="submit" value="Calculate sum" class="btn btn-primary mt-2" />

    <p class="mt-3">Result: @Model.Result</p>

    @if (Model.ErrorMessage != null)
    {
        <p class="text-danger">@Model.ErrorMessage</p>
    }
</form>
```

🟩 **Co dopisujemy** w `P09ShopWebApp.Client/Pages/Shared/_Layout.cshtml`:

```diff
⋯
                         <li class="nav-item">
                             <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
                         </li>
+                        @* asp-page: Tag Helper buduje href z routingu – gdy zmieni się adres strony, link zmieni się sam *@
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-page="/Calculator">Calculator</a>
+                        </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-page="/CalculatorBinding">Calculator binding</a>
+                        </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-page="/ProductsList">Products List</a>
+                        </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-page="/ProductsAutoGen/Index">Products (CRUD)</a>
+                        </li>
                     </ul>
⋯
```

```cshtml
@* P09ShopWebApp.Client/Pages/Shared/_Layout.cshtml *@
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - P09ShopWebApp.Client</title>
    <script type="importmap"></script>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/P09ShopWebApp.Client.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-page="/Index">P09ShopWebApp.Client</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
                        </li>
                        @* asp-page: Tag Helper buduje href z routingu – gdy zmieni się adres strony, link zmieni się sam *@
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Calculator">Calculator</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/CalculatorBinding">Calculator binding</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/ProductsList">Products List</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/ProductsAutoGen/Index">Products (CRUD)</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2026 - P09ShopWebApp.Client - <a asp-area="" asp-page="/Privacy">Privacy</a>
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>

    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

💡 **Wyjaśnienie kodu linia po linii:**

- `Request.Form["Number1"]`: ręcznie wyciągamy wartość pola o `name="Number1"`. To **zawsze tekst** (`StringValues`), bo HTTP przesyła tekst.
- `Request.Query["Number1"]` w `OnGet`: to samo dla danych z adresu. Wspólna metoda `Calculate` pokazuje, że różnica GET/POST to tylko **skąd czytamy**: adres czy treść.
- `int.TryParse` zamiast `int.Parse`: użytkownik może wpisać cokolwiek; `TryParse` nie rzuca wyjątku, tylko zwraca `false`. `ErrorMessage` z `@if` w widoku zamiast pustego `else`.
- `Number1 = number1;`: odsyłamy wartości do pól, bo HTTP jest bezstanowe (eksperyment w kroku 9).
- `@Model.Result` w widoku: `Model` to obiekt `CalculatorModel` z tego żądania. Wszystko po `@` jest C#, reszta to HTML. Blok `@{ ViewData["Title"] = …; }` ustawia tytuł zakładki (używa go `_Layout`). W widoku można pisać C#, ale większą logikę trzymamy w PageModelu.
- Komentarz Razor `@* … *@` nie trafia do HTML (w przeciwieństwie do `<!-- -->`).
- Przyciski mają klasy Bootstrap (`btn btn-primary mt-2`), bo Bootstrap jest już w `_Layout`.

### ✅ Punkt kontrolny

- `/Calculator?Number1=2&Number2=3` → `Result: 5` (obsłużył `OnGet`).
- POST `2`/`3` → `Result: 5`, pola nadal mają `2` i `3`.
- `abc`/`3` → `Result: 0` i czerwone `Podaj dwie liczby całkowite.`.
- W menu jest **Calculator**; w źródle strony `href="/Calculator"`.

### Dlaczego tak

- `int.TryParse` zamiast `int.Parse`: aplikacja nie rzuca wyjątku przy błędnych danych. Komunikat `ErrorMessage` zamiast pustego `else`.
- Wspólna metoda `Calculate(text1, text2)` dla `OnGet` i `OnPost`: różnica GET/POST to tylko **skąd** czytamy (`Request.Query` vs `Request.Form`).
- `Number1 = number1;` – bo HTTP jest bezstanowe.
- W widoku można pisać C#, ale logika ma być w `PageModel`.
- Link przez `asp-page`, nie `href`: gdy zmieni się adres strony, link zmieni się sam.
- 🔧 W edycjach 26/31 `OnGet` był pusty, więc adres `/Calculator?Number1=2&Number2=3` dawał `Result: 0`, a etap GET pokazywał tylko adres. Gałąź `else` była pusta (błędne dane → cicho `0`), `using Microsoft.AspNetCore.Mvc` był zbędny, namespace był blokowy. Teraz `OnGet` czyta `Request.Query` (kontrast GET/POST widać w kodzie, nie tylko w adresie), jest `ErrorMessage`, wspólna `Calculate`, file‑scoped namespace i tytuł strony.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| po wysłaniu formularza `400 Bad Request` | brak tokena: formularz nie ma `method="post"` jako Tag Helpera (sprawdzamy `_ViewImports.cshtml` z `@addTagHelper`) albo wysyłamy go z innej strony |
| `Result: 0` dla `2`/`3` | nazwy `name="Number1"` w widoku i `form["Number1"]` w kodzie muszą być identyczne (wielkość liter) |
| `The name 'Model' does not exist` | brak `@model P09ShopWebApp.Client.Pages.CalculatorModel` na górze `.cshtml` |
| strona `/Calculator` daje 404 | brak `@page` w pierwszej linii pliku `.cshtml` |

Ile tu było ręcznej pracy: `Request.Form`, `TryParse`, odesłanie wartości. A gdyby formularz miał 20 pól? Framework zrobi to za nas. Ten mechanizm to **model binding**.

---

## 3. P09: `CalculatorBinding` – model binding, handlery i `TempData`

**Cel:** strona z trzema formularzami (suma, różnica, iloczyn), bez ręcznego czytania `Request.Form`; framework mapuje formularz na właściwości, kilka przycisków na jednej stronie wywołuje różne metody, a `TempData` przenosi dane do kolejnego żądania.

### Teoria

- **`[BindProperty]`** na właściwości: przy POST framework znajdzie w formularzu pole o nazwie właściwości (`name="Number1"`), zamieni tekst na `int` i wpisze do właściwości. To **model binding** (wiązanie modelu). Nie ma `Request.Form` ani `TryParse`. Jeśli ktoś wpisze `abc`, do właściwości trafi `0`, a w `ModelState` pojawi się błąd (`ModelState.IsValid` sprawdzamy w sekcji 6). Domyślnie działa tylko dla POST; dla GET trzeba `[BindProperty(SupportsGet = true)]`.
- **Handlery:** trzy formularze, każdy ma wywołać inną metodę. `asp-page-handler="Add"` na formularzu → wywoła się `OnPostAdd()`. Konwencja: **`OnPost` + nazwa handlera**. W HTML powstaje `action="/CalculatorBinding?handler=Add"`.
- **`[TempData]`**: schowek, który **przeżywa jedno kolejne żądanie** i znika po odczytaniu. Domyślnie siedzi w ciasteczku (`.AspNetCore.Mvc.CookieTempDataProvider`, widać w **F12 → Application → Cookies**). Typowe użycie: komunikat „Zapisano!” pokazany **po przekierowaniu** (PRG), bo po `Redirect` nowy obiekt strony nic nie pamięta.

### Kroki

1. Prawy przycisk na `Pages` → **Add → Razor Page…** → **Razor Page – Empty** → `CalculatorBinding.cshtml`.
2. Zastępujemy `Pages/CalculatorBinding.cshtml.cs` (kod niżej).
3. Zastępujemy `Pages/CalculatorBinding.cshtml` (kod niżej). Link **Calculator binding** jest już w menu (sekcja 2).
4. F5 → **Calculator binding**. Wpisujemy `7` i `3` w pierwszym formularzu i klikamy kolejno trzy przyciski **bez odświeżania** strony. W kolejnych formularzach też wpisujemy `7` i `3`, bo każdy formularz wysyła własne pola. Wyniki porównujemy z tabelą w punkcie kontrolnym.
5. W źródle strony sprawdzamy `action="/CalculatorBinding?handler=Add"` (i `Subtract`, `Multiply`).
6. 🧪 **Eksperyment (5 min, opcjonalny):** mnożenie jako ćwiczenie. Usuwamy z kodu właściwość `MultiplicationResult`, metodę `OnPostMultiply` i trzeci formularz, a potem dopisujemy je samodzielnie (trzecia właściwość z `[TempData]`, handler i formularz z `asp-page-handler="Multiply"`). Na koniec porównujemy z kodem poniżej.

### Kod

```csharp
// P09ShopWebApp.Client/Pages/CalculatorBinding.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Ten sam kalkulator, ale framework robi za nas to, co w Calculator pisaliśmy ręcznie:
/// [BindProperty] mapuje pola formularza na właściwości (model binding),
/// asp-page-handler wybiera metodę OnPostXxx, a [TempData] przenosi wynik do kolejnego żądania.
/// </summary>
public class CalculatorBindingModel : PageModel
{
    // Przy POST framework znajdzie pole name="Number1", zamieni tekst na int i wpisze tutaj.
    // Domyślnie działa tylko dla POST; dla GET trzeba [BindProperty(SupportsGet = true)].
    [BindProperty]
    public int Number1 { get; set; }

    [BindProperty]
    public int Number2 { get; set; }

    // TempData: schowek, który przeżywa JEDNO kolejne żądanie i znika po odczytaniu (domyślnie cookie).
    // Dlatego po kliknięciu kolejnego przycisku poprzedni wynik jest widoczny jeszcze raz, a potem znika.
    [TempData]
    public int AdditionResult { get; set; }

    [TempData]
    public int SubtractionResult { get; set; }

    [TempData]
    public int MultiplicationResult { get; set; }

    public void OnGet()
    {
    }

    // Konwencja: OnPost + nazwa handlera z asp-page-handler="Add"
    public void OnPostAdd()
    {
        AdditionResult = Number1 + Number2;
    }

    public void OnPostSubtract()
    {
        SubtractionResult = Number1 - Number2;
    }

    public void OnPostMultiply()
    {
        MultiplicationResult = Number1 * Number2;
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/CalculatorBinding.cshtml *@
@page
@model P09ShopWebApp.Client.Pages.CalculatorBindingModel
@{
    ViewData["Title"] = "Calculator binding";
}

<h1>Calculator binding</h1>
<p>Model binding (<code>[BindProperty]</code>), handlery <code>OnPostAdd/OnPostSubtract/OnPostMultiply</code> i <code>[TempData]</code>.</p>

@* asp-page-handler="Add" → action="/CalculatorBinding?handler=Add" → metoda OnPostAdd() *@
<form method="post" asp-page-handler="Add">
    <label for="Number1">Number 1:</label>
    <input id="Number1" name="Number1" value="@Model.Number1" />
    <br />
    <label for="Number2">Number 2:</label>
    <input id="Number2" name="Number2" value="@Model.Number2" />
    <br />
    <input type="submit" value="Calculate sum" class="btn btn-primary mt-2" />
    <p>Result: @Model.AdditionResult</p>
</form>

<form method="post" asp-page-handler="Subtract">
    <label for="Number1Sub">Number 1:</label>
    <input id="Number1Sub" name="Number1" value="@Model.Number1" />
    <br />
    <label for="Number2Sub">Number 2:</label>
    <input id="Number2Sub" name="Number2" value="@Model.Number2" />
    <br />
    <input type="submit" value="Calculate subtraction" class="btn btn-primary mt-2" />
    <p>Result: @Model.SubtractionResult</p>
</form>

<form method="post" asp-page-handler="Multiply">
    <label for="Number1Mul">Number 1:</label>
    <input id="Number1Mul" name="Number1" value="@Model.Number1" />
    <br />
    <label for="Number2Mul">Number 2:</label>
    <input id="Number2Mul" name="Number2" value="@Model.Number2" />
    <br />
    <input type="submit" value="Calculate multiplication" class="btn btn-primary mt-2" />
    <p>Result: @Model.MultiplicationResult</p>
</form>
```

### ✅ Punkt kontrolny

Liczby `7` i `3`, bez odświeżania między kliknięciami:

| Klikamy | Suma | Różnica | Iloczyn | Dlaczego |
|---|---|---|---|---|
| Calculate sum | 10 | 0 | 0 | policzona suma trafiła do TempData |
| Calculate subtraction | **10** | 4 | 0 | suma przyszła jeszcze raz z TempData (odczyt ją kasuje) |
| Calculate multiplication | **0** | **4** | 21 | suma już „zużyta”; różnica przyszła z TempData |
| F5 albo wejście z menu (GET) | 0 | 0 | 21 | został tylko ostatni wynik, potem i on zniknie |

Trzy przyciski działają, a każdy wiersz tabeli daje się wyjaśnić działaniem `TempData`. W źródle strony każdy formularz ma `action="/CalculatorBinding?handler=…"`.

### Dlaczego tak

- 🔧 Nazwy `AdditionResult`, `SubtractionResult`, `OnPostSubtract`. W edycjach 26/31 były literówki: `AddictionResult` („addiction” to uzależnienie), `SubstractionResult`, `OnPostSubstract`, trzy formularze z powtórzonymi `id`, tylko dodawanie i odejmowanie (mnożenie dodano w scenariuszu 26). Teraz są poprawne nazwy, unikalne `id` i mnożenie od razu. Nazwy w kodzie czyta się częściej, niż pisze.
- Trzy formularze mają **te same `name`** (`Number1`, `Number2`), bo bindują do tych samych właściwości, ale **różne `id`** (`Number1Sub`, `Number1Mul`). `id` w dokumencie HTML musi być unikalne, inaczej `<label for>` i walidacja w przeglądarce działają tylko dla pierwszego formularza.
- `TempData` zamiast zwykłej właściwości pokazuje, co dzieje się z danymi **między** żądaniami. Przyda się w P11 do komunikatu po `Redirect` (zadanie dodatkowe).

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| `Result: 0` zawsze | brak `[BindProperty]` na `Number1`/`Number2` albo inne `name` w formularzu |
| wszystkie przyciski liczą sumę | brak `asp-page-handler` (wtedy wywołuje się `OnPost`, którego nie ma → błąd) albo literówka w nazwie handlera vs metody |
| `InvalidOperationException: … TempData … int` | `[TempData]` obsługuje typy proste i stringi; obiekty trzeba serializować do JSON |

Umiemy przyjmować dane z formularzy. Sklep musi jednak pokazywać **produkty z bazy**. Baza `MySuperShop` istnieje od dnia 14. Nowy projekt nie ma `Product` ani `DbContext`. Można je przepisać ręcznie, ale lepiej **wygenerować je z bazy**.

---

## 4. P09: Database First – scaffold bazy

**Cel:** `Models/Product.cs` i `Data/ShopContext.cs` wygenerowane z **istniejącej** bazy `MySuperShop`, kontekst zarejestrowany w DI, projekt się buduje.

### Teoria

- `dotnet ef dbcontext scaffold` łączy się z bazą, czyta schemat (tabele, kolumny, typy, długości, klucze, indeksy) i generuje encje (`Products` → `Product`) oraz `DbContext` z `DbSet` i konfiguracją Fluent API.
- Potrzebne są cztery pakiety NuGet (tabela niżej). Dwa narzędzia globalne (`dotnet-ef`, `dotnet-aspnet-codegenerator`) to **programy** w systemie. Pakiety to **biblioteki** w projekcie, z którymi te programy pracują. Bez pakietu `Design` polecenie `dotnet ef` zgłosi `Your startup project doesn't reference Microsoft.EntityFrameworkCore.Design`. W dniu 14 nie dodaliśmy `Tools`; dziś go dodajemy, bo wymagają go kreator w Visual Studio i Package Manager Console (PMC).

| Pakiet | Do czego | Kiedy używany |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | **provider** (sterownik EF Core dla SQL Server): LINQ → T‑SQL; ciągnie `Microsoft.EntityFrameworkCore` | w działającej aplikacji |
| `Microsoft.EntityFrameworkCore.Design` | kod dla narzędzia `dotnet ef` (scaffold, migracje); `PrivateAssets=all` – nie trafia do wdrożenia | tylko projektowanie |
| `Microsoft.EntityFrameworkCore.Tools` | polecenia **Package Manager Console** w VS (`Scaffold-DbContext`, `Add-Migration`) | tylko projektowanie; dla osób, które wolą PMC |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | szablony i logika generatora ekranów (`dotnet aspnet-codegenerator`, kreator **New Scaffolded Item**) | tylko projektowanie |

- **`NuGetAuditLevel`.** Od .NET 8 `dotnet build` sprawdza pakiety w bazie podatności. Generator ciągnie starą wersję `NuGet.Packaging` z ostrzeżeniem o **niskiej** wadze (NU1901). Nie da się tego naprawić po naszej stronie (to zależność przechodnia), a generator nie działa w runtime. Dlatego podnosimy próg raportowania do `moderate`. Alternatywa: usunąć pakiet `CodeGeneration.Design` po wygenerowaniu CRUD. Bez tej linii build przechodzi, ale z ośmioma ostrzeżeniami.
- **Connection string.** Scaffolder musi wiedzieć, z którą bazą się połączyć. Robimy jak w P05: `DefaultConnection` w `appsettings.Development.json`, **ten sam** string, co w API. `"Name=ConnectionStrings:DefaultConnection"` w poleceniu oznacza: weź connection string z konfiguracji projektu, nie z polecenia.
- **`--no-onconfiguring`**: nie generuj `OnConfiguring`; provider i connection string poda `Program.cs` przez `AddDbContext`, jak w P05.
- Wygenerowane klasy są `partial`: własny kod dopisujemy w osobnym pliku, bo `--force` nadpisuje te pliki.

### Kroki

1. **Pakiety.** Prawy przycisk na `P09ShopWebApp.Client` → **Open in Terminal** (terminal w katalogu projektu):

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.12
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.12
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.12
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 10.0.2
```

   Czwarty pakiet jest dla generatora ekranów z sekcji 6; dodajemy go od razu, żeby nie wracać do terminala.

2. **`.csproj`.** Otwieramy `.csproj` (dwuklik na projekcie) i dopisujemy ręcznie `NuGetAuditLevel` (powód w teorii). `PackageReference` dopisało już `dotnet add package`.

3. **Connection string i logowanie SQL** w `appsettings.Development.json` (kod niżej).

4. **Scaffold.** Terminal w katalogu P09; projekt musi się kompilować:

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
```

   Wynik: `Build started...`, `Build succeeded.`, potem w Solution Explorer nowe pliki `Models/Product.cs` i `Data/ShopContext.cs`.

   Wariant z **Package Manager Console** (**Tools → NuGet Package Manager → Package Manager Console**, Default project: P09; działa dzięki pakietowi `Tools`):

```powershell
Scaffold-DbContext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context ShopContext -NoOnConfiguring -Force
```

5. **Porównanie.** Porównujemy wygenerowane pliki z cytowanymi niżej. **Nie przepisujemy ich.** ⚠️ Nagłówek `// <auto-generated>` z poleceniem jest w repozytorium dopisany ręcznie, żeby było wiadomo, skąd plik pochodzi. Prawdziwy scaffold go nie dodaje: wygenerowany plik zaczyna się od `using System;`. Nagłówek można dopisać albo pominąć.

6. **Rejestracja kontekstu** w `Program.cs` (wersja tymczasowa, **bez** kultury; kulturę dopiszemy w sekcji 6.4, gdy zobaczymy problem, który rozwiązuje).

7. `dotnet build` (Ctrl+Shift+B).

### Kod

🟩 **Co dopisujemy** w `P09ShopWebApp.Client/P09ShopWebApp.Client.csproj` (względem szablonu; `PackageReference` dopisało `dotnet add package`, `NuGetAuditLevel` ręcznie):

```diff
   <PropertyGroup>
     <TargetFramework>net10.0</TargetFramework>
     <Nullable>enable</Nullable>
     <ImplicitUsings>enable</ImplicitUsings>
+    <!-- Audyt NuGet: pakiet generatora kodu (CodeGeneration.Design) ciągnie NuGet.Packaging 6.12.1 z ostrzeżeniem
+         NU1901 o NISKIEJ wadze. Generator działa tylko w czasie projektowania, więc pokazujemy ostrzeżenia od "moderate". -->
+    <NuGetAuditLevel>moderate</NuGetAuditLevel>
   </PropertyGroup>
 
+  <ItemGroup>
+    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.12">
+      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
+      <PrivateAssets>all</PrivateAssets>
+    </PackageReference>
+    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.12" />
+    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.12">
+      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
+      <PrivateAssets>all</PrivateAssets>
+    </PackageReference>
+    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="10.0.2" />
+  </ItemGroup>
+
 </Project>
```

```xml
<!-- P09ShopWebApp.Client/P09ShopWebApp.Client.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <!-- Audyt NuGet: pakiet generatora kodu (CodeGeneration.Design) ciągnie NuGet.Packaging 6.12.1 z ostrzeżeniem
         NU1901 o NISKIEJ wadze. Generator działa tylko w czasie projektowania, więc pokazujemy ostrzeżenia od "moderate". -->
    <NuGetAuditLevel>moderate</NuGetAuditLevel>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.12">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.12">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="10.0.2" />
  </ItemGroup>

</Project>
```

🟩 **Co dopisujemy** w `P09ShopWebApp.Client/appsettings.Development.json`:

```diff
 {
+  "ConnectionStrings": {
+    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MySuperShop;Trusted_Connection=True;TrustServerCertificate=True"
+  },
   "DetailedErrors": true,
   "Logging": {
     "LogLevel": {
       "Default": "Information",
-      "Microsoft.AspNetCore": "Warning"
+      "Microsoft.AspNetCore": "Warning",
+      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
     }
   }
 }
```

```json
// P09ShopWebApp.Client/appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MySuperShop;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

💡 **Wyjaśnienie (connection string):**

- W JSON backslash jest **podwójny**: `(localdb)\\mssqllocaldb`. `TrustServerCertificate=True` jest konieczne z nowym sterownikiem (dzień 14). Elementy stringa znamy z dnia 14: `Server`, `Database`, `Trusted_Connection` (konto Windows, bez hasła).
- `appsettings.Development.json` jest doklejany tylko w trybie Development (`ASPNETCORE_ENVIRONMENT` z `launchSettings.json`). LocalDB nie ma hasła, więc plik może być w repozytorium. Dla bazy z hasłem (Azure) ten sam JSON idzie do **User Secrets**: prawy przycisk → **Manage User Secrets**. ⚠️ `dotnet ef dbcontext scaffold "Name=…"` **czyta User Secrets** tylko wtedy, gdy projekt ma `UserSecretsId` w `.csproj` (dodaje go pierwsze **Manage User Secrets**).
- `Microsoft.EntityFrameworkCore.Database.Command: Information`: każdy SQL trafia do konsoli, jak w API. Konsola P09 otwiera się razem z F5 (okno `dotnet.exe`).

💡 **Wyjaśnienie (co robi polecenie scaffold, krok po kroku):**

1. **Buduje projekt** (dlatego kod musi się kompilować).
2. **Czyta connection string**: `Name=…` oznacza „weź z konfiguracji projektu klucz `ConnectionStrings:DefaultConnection`” (tak jak `GetConnectionString`). Konfigurację czyta jak aplikacja: `appsettings.json`, `appsettings.Development.json` (środowisko Development bierze z `launchSettings.json`), User Secrets.
3. **Łączy się z bazą** `MySuperShop` i czyta jej **schemat**: tabele, kolumny, typy, długości, klucze, indeksy.
4. **Generuje encję** dla każdej tabeli: `Products` → klasa `Product` (liczba pojedyncza), kolumna → właściwość odpowiedniego typu.
5. **Generuje kontekst** `ShopContext : DbContext` z `DbSet<Product> Products` i konfiguracją we Fluent API (`OnModelCreating`) dla wszystkiego, co nie wynika z konwencji.

| Fragment | Znaczenie |
|---|---|
| `dotnet ef dbcontext scaffold` | „wygeneruj DbContext i encje z bazy” |
| `"Name=ConnectionStrings:DefaultConnection"` | connection string z konfiguracji, nie w poleceniu |
| `Microsoft.EntityFrameworkCore.SqlServer` | provider, którym czytać bazę (musi pasować do typu bazy) |
| `--output-dir Models` | folder na encje |
| `--context-dir Data` | folder na kontekst (jak `Data/DataContext.cs` w P05) |
| `--context ShopContext` | nazwa klasy; bez tego byłoby `MySuperShopContext` |
| `--no-onconfiguring` | **nie generuj** `OnConfiguring` z connection stringiem; konfiguracja będzie w `Program.cs` |
| `--force` | nadpisz pliki, jeśli istnieją (po zmianie bazy uruchamiamy ponownie) |

Przydatne opcje, których dziś nie używamy: `--table Products` (tylko wybrane tabele), `--data-annotations` (atrybuty `[MaxLength]` zamiast Fluent API), `--schema dbo`, `--namespace`.

```csharp
// P09ShopWebApp.Client/Models/Product.cs  (wygenerowane przez dotnet ef dbcontext scaffold – nie przepisujemy, porównujemy; nagłówek <auto-generated> dopisany ręcznie)
// <auto-generated>
// Plik wygenerowany z istniejącej bazy MySuperShop (Database First) poleceniem:
//   dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
// Ponowne uruchomienie z --force nadpisze ten plik – własny kod dopisuj w osobnym pliku partial.
// </auto-generated>
using System;
using System.Collections.Generic;

namespace P09ShopWebApp.Client.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; }
}
```

💡 **Wyjaśnienie (`Product`):**

- `partial class`: klasę można rozbić na kilka plików. Własne metody dopisujemy w **osobnym** pliku `partial`, bo ponowny scaffold z `--force` nadpisze ten.
- `= null!;`: projekt ma `Nullable` włączone; kolumny są `NOT NULL`, więc scaffolder mówi kompilatorowi „to będzie ustawione”. W P05 pisaliśmy `= string.Empty`.
- `decimal Price`: w bazie `decimal(8,2)` (z `HasPrecision(8, 2)` z dnia 14). `string Title` bez `[MaxLength]`, bo długość jest we Fluent API kontekstu (bez `--data-annotations`).
- **Brak atrybutów walidacji** (`[Required]`, `[Range]`): baza ich nie zna. Walidacja w P09 to tylko typy i `NOT NULL`. Dla porównania `P06Shop.Shared/Models/Product.cs` ma walidację po polsku. Z tego skorzysta P11.

```csharp
// P09ShopWebApp.Client/Data/ShopContext.cs  (wygenerowane przez dotnet ef dbcontext scaffold – nie przepisujemy, porównujemy; nagłówek <auto-generated> dopisany ręcznie)
// <auto-generated>
// Plik wygenerowany z istniejącej bazy MySuperShop (Database First) poleceniem:
//   dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
// --no-onconfiguring: brak metody OnConfiguring – provider i connection string podaje Program.cs (AddDbContext),
// a kontekst dostaje je przez konstruktor z DbContextOptions<ShopContext>.
// </auto-generated>
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Data;

public partial class ShopContext : DbContext
{
    public ShopContext()
    {
    }

    public ShopContext(DbContextOptions<ShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Barcode).HasMaxLength(12);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

💡 **Wyjaśnienie (`ShopContext`):**

- Dwa konstruktory: bezparametrowy (dla narzędzi) i z `DbContextOptions<ShopContext>` (dla DI; tego użyje `AddDbContext`). Scaffolder generuje oba także z `--no-onconfiguring`.
- `DbSet<Product> Products { get; set; }` jako `virtual` i bez `= null!`: styl generatora (`DbSet` EF inicjalizuje sam). W P05 pisaliśmy `=> Set<Product>()`; oba działają.
- `OnModelCreating`: `HasMaxLength(100)` dla `Title`, `HasMaxLength(12)` dla `Barcode`, `HasColumnType("decimal(8, 2)")` dla `Price`. To są **nasze** reguły z dnia 14 (`[MaxLength]` i `HasPrecision`), odczytane z bazy. `Description` nie ma wpisu, bo `nvarchar(max)` to konwencja. EF Core nie wie, że `HasPrecision` i `HasColumnType("decimal(8, 2)")` to u nas to samo; generuje wersję dla SQL Server.
- `OnModelCreatingPartial`: hak na własną konfigurację w osobnym pliku `partial`, odporny na `--force`.
- Brak `OnConfiguring` (efekt `--no-onconfiguring`). Provider i connection string dostaje `Program.cs`.

💡 **Porównanie z dniem 14:** w P05 napisaliśmy `Product`, `DataContext` i migracje, a EF utworzył tabelę. Dziś z tabeli EF wygenerował klasy. Kod wygląda prawie tak samo, bo **model w obu kierunkach jest ten sam**: `DbContext`, `DbSet`, Fluent API. Różnica: tu nie ma folderu `Migrations` i nie wolno go tworzyć, bo schematem rządzi API.

🟩 **Co dopisujemy** w `P09ShopWebApp.Client/Program.cs` (względem szablonu):

```diff
+using Microsoft.EntityFrameworkCore;
+using P09ShopWebApp.Client.Data;
+
 var builder = WebApplication.CreateBuilder(args);
 
 // Add services to the container.
 builder.Services.AddRazorPages();
 
+// Database First: ShopContext wygenerowany z bazy (dotnet ef dbcontext scaffold).
+// Provider i connection string podajemy tutaj, nie w OnConfiguring – tak samo jak DataContext w P05.
+// AddDbContext = Scoped: jeden ShopContext na jedno żądanie HTTP.
+builder.Services.AddDbContext<ShopContext>(options =>
+    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
+
 var app = builder.Build();
⋯
```

```csharp
// P09ShopWebApp.Client/Program.cs  (wersja tymczasowa – bez kultury; pełna w sekcji 6.4)
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

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
```

💡 **Wyjaśnienie (`AddDbContext`):** to dokładnie ta sama linia, co w `P05Shop.API/Program.cs` z dnia 14. Rejestruje `ShopContext` jako **Scoped** (jeden na żądanie HTTP). Każdy `PageModel`, który poprosi o `ShopContext` w konstruktorze, dostanie gotowy obiekt z connection stringiem. `UseSqlServer` wybiera provider; w testach tej instrukcji był tu `UseSqlite` (Dodatek B).

### ✅ Punkt kontrolny

- Build bez błędów i ostrzeżeń (bez `NuGetAuditLevel` byłoby 8 ostrzeżeń NU1901).
- `Models/Product.cs`: `Id`, `Title`, `Description`, `Barcode`, `decimal Price`, `DateTime ReleaseDate`, `= null!` przy stringach.
- `Data/ShopContext.cs`: dwa konstruktory, `DbSet<Product> Products`, w `OnModelCreating` `HasMaxLength(12)`, `HasColumnType("decimal(8, 2)")`, `HasMaxLength(100)`; **brak** `OnConfiguring`.
- W przeglądarce nic nowego: żadna strona nie używa jeszcze bazy.

### Dlaczego tak

- 🔧 `--no-onconfiguring`: w edycji 26 scaffold generował `OnConfiguring` z `UseSqlServer("Name=…")`, które trzeba było ręcznie komentować, bo konfigurację robi DI; w 31 dodatkowo pojawiał się `#warning` w kodzie, gdy connection string był w poleceniu. Teraz jedna opcja i nie ma czego komentować.
- 🔧 `Name=…` zamiast connection stringa w poleceniu: string nie trafia do kodu ani do historii terminala. Notatki 26 podawały connection string wprost w poleceniu (`"Data Source=(localdb)\mssqllocaldb;…"`): to działa, ale wtedy trafia do kodu.
- 🔧 Ten sam connection string, co w P05 (z `TrustServerCertificate=True`, wymaganym przez nowy `Microsoft.Data.SqlClient`): P09, P10 i API widzą tę samą bazę. W edycji 26 był `Data Source=…;Initial Catalog=…;Integrated Security=True` bez `TrustServerCertificate` (błąd certyfikatu z `Microsoft.Data.SqlClient` 4+), a w P09 edycji 26 connection string do Azure z hasłem w `appsettings.json` (trafił na GitHuba). Teraz: bez haseł, z logowaniem SQL. Connection string w `appsettings.Development.json` nie idzie na produkcję; LocalDB nie ma hasła, więc może być w repozytorium.
- Nie tworzymy folderu `Migrations` w P09: schematem rządzi API (Code First). Dwa projekty z migracjami do jednej bazy skończyłyby się konfliktem.
- 🔧 `NuGetAuditLevel=moderate`: w edycjach 26/31 były trzy pakiety EF + generator bez tej linii (ostrzeżenia NU1901 w buildzie). Teraz build jest czysty.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| `Your startup project doesn't reference Microsoft.EntityFrameworkCore.Design` | brak pakietu `Design` albo terminal nie w katalogu projektu |
| `A named connection string was used, but the name 'DefaultConnection' was not found` | brak sekcji `ConnectionStrings` w `appsettings.Development.json`; albo plik Development nie jest czytany (zmienna `ASPNETCORE_ENVIRONMENT` w `launchSettings.json` ma być `Development`; w razie czego `$env:ASPNETCORE_ENVIRONMENT="Development"` w terminalu) |
| `Login failed` / `Cannot open database "MySuperShop"` | LocalDB nie działa (`sqllocaldb start mssqllocaldb`) albo bazy nie ma (`dotnet ef database update` w P05) |
| `The certificate chain was issued by an authority that is not trusted` | brak `TrustServerCertificate=True` |
| `Build failed. Use dotnet build to see the errors.` | projekt się nie kompiluje; naprawiamy błędy z sekcji 2–3 |
| po scaffoldzie ostrzeżenia nullable w `ShopContext` | wygenerowany plik nie daje ostrzeżeń; jeśli są, sprawdzamy, czy nie zmieniono `virtual DbSet` |

Mamy `Product` i kontekst podłączony do bazy. Pierwszą stronę z produktami piszemy **ręcznie**, żeby zobaczyć, jak to działa. Potem generator zrobi cały CRUD.

---

## 5. P09: ręczna lista produktów

**Cel:** `/ProductsList` pokazuje 100 produktów z bazy; `ShopContext` jest wstrzyknięty przez DI do PageModelu; dane wyświetla pętla Razor; SQL widać w konsoli.

### Teoria

- **Konstruktor z `ShopContext`** (`ProductsListModel(ShopContext context)`): nigdzie nie piszemy `new ShopContext()`. Framework tworzy `PageModel` na każde żądanie i **wstrzykuje** kontekst z `Program.cs`. To samo DI, co w WPF (dzień 12) i w API (dzień 13).
- `_context.Products.OrderBy(p => p.Id).ToList()`: LINQ → `SELECT … ORDER BY`. `OrderBy` jawnie, bo baza bez `ORDER BY` nie gwarantuje kolejności.
- `@foreach` to pętla C# w widoku; wewnątrz zwykły HTML z `@item.Title`. Razor sam koduje HTML (`<` → `&lt;`), więc tytuł ze znakami specjalnymi nie zepsuje strony (ochrona przed XSS, Cross‑Site Scripting).
- `@Html.DisplayFor(x => item.Price)` zamiast `@item.Price`: formatuje według typu i atrybutów (`[DisplayFormat]`, `[DataType]`); generator użyje tego wszędzie.
- `@@foreach` w tekście akapitu: podwójne `@` to sposób na literalny znak `@` w Razorze.

### Kroki

1. Prawy przycisk na `Pages` → **Add → Razor Page…** → **Razor Page – Empty** → `ProductsList.cshtml`.
2. Zastępujemy `Pages/ProductsList.cshtml.cs` (kod niżej).
3. Zastępujemy `Pages/ProductsList.cshtml` (kod niżej).
4. F5 → **Products List**. Obserwujemy okno konsoli, które otworzyło się razem z aplikacją.

### Kod

```csharp
// P09ShopWebApp.Client/Pages/ProductsList.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Ręczna lista produktów: ShopContext przychodzi przez DI (zarejestrowany w Program.cs),
/// nigdzie nie piszemy new ShopContext().
/// </summary>
public class ProductsListModel : PageModel
{
    private readonly ShopContext _context;

    public List<Product> Products { get; set; } = new();

    public ProductsListModel(ShopContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        // EF Core wysyła SELECT ... FROM Products (widać w konsoli: "Executed DbCommand")
        Products = _context.Products.OrderBy(p => p.Id).ToList();
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsList.cshtml *@
@page
@model P09ShopWebApp.Client.Pages.ProductsListModel
@{
    ViewData["Title"] = "Products List";
}

<h1>Products List</h1>
<p>Lista napisana ręcznie: <code>ShopContext</code> wstrzyknięty do PageModelu, pętla <code>@@foreach</code> w widoku. Produktów: @Model.Products.Count</p>

<table class="table table-striped">
    <thead>
        <tr>
            <th>Id</th>
            <th>Title</th>
            <th>Description</th>
            <th>Price</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model.Products)
        {
            <tr>
                <td>@item.Id</td>
                <td>@item.Title</td>
                <td>@item.Description</td>
                @* DisplayFor formatuje wartość według typu i atrybutów (np. [DisplayFormat]) *@
                <td>@Html.DisplayFor(x => item.Price)</td>
            </tr>
        }
    </tbody>
</table>
```

### ✅ Punkt kontrolny

- Tabela ze 100 wierszami (`Id` 1–100, pierwszy `Fantastic Fresh Gloves`), nad nią `Produktów: 100`.
- W konsoli P09: `Executed DbCommand (… ms) … SELECT [p].[Id], [p].[Barcode], [p].[Description], [p].[Price], [p].[ReleaseDate], [p].[Title] FROM [Products] AS [p] ORDER BY [p].[Id]`.

### Dlaczego tak

- To samo DI, co w WPF i API: klasa prosi o zależność w konstruktorze, kontener ją dostarcza. `ShopContext` jest Scoped, więc żyje tyle, co jedno żądanie.
- 🔧 W edycjach 26/31 tabela była „goła”: bez `<thead>` i klas CSS, `ToList()` bez `OrderBy`, bez licznika. Teraz jest Bootstrap `table table-striped` (Bootstrap z `_Layout`), kolumna `Id` i licznik `Produktów: N`. Ten sam licznik będzie w P11, żeby porównać: 100 z bazy, 100 z API, 0 bez API.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| `Unable to resolve service for type 'ShopContext'` | brak `AddDbContext<ShopContext>` w `Program.cs` (sekcja 4, krok 6) |
| `Invalid object name 'Products'` | connection string wskazuje inną/pustą bazę; sprawdzamy `Database=MySuperShop` |
| nie widać SQL w konsoli | brak `Microsoft.EntityFrameworkCore.Database.Command: Information` w `appsettings.Development.json` |

Lista to tylko **Read**. Pełny CRUD to 10 plików i godzina pracy. Generator zrobi to w kilkanaście sekund, a my ten kod **przeczytamy**.

---

## 6. P09: CRUD z generatora i problem z przecinkiem

**Cel:** pięć stron CRUD w `Pages/ProductsAutoGen` wygenerowanych narzędziem; rozumiemy wygenerowany kod (binding całego obiektu, `ModelState`, PRG, Delete przez POST); CRUD działa; cena z kropką przechodzi dzięki `UseRequestLocalization`.

### Teoria

- Generator `dotnet aspnet-codegenerator razorpage` buduje projekt, przez **refleksję** czyta `Product` (właściwości, typy, klucz `Id`) i `ShopContext` (który `DbSet` to produkty), bierze wbudowane szablony pięciu stron i wypełnia je nazwami naszych właściwości. Powstają `Index`, `Create`, `Edit`, `Details`, `Delete` (po dwa pliki każda). Dlatego generator wymaga zbudowanego projektu i zarejestrowanego kontekstu (`AddDbContext` w `Program.cs`).
- `[BindProperty] public Product Product`: binding **całego obiektu** (pola `name="Product.Title"`). `ModelState.IsValid`: czy binding i walidacja się udały.
- **PRG**: po udanym zapisie `RedirectToPage("./Index")` (`302`), a nie renderowanie strony; F5 na liście nie powtórzy zapisu.
- `Delete`: GET pokazuje potwierdzenie, POST usuwa. **Nigdy nie usuwamy przez GET.**
- Przeglądarka (jQuery Validation) przyjmuje w liczbie tylko kropkę, a serwer na polskim Windows (kultura pl‑PL) oczekuje przecinka. `UseRequestLocalization` z `en-US` ustawia jedną kulturę dla wszystkich żądań.

### Kroki

1. **Generowanie (6.1).** Terminal w katalogu P09; projekt zbudowany, `AddDbContext` w `Program.cs` jest konieczny, inaczej generator nie znajdzie kontekstu:

```powershell
dotnet build
dotnet aspnet-codegenerator razorpage -m Product -dc ShopContext -udl -outDir Pages/ProductsAutoGen --referenceScriptLibraries
```

   Wyjście: `Building project ...`, `Finding the generator 'razorpage'...`, `Added Razor Page … Create.cshtml`, `… Edit`, `… Details`, `… Delete`, `… Index` (`RunTime 00:00:14`). W Solution Explorer pojawia się folder `Pages/ProductsAutoGen` z 10 plikami. Link **Products (CRUD)** jest już w menu (sekcja 2).

| Parametr CLI | Znaczenie |
|---|---|
| `razorpage` | generator stron Razor (w sekcji 8 będzie `controller`) |
| `-m Product` | model (encja) |
| `-dc ShopContext` | DbContext, którego mają używać strony |
| `-udl` | use default layout (`_Layout`) |
| `-outDir Pages/ProductsAutoGen` | folder docelowy → adres `/ProductsAutoGen` |
| `--referenceScriptLibraries` | dołącz `_ValidationScriptsPartial` (walidacja w przeglądarce) do `Create` i `Edit` |

   **Alternatywa w Visual Studio** (kreator; wynik jest taki sam, jak z CLI): prawy przycisk na `Pages` → **Add → New Folder** `ProductsAutoGen` → prawy przycisk na nim → **Add → New Scaffolded Item…** → po lewej **Razor Pages**, na liście **Razor Pages using Entity Framework (CRUD)** → **Add** → **Model class:** `Product (P09ShopWebApp.Client.Models)`, **DbContext class:** `ShopContext (P09ShopWebApp.Client.Data)`, **Use a layout page** zaznaczone, pole puste → **Add**. Kreator może doinstalować `Microsoft.VisualStudio.Web.CodeGeneration.Design` (już jest) i `Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter` (zbędny; można go usunąć).

2. **Przegląd wygenerowanego kodu (6.2).** Wszystkie 10 plików w sekcji „Kod” jest **wygenerowanych przez `dotnet aspnet-codegenerator`** – nie przepisujemy ich, tylko porównujemy (styl generatora: blokowe namespace, pełne nazwy typów, `default!`). Czytamy dokładnie `.cshtml.cs`, widoki pobieżnie. Komentarze są pod każdą parą plików.

3. **Test CRUD (6.3).** F5 → **Products (CRUD)** → 100 wierszy → **Create New** → Title `Klawiatura mechaniczna`, Description `Klawiatura z podświetleniem RGB`, Barcode `590123456789`, ReleaseDate `2024-05-01 10:00`, Price `249.99` (z kropką) → **Create**. W **F12 → Network**: `Create` → `302`, `Index` → `200`. Na liście 101 wierszy, ostatni `Klawiatura mechaniczna`; w konsoli `INSERT INTO [Products] …`. Potem **Edit** (cena `199.99`) → **Details** (`199.99`) → **Delete** → **Delete** → 100 wierszy; w konsoli `UPDATE … SET …`, `DELETE FROM [Products] WHERE [Id] = @p0`.

4. **Przecinek.** Jeszcze raz **Create New**, Price `249,99` (z **przecinkiem**) → **Create**. Pod polem od razu pojawia się `The field Price must be a number.`, **bez** żądania do serwera (Network pusty). To walidacja **w przeglądarce** (jQuery Validation), która zna tylko kropkę.

5. **Kultura (6.4).** Druga strona medalu: na polskim Windows serwer działa w kulturze **pl‑PL**, więc oczekuje przecinka. Wpisana z kropką `249.99` w niektórych konfiguracjach wraca z serwera jako `The value '249.99' is not valid for Price.`, a jeśli przejdzie, to w bazie może wylądować `24999`. Przeglądarka chce kropki, serwer przecinka. To klasyczny problem ASP.NET na polskich komputerach. Rozwiązanie na dziś: **jedna kultura dla całej aplikacji**, `en-US`, ustawiona w pipeline. Dopisujemy dwa `using` i blok po `UseHttpsRedirection` w `Program.cs` (wersja końcowa, kod niżej).

6. Restart (Ctrl+Shift+F5), **Create New** z `249.99` → produkt na liście; potem Edit → Details → Delete jak w kroku 3.

### Kod

```csharp
// P09ShopWebApp.Client/Pages/ProductsAutoGen/Index.cshtml.cs  (wygenerowane przez dotnet aspnet-codegenerator – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class IndexModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public IndexModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Product = await _context.Products.ToListAsync();
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsAutoGen/Index.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@page
@model P09ShopWebApp.Client.Pages.ProductsAutoGen.IndexModel

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-page="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Product[0].Title)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Product[0].Description)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Product[0].Barcode)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Product[0].Price)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Product[0].ReleaseDate)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model.Product) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Title)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Description)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Barcode)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Price)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ReleaseDate)
            </td>
            <td>
                <a asp-page="./Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-page="./Details" asp-route-id="@item.Id">Details</a> |
                <a asp-page="./Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

💡 `Index`: `OnGetAsync()` i `ToListAsync()` to to samo, co w `ProductsList`, tylko **asynchronicznie**: wątek serwera nie czeka bezczynnie na bazę i obsługuje w tym czasie inne żądania. `IList<Product> Product` ma liczbę pojedynczą (nazwa z generatora). W widoku `@Html.DisplayNameFor(model => model.Product[0].Title)` bierze nazwę kolumny z właściwości (albo z `[Display(Name = …)]`), a linki `asp-page="./Edit" asp-route-id="@item.Id"` dają `Edit?id=5`.

```csharp
// P09ShopWebApp.Client/Pages/ProductsAutoGen/Create.cshtml.cs  (wygenerowane – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class CreateModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public CreateModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsAutoGen/Create.cshtml  (wygenerowane – nie przepisujemy, porównujemy; wcięcia na górze pliku są takie, jak zostawił generator) *@
@page
@model P09ShopWebApp.Client.Pages.ProductsAutoGen.CreateModel

        @{
        ViewData["Title"] = "Create";
        }
        
        <h1>Create</h1>
        
    <h4>Product</h4>
    <hr />
    <div class="row">
    <div class="col-md-4">
    <form method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Product.Title" class="control-label"></label>
                <input asp-for="Product.Title" class="form-control" />
                <span asp-validation-for="Product.Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Description" class="control-label"></label>
                <input asp-for="Product.Description" class="form-control" />
                <span asp-validation-for="Product.Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Barcode" class="control-label"></label>
                <input asp-for="Product.Barcode" class="form-control" />
                <span asp-validation-for="Product.Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Price" class="control-label"></label>
                <input asp-for="Product.Price" class="form-control" />
                <span asp-validation-for="Product.Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.ReleaseDate" class="control-label"></label>
                <input asp-for="Product.ReleaseDate" class="form-control" />
                <span asp-validation-for="Product.ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-page="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

💡 `Create`:

- `[BindProperty] public Product Product`: binding **całego obiektu**. W widoku pola mają `asp-for="Product.Title"`, więc `name="Product.Title"`, a binder rozumie kropkę. Dla porównania w kalkulatorze było `name="Number1"`.
- `ModelState.IsValid`: czy binding się udał (np. `Price` to liczba, `ReleaseDate` to data) i czy przeszły atrybuty walidacji (tu ich nie ma, bo `Product` ze scaffoldu ich nie ma). Jeśli nie: `return Page()` z komunikatami.
- `_context.Products.Add(Product); await _context.SaveChangesAsync();` → `INSERT`.
- `RedirectToPage("./Index")`: **PRG**. Po udanym zapisie nie renderujemy strony, tylko odsyłamy `302` na listę. Dzięki temu F5 na liście nie wyśle formularza drugi raz, więc nie powstanie duplikat. W F12 widać: `POST Create → 302`, potem `GET Index → 200`.
- Widok: `asp-for` generuje `id`, `name`, `value`, `type` (`number` dla `decimal`, `datetime-local` dla `DateTime`) i atrybuty `data-val-*`; `asp-validation-for` to miejsce na komunikat; `asp-validation-summary="ModelOnly"` zbiera błędy niezwiązane z konkretnym polem (w P11 pokażemy tam komunikaty z API). `@section Scripts` z `_ValidationScriptsPartial` dołącza jQuery Validation.

```csharp
// P09ShopWebApp.Client/Pages/ProductsAutoGen/Edit.cshtml.cs  (wygenerowane – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class EditModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public EditModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsAutoGen/Edit.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@page
@model P09ShopWebApp.Client.Pages.ProductsAutoGen.EditModel

@{
    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

<h4>Product</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <input type="hidden" asp-for="Product.Id" />
            <div class="form-group">
                <label asp-for="Product.Title" class="control-label"></label>
                <input asp-for="Product.Title" class="form-control" />
                <span asp-validation-for="Product.Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Description" class="control-label"></label>
                <input asp-for="Product.Description" class="form-control" />
                <span asp-validation-for="Product.Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Barcode" class="control-label"></label>
                <input asp-for="Product.Barcode" class="form-control" />
                <span asp-validation-for="Product.Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.Price" class="control-label"></label>
                <input asp-for="Product.Price" class="form-control" />
                <span asp-validation-for="Product.Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Product.ReleaseDate" class="control-label"></label>
                <input asp-for="Product.ReleaseDate" class="form-control" />
                <span asp-validation-for="Product.ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-page="./Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

💡 `Edit`: `OnGetAsync(int? id)`: `id` przychodzi z adresu `Edit?id=5` (model binding z query stringa do parametru). `_context.Attach(Product).State = EntityState.Modified`: aktualizacja **bez ponownego pobierania** z bazy; EF wyśle `UPDATE` wszystkich kolumn (w P05 w dniu 14 robiliśmy to inaczej: pobranie i przepisanie pól, `UPDATE` tylko zmienionych). `DbUpdateConcurrencyException`: ktoś w międzyczasie usunął rekord → `404`. W widoku jest ukryte pole `asp-for="Product.Id"`; bez niego binder nie wiedziałby, który wiersz zmienić.

```csharp
// P09ShopWebApp.Client/Pages/ProductsAutoGen/Details.cshtml.cs  (wygenerowane – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class DetailsModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public DetailsModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product is not null)
            {
                Product = product;

                return Page();
            }

            return NotFound();
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsAutoGen/Details.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@page
@model P09ShopWebApp.Client.Pages.ProductsAutoGen.DetailsModel

@{
    ViewData["Title"] = "Details";
}

<h1>Details</h1>

<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Title)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Title)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Description)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Description)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Barcode)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Barcode)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Price)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Price)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.ReleaseDate)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.ReleaseDate)
        </dd>
    </dl>
</div>
<div>
    <a asp-page="./Edit" asp-route-id="@Model.Product.Id">Edit</a> |
    <a asp-page="./Index">Back to List</a>
</div>
```

💡 `Details`: `@Html.DisplayNameFor` / `@Html.DisplayFor` w `<dl class="row">`. Nagłówki są angielskie (`Title`, `Price`), bo to nazwy właściwości; `[Display(Name = "Cena")]` na modelu zmieniłoby je (zadanie dodatkowe).

```csharp
// P09ShopWebApp.Client/Pages/ProductsAutoGen/Delete.cshtml.cs  (wygenerowane – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class DeleteModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public DeleteModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product is not null)
            {
                Product = product;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                Product = product;
                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
```

```cshtml
@* P09ShopWebApp.Client/Pages/ProductsAutoGen/Delete.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@page
@model P09ShopWebApp.Client.Pages.ProductsAutoGen.DeleteModel

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Title)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Title)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Description)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Description)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Barcode)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Barcode)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.Price)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.Price)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Product.ReleaseDate)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Product.ReleaseDate)
        </dd>
    </dl>
    
    <form method="post">
        <input type="hidden" asp-for="Product.Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-page="./Index">Back to List</a>
    </form>
</div>
```

💡 `Delete`: GET pokazuje potwierdzenie (`<dl>` z danymi), POST usuwa (`FindAsync` → `Remove` → `SaveChangesAsync` → `RedirectToPage`). **Nigdy nie usuwamy przez GET.** Link `Delete?id=5` mógłby „kliknąć” robot wyszukiwarki albo rozszerzenie przeglądarki, które wczytuje linki z wyprzedzeniem. Zmiana stanu zawsze idzie przez POST z tokenem.

🟩 **Co dopisujemy** w `P09ShopWebApp.Client/Program.cs` (dwa `using` i blok po `UseHttpsRedirection`):

```diff
+using System.Globalization;
+using Microsoft.AspNetCore.Localization;
 using Microsoft.EntityFrameworkCore;
 using P09ShopWebApp.Client.Data;
⋯
 app.UseHttpsRedirection();
 
+// Jedna kultura dla całej aplikacji (kropka jako separator dziesiętny).
+// Bez tego na polskim Windowsie cena "249.99" nie przechodzi walidacji na serwerze (pl-PL chce przecinka),
+// a "249,99" nie przechodzi walidacji w przeglądarce (jQuery Validation chce kropki).
+var appCulture = new CultureInfo("en-US");
+app.UseRequestLocalization(new RequestLocalizationOptions
+{
+    DefaultRequestCulture = new RequestCulture(appCulture),
+    SupportedCultures = new List<CultureInfo> { appCulture },
+    SupportedUICultures = new List<CultureInfo> { appCulture }
+});
+
 app.UseRouting();
⋯
```

```csharp
// P09ShopWebApp.Client/Program.cs
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
```

💡 **Wyjaśnienie (`UseRequestLocalization`):**

- `UseRequestLocalization` to kolejny krok w pipeline: dla każdego żądania ustawia kulturę wątku na `en-US`, więc model binding parsuje liczby z kropką, tak jak przeglądarka. Kolejność ma znaczenie: po `UseHttpsRedirection`, przed `UseRouting`, żeby kultura obowiązywała, zanim strona zacznie bindować dane.
- Skutek uboczny: daty wyświetlają się po amerykańsku (`5/1/2024 10:00:00 AM`). Pełne rozwiązanie z polską kulturą (lokalizacja skryptów walidacji, `[DisplayFormat]`) to temat na inny dzień.
- ⚠️ Pułapka: z `en-US` serwer **nie odrzuca** `249,99`, tylko traktuje przecinek jako separator tysięcy i zapisuje `24999.00`. W przeglądarce ratuje nas jQuery Validation; żądanie wysłane poza przeglądarką (curl, Postman) przejdzie. W P11 `[Range(0.01, 100000)]` ratuje tylko częściowo (24 999 mieści się w zakresie). Wniosek: formatu liczb pilnujemy po **obu** stronach (tak zachował się kod w testach, Dodatek B).

### ✅ Punkt kontrolny

- `/ProductsAutoGen` → 100 wierszy; po Create 101 (`Klawiatura mechaniczna` na końcu); Edit `199.99`, Details `199.99`; po Delete 100.
- Konsola: `INSERT INTO [Products] …`, `UPDATE [Products] SET …`, `DELETE FROM [Products] WHERE [Id] = @p0`.
- `249,99` zatrzymuje przeglądarka; `249.99` przechodzi.
- Daty w tabeli w formacie amerykańskim (`5/1/2024 10:00:00 AM`): efekt `en-US`.

### Dlaczego tak

- 🔧 Generator z CLI, kreator VS jako alternatywa (w edycjach 26/31 odwrotnie): CLI działa wszędzie (VS Code, Linux, skrypty), jest powtarzalne bez VS i tak powstało repozytorium. Oba polecenia są w README.
- 🔧 Kultura: w edycji 26 (kod) problem obchodzono tylko przy wyświetlaniu (`String.Format(CultureInfo.InvariantCulture, …)` w liście MVC); w 31 był `Configure<RequestLocalizationOptions>` + `UseRequestLocalization()` bez parametru. Teraz opcje są inline po `UseHttpsRedirection` (jak w scenariuszu 26; mniej kodu do pokazania) z komentarzem, dlaczego.
- Nagłówki kolumn po angielsku (`Title`, `Price`), bo to nazwy właściwości; `[Display(Name = "Cena")]` by je zmieniło (zadanie dodatkowe).

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| `No DbContext named 'ShopContext' was found` / generator nie widzi modelu | projekt niezbudowany albo brak `AddDbContext` w `Program.cs`; `dotnet build` i ponawiamy |
| `Could not execute because the specified command or file was not found` dla `dotnet aspnet-codegenerator` | narzędzie niezainstalowane (sekcja 0) albo brak `%USERPROFILE%\.dotnet\tools` w PATH (nowe okno terminala) |
| generator: `Package Microsoft.VisualStudio.Web.CodeGeneration.Design is required` | brak pakietu z sekcji 4, krok 1 |
| `The value '249.99' is not valid for Price.` z serwera | brak `UseRequestLocalization` (krok 5) |
| po Create lista bez nowego wiersza, w adresie `Create` | `ModelState` nieważny (np. pusta data); sprawdzamy komunikaty pod polami |
| `/ProductsAutoGen` → 404 | folder ma inną nazwę niż w linku (`asp-page="/ProductsAutoGen/Index"`) |

Mamy kompletną aplikację Razor Pages. Każda strona CRUD to osobna para plików z własną logiką: 5 klas na jeden produkt. W większych aplikacjach częściej spotyka się **MVC**: jedna klasa, kontroler, obsługuje wszystkie operacje na produkcie, a widoki tylko wyświetlają. Tę samą funkcjonalność budujemy teraz w MVC, zaczynając od kalkulatora, żeby porównać 1:1.

---

## 7. P10: MVC – kontroler, akcja, widok (dwa kalkulatory)

**Cel:** projekt `P10ShopWebAppMVC.Client` na porcie 7202 z dwoma kalkulatorami; rozumiemy Model–View–Controller na znanym przykładzie: najpierw `Calculator` (`IFormCollection`, `ViewBag`), potem `NewCalculator` (silnie typowany model, `asp-for`).

### Teoria

W Razor Pages adres wskazywał **plik**, a strona miała własną logikę. W MVC adres wskazuje **kontroler i akcję** (metodę). Kontroler decyduje, co zrobić i który **widok** zwrócić, a dane przekazuje w **modelu**. Jedna klasa, np. `ProductsController`, obsługuje wszystkie operacje na produktach.

| | Razor Pages (P09) | MVC (P10) |
|---|---|---|
| `/Calculator` trafia do | `Pages/Calculator.cshtml` | `CalculatorController.Index()` |
| Logika | `PageModel` (`OnGet`, `OnPost`) | akcje kontrolera (`[HttpGet]`, `[HttpPost]`) |
| Widok | ten sam plik, co strona | `Views/Calculator/Index.cshtml`, wybierany przez kontroler |
| Dane dla widoku | właściwości `PageModel` (`@Model.Result`) | `ViewBag`/`ViewData` albo model przekazany do `View(model)` |
| Rejestracja | `AddRazorPages()` / `MapRazorPages()` | `AddControllersWithViews()` / `MapControllerRoute(...)` |

- Trasa `{controller=Home}/{action=Index}/{id?}` w `Program.cs`: pierwszy segment adresu to kontroler, drugi akcja, trzeci opcjonalne id. `/Products/Edit/5` → `ProductsController.Edit(5)`; `/Calculator/Add` → `CalculatorController.Add()`; `/` → `Home/Index`.
- Kontroler wybiera widok: `View()` = `Views/<Kontroler>/<Akcja>.cshtml`, `View("Index")` = inny widok o podanej nazwie.
- `IFormCollection form` to odpowiednik `Request.Form` z Razor Pages (w kontrolerze można też pisać `Request.Form`).
- `ViewBag` (obiekt `dynamic`) i `ViewData` (to samo jako słownik, `ViewData["MyResult"]`) przenoszą proste dane do widoku bez kontroli typów: literówka wychodzi dopiero w działającej aplikacji. Wybór między nimi to preferencja; dane złożone przekazujemy modelem.
- **Silnie typowany model**: zwykła klasa C#, `@model OperationModel` w widoku, `View(operationModel)` w kontrolerze. Kompilator i IntelliSense pilnują nazw, a Tag Helpery generują formularz z właściwości. Akcja POST przyjmuje parametr `OperationModel operation` i model binding wypełnia go sam.
- `asp-for="Number1"` generuje `id`, `name`, `value`, `type="number"` (z typu `int`) i atrybuty `data-val-*`.

### Kroki

1. **Nowy projekt.** Prawy przycisk na solucję → **Add → New Project…** → **ASP.NET Core Web App (Model‑View‑Controller)** → `P10ShopWebAppMVC.Client`, .NET 10.0, Authentication **None**, HTTPS → **Create**. Z terminala:

```powershell
dotnet new mvc -n P10ShopWebAppMVC.Client -o P10ShopWebAppMVC.Client -f net10.0
dotnet sln 32Dzien16.slnx add P10ShopWebAppMVC.Client/P10ShopWebAppMVC.Client.csproj
```

2. **Porty.** W `P10ShopWebAppMVC.Client/Properties/launchSettings.json` ustawiamy porty **7202/5202** (plik jak w P09, zmieniamy tylko liczby). **Set as Startup Project** → F5.

3. **Przegląd struktury** (z szablonu):

| Element | Co to jest |
|---|---|
| `Controllers/HomeController.cs` | akcja `Index()` zwraca `View()`: widok o nazwie akcji w folderze o nazwie kontrolera: `Views/Home/Index.cshtml`; `Error()` z `ErrorViewModel` |
| `Views/Home/`, `Views/Shared/_Layout.cshtml` | widoki pogrupowane według kontrolera; `Shared` to layout i `_ValidationScriptsPartial`, tak jak w P09 |
| `Views/_ViewImports.cshtml` | `@using P10ShopWebAppMVC.Client.Models` (dlatego w widokach piszemy `@model OperationModel` bez namespace) i Tag Helpery |
| `Models/ErrorViewModel.cs` | jedyny model z szablonu; tu dodamy `OperationModel`, a scaffold dołoży `Product` |
| `Program.cs` | `AddControllersWithViews()` zamiast `AddRazorPages()`; trasa `{controller=Home}/{action=Index}/{id?}` |

4. **`Calculator` (7.2).** Prawy przycisk na `Controllers` → **Add → Controller…** → **MVC Controller – Empty** → `CalculatorController.cs` (kod niżej).

5. Prawy przycisk na `Views` → **Add → New Folder** `Calculator` → prawy przycisk na nim → **Add → View…** → **Razor View – Empty** → `Index.cshtml` (kod niżej).

6. **Linki.** W `Views/Shared/_Layout.cshtml` pod **Privacy** dopisujemy od razu trzy linki (jak w P09).

7. F5 → **Calculator** → `2` i `3` → **Add** → `Result: 5`. Potem puste pierwsze pole → **Add** → `Podaj dwie liczby całkowite.`.

8. 🧪 **Eksperymenty (5 min, opcjonalne; po każdym przywracamy kod):**
   - (a) **Słabość `ViewBag`:** w widoku zmieniamy `@ViewBag.MyResult` na `@ViewBag.MyResul`. Kompiluje się, a wynik po prostu znika. Brak IntelliSense i kontroli typów.
   - (b) **`int.Parse`:** w kontrolerze zamieniamy `int.TryParse(form["number1"], out int number1)` na `int number1 = int.Parse(form["number1"]);` i wysyłamy puste pole. Pojawia się strona błędu z `FormatException` (w Development z pełnym stosem, dzięki `UseDeveloperExceptionPage`, który jest włączony domyślnie). Ta sama lekcja, co w P09: nigdy nie ufamy danym od użytkownika.
   - (c) **Nazwa widoku:** w `return View("Index")` usuwamy `"Index"`. Wynik: `InvalidOperationException: The view 'Add' was not found`, bo MVC szuka `Views/Calculator/Add.cshtml`.

9. **`NewCalculator` (7.3).** `ViewBag` działa „na słowo honoru”. Profesjonalnie przekazuje się do widoku silnie typowany model. Prawy przycisk na `Models` → **Add → Class…** → `OperationModel.cs` (kod niżej).

10. `Controllers` → **Add → Controller… → MVC Controller – Empty** → `NewCalculatorController.cs` (kod niżej).

11. Folder `Views/NewCalculator` → **Add → View… → Razor View – Empty** → `Index.cshtml` (kod niżej).

12. F5 → **New calculator**: pola mają `8` i `10` → **Add** → `Result: 18`; wpisujemy `7` i `3` → `Result: 10`. W źródle strony sprawdzamy, co wygenerował `asp-for`.

### Kod

```json
// P10ShopWebAppMVC.Client/Properties/launchSettings.json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5202",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7202;http://localhost:5202",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

```csharp
// P10ShopWebAppMVC.Client/Controllers/CalculatorController.cs
using Microsoft.AspNetCore.Mvc;

namespace P10ShopWebAppMVC.Client.Controllers;

/// <summary>
/// Adres /Calculator → ten kontroler. W MVC adres wskazuje kontroler i akcję (metodę),
/// a kontroler decyduje, który widok zwrócić i jakie dane mu przekazać.
/// Wersja "ręczna": IFormCollection (odpowiednik Request.Form z Razor Pages) i ViewBag.
/// </summary>
public class CalculatorController : Controller
{
    // GET: /Calculator – pusty formularz
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Calculator/Add – formularz wysłany przyciskiem
    [HttpPost]
    public IActionResult Add(IFormCollection form)
    {
        // TryParse zamiast int.Parse: puste pole nie wywraca aplikacji (FormatException)
        bool ok1 = int.TryParse(form["number1"], out int number1);
        bool ok2 = int.TryParse(form["number2"], out int number2);

        // ViewBag: dynamiczny obiekt na proste dane dla widoku (ViewData["X"] robi to samo jako słownik).
        // Wada: literówka w widoku wychodzi dopiero w działającej aplikacji – patrz NewCalculator z modelem.
        ViewBag.Number1 = number1;
        ViewBag.Number2 = number2;

        if (!ok1 || !ok2)
        {
            ViewBag.Error = "Podaj dwie liczby całkowite.";
            return View("Index");
        }

        ViewBag.MyResult = number1 + number2;

        // Akcja nazywa się Add, ale zwracamy widok Index – bez nazwy MVC szukałby Views/Calculator/Add.cshtml
        return View("Index");
    }
}
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Calculator/Index.cshtml *@
@{
    ViewData["Title"] = "Calculator";
}

<h1>Calculator (ViewBag)</h1>
<p>Akcja <code>Add</code> czyta <code>IFormCollection</code> i przekazuje wynik przez <code>ViewBag</code>.</p>

@* action="/Calculator/Add": kontroler Calculator, akcja Add – adres wynika z trasy w Program.cs *@
<form method="post" action="/Calculator/Add">
    <input type="text" name="number1" value="@ViewBag.Number1" />
    <input type="text" name="number2" value="@ViewBag.Number2" />
    <input type="submit" value="Add" class="btn btn-primary" />
</form>

<p class="mt-3">Result: @ViewBag.MyResult</p>
<p class="text-danger">@ViewBag.Error</p>
```

🟩 **Co dopisujemy** w `P10ShopWebAppMVC.Client/Views/Shared/_Layout.cshtml`:

```diff
⋯
                         <li class="nav-item">
                             <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                         </li>
+                        @* asp-controller + asp-action: Tag Helper buduje href z trasy {controller}/{action} *@
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-controller="Calculator" asp-action="Index">Calculator</a>
+                        </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-controller="NewCalculator" asp-action="Index">New calculator</a>
+                        </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-controller="Products" asp-action="Index">Products</a>
+                        </li>
                     </ul>
⋯
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Shared/_Layout.cshtml *@
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - P10ShopWebAppMVC.Client</title>
    <script type="importmap"></script>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/P10ShopWebAppMVC.Client.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">P10ShopWebAppMVC.Client</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                        @* asp-controller + asp-action: Tag Helper buduje href z trasy {controller}/{action} *@
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Calculator" asp-action="Index">Calculator</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="NewCalculator" asp-action="Index">New calculator</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Products" asp-action="Index">Products</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2026 - P10ShopWebAppMVC.Client - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

💡 **Wyjaśnienie (`Calculator`):**

- `[HttpGet] Index()` wyświetla pusty formularz, `[HttpPost] Add(IFormCollection form)` go obsługuje.
- `action="/Calculator/Add"`: kontroler `Calculator`, akcja `Add`. Adres wynika z trasy w `Program.cs`. Nazwy pól `number1` są małą literą: `form["number1"]` jest wrażliwe na wielkość liter, więc musi się zgadzać z `name`.
- `return View("Index")`: akcja nazywa się `Add`, ale zwracamy widok **Index**, bo akcja `Add` nie ma własnego widoku (eksperyment c).
- `ViewBag.MyResult`, `ViewBag.Error`: proste dane dla widoku bez kontroli typów (eksperyment a).

```csharp
// P10ShopWebAppMVC.Client/Models/OperationModel.cs
namespace P10ShopWebAppMVC.Client.Models;

/// <summary>
/// Silnie typowany model dla widoku NewCalculator – zamiast ViewBag.
/// Kompilator i IntelliSense pilnują nazw właściwości.
/// </summary>
public class OperationModel
{
    public int Number1 { get; set; }

    public int Number2 { get; set; }

    public int Result { get; set; }
}
```

```csharp
// P10ShopWebAppMVC.Client/Controllers/NewCalculatorController.cs
using Microsoft.AspNetCore.Mvc;
using P10ShopWebAppMVC.Client.Models;

namespace P10ShopWebAppMVC.Client.Controllers;

/// <summary>
/// Ten sam kalkulator z silnie typowanym modelem (OperationModel) i Tag Helperami asp-for.
/// Dwie akcje Index rozróżniają atrybuty [HttpGet] i [HttpPost]; formularz bez action wysyła POST pod ten sam adres.
/// </summary>
public class NewCalculatorController : Controller
{
    // GET: /NewCalculator – model z wartościami startowymi
    [HttpGet]
    public IActionResult Index()
    {
        var operationModel = new OperationModel
        {
            Number1 = 8,
            Number2 = 10
        };

        return View(operationModel);
    }

    // POST: /NewCalculator – model binding sam tworzy OperationModel z pól Number1 i Number2
    [HttpPost]
    public IActionResult Index(OperationModel operation)
    {
        operation.Result = operation.Number1 + operation.Number2;
        return View(operation);
    }
}
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/NewCalculator/Index.cshtml *@
@model OperationModel
@{
    ViewData["Title"] = "New calculator";
}

<h1>New calculator (model + asp-for)</h1>
<p>Widok zna typ modelu (<code>@@model OperationModel</code>), a <code>asp-for</code> generuje <code>id</code>, <code>name</code>, <code>value</code> i <code>type="number"</code> z właściwości.</p>

<form method="post">
    <input asp-for="Number1" />
    <input asp-for="Number2" />
    <input type="submit" value="Add" class="btn btn-primary" />
</form>

<p class="mt-3">Result: @Model.Result</p>
```

💡 **Wyjaśnienie (`NewCalculator`):**

- `@model OperationModel` na górze widoku określa typ `Model`. Po `@Model.` działa IntelliSense; literówka to błąd kompilacji widoku.
- GET: kontroler tworzy obiekt z wartościami startowymi i przekazuje go przez `View(operationModel)`.
- POST: akcja przyjmuje parametr `OperationModel operation`. **Model binding** sam tworzy obiekt i wypełnia go z pól `Number1`, `Number2` (wielkość liter nie ma znaczenia). Bez `IFormCollection` i parsowania. Ten sam mechanizm, co `[BindProperty]` w Razor Pages, tylko na parametrze akcji.
- Dwie akcje `Index` rozróżniają `[HttpGet]` i `[HttpPost]`; formularz bez `action` wysyła POST pod ten sam adres.
- W źródle strony `asp-for="Number1"` daje `<input type="number" data-val="true" data-val-required="The Number1 field is required." id="Number1" name="Number1" value="8">`. `type="number"` wynika z typu `int`, `name` z nazwy właściwości (literówka niemożliwa), `data-val-*` służy walidacji w przeglądarce. W wygenerowanym CRUD `asp-for` będzie wszędzie.

### ✅ Punkt kontrolny

- `/Calculator`: `2`/`3` → `Result: 5`; puste pole → `Podaj dwie liczby całkowite.`, bez strony błędu.
- `/NewCalculator`: w źródle `<input type="number" data-val="true" data-val-required="…" id="Number1" name="Number1" value="8">`; `Result: 18`, potem `10`.
- Trzy linki w menu.
- Różnica `ViewBag` vs model i `name` ręczny vs `asp-for` jest jasna.

### Dlaczego tak

- `[HttpGet] Index()` i `[HttpPost] Add(...)` w `Calculator`: `action="/Calculator/Add"` wskazuje kontroler i akcję; `return View("Index")`, bo akcja `Add` nie ma własnego widoku.
- 🔧 `TryParse` + `ViewBag.Error`: w edycjach 26 i 31 (kod) `int.Parse` wywracał aplikację przy pustym polu, a widok był pusty, bez tytułu. Teraz jest `TryParse`, komunikat, tytuł, komentarze „dlaczego” i file‑scoped namespace.
- Dwie akcje `Index` w `NewCalculator` rozróżnione `[HttpGet]`/`[HttpPost]`; formularz bez `action` wysyła POST pod ten sam adres.
- 🔧 `asp-for` zamiast ręcznych `name`/`value`: nazwa pola bierze się z właściwości (literówka = błąd kompilacji), `type="number"` i walidacja w przeglądarce są za darmo. W edycji 26 (kod) było ręczne `name="number1" value="@Model.Number1"`, a scenariusz 26 przechodził na `asp-for` jako poprawkę. Teraz `asp-for` jest od razu, a wariant ręczny to po prostu to, co Tag Helper generuje.
- `@model OperationModel` bez namespace, bo `_ViewImports.cshtml` ma `@using P10ShopWebAppMVC.Client.Models`.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| `The view 'Index' was not found` | widok w złym folderze: ma być `Views/Calculator/Index.cshtml` (nazwa folderu = kontroler bez `Controller`) |
| `Result:` puste w `Calculator` | nazwy `name="number1"` w widoku i `form["number1"]` w kontrolerze muszą być identyczne (wielkość liter) |
| `/NewCalculator` → `Result: 0` po POST | `name` pól nie pasuje do właściwości modelu (używamy `asp-for`) |
| `405 Method Not Allowed` | formularz POST wysłany na akcję tylko `[HttpGet]` |

Znamy mechanikę MVC. Teraz to samo, co w P09: podłączamy bazę i generujemy CRUD. Tym razem wszystko z **terminala**, bo w pracy często nie ma kreatora (VS Code, Linux, CI).

---

## 8. P10: scaffold bazy i kontroler CRUD z CLI

**Cel:** P10 ma `ShopContext` z bazy (te same kroki Database First co w P09), `ProductsController` i `Views/Products/*` z generatora `controller`; CRUD działa na `/Products`; rozumiemy w kontrolerze `[Bind]`, `[ValidateAntiForgeryToken]`, `ActionName` i PRG.

### Teoria

- Scaffold bazy jest **identyczny** jak w P09 (te same pakiety, connection string, polecenie, `AddDbContext`): EF Core i DI nie zależą od tego, czy robimy Razor Pages, czy MVC.
- `dotnet aspnet-codegenerator controller` tworzy jedną klasę z parami akcji GET/POST (`Create`, `Edit`, `Delete`) i pięć widoków.
- **`[Bind("Id,Title,…")]`**: lista pól, które wolno bindować. Ochrona przed **overpostingiem** (nadmiarowym przesłaniem pól): ktoś dopisuje do formularza pole, którego nie przewidzieliśmy (np. `IsAdmin=true`), a binder by je wypełnił.
- **`[ValidateAntiForgeryToken]`**: sprawdzenie tokena z sekcji 2. Razor Pages robi to automatycznie, **MVC wymaga atrybutu** na każdej akcji POST.
- **`[HttpPost, ActionName("Delete")] DeleteConfirmed(int id)`**: dwie metody `Delete(int)` o tej samej sygnaturze są w C# niedozwolone, więc POST ma inną nazwę, a `ActionName` zachowuje adres `/Products/Delete/5`.
- **`RedirectToAction(nameof(Index))`**: PRG, jak `RedirectToPage` w P09. `nameof` zamiast stringa: zmiana nazwy akcji to błąd kompilacji, nie 404.

### Kroki

1. **Pakiety i `.csproj` (8.1).** Terminal w katalogu P10: te same cztery `dotnet add package` co w sekcji 4 i ta sama właściwość `NuGetAuditLevel`. Można też skopiować linię `NuGetAuditLevel` i cały `<ItemGroup>` z `P09ShopWebApp.Client.csproj`.

2. **`appsettings.Development.json`.** Szablon MVC ma tylko sekcję `Logging` (bez `DetailedErrors`); wklejamy **ten sam plik, co w P09**.

3. **Scaffold bazy (8.2).** Terminal w katalogu P10; to samo polecenie, co w sekcji 4:

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
```

   Powstają `Models/Product.cs` i `Data/ShopContext.cs`, identyczne jak w P09 poza namespace (`P10ShopWebAppMVC.Client.Models` / `.Data`). Nagłówek `<auto-generated>` jest w repozytorium dopisany ręcznie, jak w P09.

4. **`Program.cs`.** Zastępujemy `Program.cs`. Rejestracja `ShopContext` i kultura z sekcji 6.4 wchodzą od razu, bo wiemy już, po co są.

5. **Generator kontrolera CRUD (8.3)** (terminal P10):

```powershell
dotnet build
dotnet aspnet-codegenerator controller -name ProductsController -m Product -dc ShopContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

   Wyjście: `Added Controller : \Controllers\ProductsController.cs`, `Added View : \Views\Products\Create.cshtml`, `… Delete`, `… Details`, `… Edit`, `… Index` (`RunTime 00:00:13`).

| Fragment | Znaczenie |
|---|---|
| `controller` | generator kontrolera MVC (w P09 był `razorpage`) |
| `-name ProductsController` | nazwa klasy → adres `/Products` |
| `-m Product` | model (encja) |
| `-dc ShopContext` | DbContext dla kontrolera |
| `--relativeFolderPath Controllers` | gdzie zapisać kontroler |
| `--useDefaultLayout` | widoki z `_Layout.cshtml` |
| `--referenceScriptLibraries` | `_ValidationScriptsPartial` w `Create`/`Edit` (walidacja w przeglądarce) |

   Alternatywa w Visual Studio: prawy przycisk na `Controllers` → **Add → New Scaffolded Item… → MVC Controller with views, using Entity Framework** → Model `Product`, DbContext `ShopContext`, nazwa `ProductsController` → **Add**.

6. **Przegląd (8.4).** Porównujemy wygenerowany kontroler i widoki z cytowanymi niżej (nie przepisujemy) i czytamy komentarze.

7. **Test.** Link **Products** jest już w menu (sekcja 7). F5 → **Products** → 100 wierszy → **Create New** → dane testowe z tabeli (cena `249.99`) → **Create** → **Edit** (`199.99`) → **Details** → **Delete**. W konsoli `INSERT`/`UPDATE`/`DELETE`. Po usunięciu wpisujemy `/Products/Details/101` → `404`.

### Kod

🟩 **Co dopisujemy** w `P10ShopWebAppMVC.Client/P10ShopWebAppMVC.Client.csproj` (identycznie jak w P09):

```diff
   <PropertyGroup>
     <TargetFramework>net10.0</TargetFramework>
     <Nullable>enable</Nullable>
     <ImplicitUsings>enable</ImplicitUsings>
+    <!-- Audyt NuGet: pakiet generatora kodu (CodeGeneration.Design) ciągnie NuGet.Packaging 6.12.1 z ostrzeżeniem
+         NU1901 o NISKIEJ wadze. Generator działa tylko w czasie projektowania, więc pokazujemy ostrzeżenia od "moderate". -->
+    <NuGetAuditLevel>moderate</NuGetAuditLevel>
   </PropertyGroup>
 
+  <ItemGroup>
+    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.12">
+      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
+      <PrivateAssets>all</PrivateAssets>
+    </PackageReference>
+    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.12" />
+    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.12">
+      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
+      <PrivateAssets>all</PrivateAssets>
+    </PackageReference>
+    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="10.0.2" />
+  </ItemGroup>
+
 </Project>
```

```xml
<!-- P10ShopWebAppMVC.Client/P10ShopWebAppMVC.Client.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <!-- Audyt NuGet: pakiet generatora kodu (CodeGeneration.Design) ciągnie NuGet.Packaging 6.12.1 z ostrzeżeniem
         NU1901 o NISKIEJ wadze. Generator działa tylko w czasie projektowania, więc pokazujemy ostrzeżenia od "moderate". -->
    <NuGetAuditLevel>moderate</NuGetAuditLevel>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.12">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.12">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="10.0.2" />
  </ItemGroup>

</Project>
```

🟩 **Co dopisujemy** w `P10ShopWebAppMVC.Client/appsettings.Development.json`:

```diff
 {
+  "ConnectionStrings": {
+    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MySuperShop;Trusted_Connection=True;TrustServerCertificate=True"
+  },
+  "DetailedErrors": true,
   "Logging": {
     "LogLevel": {
       "Default": "Information",
-      "Microsoft.AspNetCore": "Warning"
+      "Microsoft.AspNetCore": "Warning",
+      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
     }
   }
 }
```

```json
// P10ShopWebAppMVC.Client/appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MySuperShop;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

```csharp
// P10ShopWebAppMVC.Client/Models/Product.cs  (wygenerowane przez dotnet ef dbcontext scaffold – nie przepisujemy, porównujemy; nagłówek <auto-generated> dopisany ręcznie)
// <auto-generated>
// Plik wygenerowany z istniejącej bazy MySuperShop (Database First) poleceniem:
//   dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
// Ponowne uruchomienie z --force nadpisze ten plik – własny kod dopisuj w osobnym pliku partial.
// </auto-generated>
using System;
using System.Collections.Generic;

namespace P10ShopWebAppMVC.Client.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; }
}
```

```csharp
// P10ShopWebAppMVC.Client/Data/ShopContext.cs  (wygenerowane przez dotnet ef dbcontext scaffold – nie przepisujemy, porównujemy; nagłówek <auto-generated> dopisany ręcznie)
// <auto-generated>
// Plik wygenerowany z istniejącej bazy MySuperShop (Database First) poleceniem:
//   dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ShopContext --no-onconfiguring --force
// --no-onconfiguring: brak metody OnConfiguring – provider i connection string podaje Program.cs (AddDbContext),
// a kontekst dostaje je przez konstruktor z DbContextOptions<ShopContext>.
// </auto-generated>
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using P10ShopWebAppMVC.Client.Models;

namespace P10ShopWebAppMVC.Client.Data;

public partial class ShopContext : DbContext
{
    public ShopContext()
    {
    }

    public ShopContext(DbContextOptions<ShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Barcode).HasMaxLength(12);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

🟩 **Co dopisujemy** w `P10ShopWebAppMVC.Client/Program.cs` (względem szablonu):

```diff
+using System.Globalization;
+using Microsoft.AspNetCore.Localization;
+using Microsoft.EntityFrameworkCore;
+using P10ShopWebAppMVC.Client.Data;
+
 var builder = WebApplication.CreateBuilder(args);
 
 // Add services to the container.
 builder.Services.AddControllersWithViews();
 
+// Database First: ShopContext wygenerowany z bazy – dokładnie ta sama rejestracja co w P09 (Razor Pages).
+// EF Core i DI działają tak samo niezależnie od tego, czy robimy Razor Pages, czy MVC.
+builder.Services.AddDbContext<ShopContext>(options =>
+    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
+
 var app = builder.Build();
⋯
 app.UseHttpsRedirection();
+
+// Jedna kultura dla całej aplikacji (kropka jako separator dziesiętny) – patrz P09/Program.cs
+var appCulture = new CultureInfo("en-US");
+app.UseRequestLocalization(new RequestLocalizationOptions
+{
+    DefaultRequestCulture = new RequestCulture(appCulture),
+    SupportedCultures = new List<CultureInfo> { appCulture },
+    SupportedUICultures = new List<CultureInfo> { appCulture }
+});
+
 app.UseRouting();
 
 app.UseAuthorization();
 
 app.MapStaticAssets();
 
+// Trasa domyślna: /Products/Edit/5 → ProductsController.Edit(5)
 app.MapControllerRoute(
     name: "default",
     pattern: "{controller=Home}/{action=Index}/{id?}")
     .WithStaticAssets();
 
-
 app.Run();
```

```csharp
// P10ShopWebAppMVC.Client/Program.cs
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
```

💡 To dokładnie ten sam kod, co w P09. EF Core, DI i kultura działają tak samo niezależnie od tego, czy robimy Razor Pages, czy MVC. Różni się tylko para `AddControllersWithViews`/`MapControllerRoute`.

```csharp
// P10ShopWebAppMVC.Client/Controllers/ProductsController.cs  (wygenerowane przez dotnet aspnet-codegenerator – nie przepisujemy, porównujemy)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P10ShopWebAppMVC.Client.Data;
using P10ShopWebAppMVC.Client.Models;

namespace P10ShopWebAppMVC.Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
```

💡 **Wyjaśnienie (kontroler, po kolei):**

- **Konstruktor z `ShopContext`**: DI, jak w `PageModel`. Adresy w komentarzach (`// GET: Products/Details/5`) wynikają z trasy.
- **Pary GET/POST** dla `Create`, `Edit`, `Delete`: GET pokazuje formularz, POST zapisuje. W P09 były to `OnGet`/`OnPost` w pięciu plikach; tu wszystko jest w jednej klasie.
- **`[Bind("Id,Title,Description,Barcode,Price,ReleaseDate")]`**: generator wypisuje wszystkie właściwości. W prawdziwym kodzie usuwa się z listy to, czego użytkownik nie może zmieniać (np. `Id` przy `Create`).
- **`[ValidateAntiForgeryToken]`** na każdej akcji POST.
- **`ModelState.IsValid`**: binding i walidacja. Przy błędzie `return View(product)` – formularz z komunikatami i wpisanymi wartościami.
- **`_context.Update(product)`** w `Edit`: oznacza cały obiekt jako zmieniony (`UPDATE` wszystkich kolumn), jak `Attach(...).State = Modified` w P09. W API z dnia 14 robiliśmy to przez pobranie i przepisanie pól – mniejszy `UPDATE`, czysty `404`. `DbUpdateConcurrencyException` → `404`, gdy ktoś usunął rekord.
- **`[HttpPost, ActionName("Delete")] DeleteConfirmed(int id)`** i **`RedirectToAction(nameof(Index))`** – patrz teoria.
- `FindAsync(id)` (po kluczu, może wziąć obiekt z pamięci kontekstu, jeśli już pobrany) vs `FirstOrDefaultAsync(m => m.Id == id)` (zawsze zapytanie): generator używa obu; dla jednego rekordu po `Id` oba są poprawne.

Widoki (wygenerowane; różnią się od P09 tylko `@model` zamiast `@page` + `Model.Product` oraz `asp-action` zamiast `asp-page`):

```cshtml
@* P10ShopWebAppMVC.Client/Views/Products/Index.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@model IEnumerable<P10ShopWebAppMVC.Client.Models.Product>

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-action="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Title)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Description)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Barcode)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Price)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.ReleaseDate)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Title)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Description)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Barcode)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Price)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ReleaseDate)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Products/Create.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@model P10ShopWebAppMVC.Client.Models.Product

@{
    ViewData["Title"] = "Create";
}

<h1>Create</h1>

<h4>Product</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Description" class="control-label"></label>
                <input asp-for="Description" class="form-control" />
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Barcode" class="control-label"></label>
                <input asp-for="Barcode" class="form-control" />
                <span asp-validation-for="Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Price" class="control-label"></label>
                <input asp-for="Price" class="form-control" />
                <span asp-validation-for="Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="ReleaseDate" class="control-label"></label>
                <input asp-for="ReleaseDate" class="form-control" />
                <span asp-validation-for="ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Products/Edit.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@model P10ShopWebAppMVC.Client.Models.Product

@{
    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

<h4>Product</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Description" class="control-label"></label>
                <input asp-for="Description" class="form-control" />
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Barcode" class="control-label"></label>
                <input asp-for="Barcode" class="form-control" />
                <span asp-validation-for="Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Price" class="control-label"></label>
                <input asp-for="Price" class="form-control" />
                <span asp-validation-for="Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="ReleaseDate" class="control-label"></label>
                <input asp-for="ReleaseDate" class="form-control" />
                <span asp-validation-for="ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Products/Details.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@model P10ShopWebAppMVC.Client.Models.Product

@{
    ViewData["Title"] = "Details";
}

<h1>Details</h1>

<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Title)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Title)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Description)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Description)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Barcode)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Barcode)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Price)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Price)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.ReleaseDate)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.ReleaseDate)
        </dd>
    </dl>
</div>
<div>
    <a asp-action="Edit" asp-route-id="@Model?.Id">Edit</a> |
    <a asp-action="Index">Back to List</a>
</div>
```

```cshtml
@* P10ShopWebAppMVC.Client/Views/Products/Delete.cshtml  (wygenerowane – nie przepisujemy, porównujemy) *@
@model P10ShopWebAppMVC.Client.Models.Product

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Title)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Title)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Description)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Description)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Barcode)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Barcode)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Price)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Price)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.ReleaseDate)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.ReleaseDate)
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-action="Index">Back to List</a>
    </form>
</div>
```

💡 **Wyjaśnienie (widoki):** `@model IEnumerable<P10ShopWebAppMVC.Client.Models.Product>` w `Index` ma pełną nazwę, bo generator nie polega na `_ViewImports`. `<form asp-action="Create">` zamiast `method="post" action="…"`: Tag Helper formularza domyślnie robi POST i dodaje token. `asp-route-id="@item.Id"` daje `/Products/Edit/5` (segment `{id?}` trasy), a nie `?id=5` jak w Razor Pages. W `Details` jest `@Model?.Id` z `?.`, bo model widoku może być `null` w analizie nullable.

### ✅ Punkt kontrolny

- `/Products`: 100 wierszy; po Create 101; `Details/101` → `249.99`; po Edit `199.99`; po Delete 100 i `Details/101` → `404`.
- Konsola: `INSERT`, `UPDATE`, `DELETE`.
- W kontrolerze łatwo wskazać: `[Bind(...)]`, `[ValidateAntiForgeryToken]`, `[HttpPost, ActionName("Delete")]`, `RedirectToAction(nameof(Index))`, `DbUpdateConcurrencyException`.
- P09 i P10 mają kompletny CRUD; obie pracują **bezpośrednio na bazie**.

### Dlaczego tak

- Ten sam `Program.cs` co w P09 poza parą `AddControllersWithViews`/`MapControllerRoute`: EF Core i DI są niezależne od technologii stron.
- 🔧 W edycji 26 był kreator VS w P09 i CLI w P10, ale bez omówienia `FindAsync` vs `FirstOrDefaultAsync` i bez testu `404` po usunięciu; w 31 podobnie. Teraz oba generatory działają z CLI (powtarzalne, bez VS), a kreatory VS są alternatywą.

### Częste problemy

| Objaw | Rozwiązanie |
|---|---|
| generator: `To scaffold controllers and views using models, install Entity Framework core packages` | brak pakietów EF w projekcie (krok 1) albo brak `AddDbContext` (krok 4) |
| `Unable to resolve service for type 'ShopContext'` | brak `AddDbContext` w `Program.cs` |
| widoki nie mają walidacji w przeglądarce | brak `--referenceScriptLibraries` (sekcja `@section Scripts` w `Create`/`Edit`) |
| `The value '249.99' is not valid for Price.` | brak bloku `UseRequestLocalization` (krok 4) |
| czy `/Products/Delete/5` przez GET usuwa? | nie: GET pokazuje potwierdzenie, usuwa dopiero POST `DeleteConfirmed` |

⚠️ Tu pojawia się problem architektoniczny. Mamy **trzy** projekty, które łączą się z bazą: API, P09 i P10. Każdy ma własną kopię `Product` i własny connection string. Zmiana tabeli oznacza trzy miejsca do poprawy. Reguła „nie usuwamy droższych niż 1000 zł” jest tylko w API; w P09 i P10 można usunąć wszystko. Każdy klient na bazie to kolejne miejsce z dostępem do bazy i kolejna kopia logiki. Mamy API, z którego korzysta WPF, więc aplikacja webowa też powinna z niego korzystać. Stąd **P11: MVC, które nie wie nic o bazie**.

---

## 9. P11: MVC przez API i wspólny `ProductHttpService`

**Cel:** `P11ShopWebAppMVCApi.Client` (port 7203) pokazuje i zmienia produkty **przez API (P05)**, używając tego samego `ProductHttpService` z `P06Shop.Shared`, co WPF; komunikaty z API (reguła 1000 zł, brak połączenia, walidacja) widać na stronach; P11 nie ma ani jednego śladu EF.

```text
P11 (MVC) ──HTTP/JSON──► P05 (API) ──EF Core──► baza MySuperShop
P03 (WPF) ──HTTP/JSON──┘
        └── oba używają ProductHttpService z P06Shop.Shared (bez zmian od dnia 14)
```

### Teoria

- `ProductHttpService` w dniach 13 i 14 od razu trafił do biblioteki wspólnej, a od dnia 14 ma `SendAsync<T>`: brak połączenia → `Fail("Brak połączenia z API: …")`, 404/400 z `ServiceResponse` → zwraca je, ProblemDetails → lista błędów, timeout. Dziś **nic nie przenosimy** i nic w nim nie dopisujemy; tylko rejestrujemy go w P11.
- Plan w pięciu krokach: (1) nowy projekt MVC z referencją do `P06Shop.Shared`, bez pakietów EF; (2) `AppSettings` z `ApiBaseUrl` jak w P03; (3) `AddHttpClient<IProductService, ProductHttpService>` w `Program.cs`; (4) kontroler **ręcznie**, o strukturze wygenerowanego z P10, ale na `IProductService`; (5) widoki skopiowane z P10 z podmienionym `@model`.
- `Configure<AppSettings>(GetSection("AppSettings"))` + `IOptions<AppSettings>`: wzorzec **Options**, jak w P03 od dnia 13 (sekcja `AppSettings` ↔ klasa `AppSettings`, `SectionName` jako `const`).
- `AddHttpClient<IProductService, ProductHttpService>((sp, client) => …)`: **typed client** (typowany klient HTTP). `IHttpClientFactory` tworzy `HttpClient` z `BaseAddress` (zarządza połączeniami, nie wyczerpuje gniazd), wstrzykuje go do konstruktora `ProductHttpService` i rejestruje serwis pod interfejsem. Te same linie, co w `App.xaml.cs` P03.
- Kontroler zamiast `ShopContext` dostaje `IProductService`. Każda metoda zwraca `ServiceResponse<T>`. Zasada: sprawdzamy `Success`, do widoku dajemy `Data`, a `Message` pokazujemy użytkownikowi przez `ModelState.AddModelError(string.Empty, …)` (→ `asp-validation-summary="ModelOnly"`) albo `ViewBag`.
- Model widoków to `P06Shop.Shared.Models.Product` z atrybutami `[Required]`, `[MaxLength]`, `[Range]` po polsku: te same komunikaty w przeglądarce (`data-val-*`), w `ModelState` i w API.
- ⚠️ Generator `controller` **nie zadziała** bez `DbContext`: `dotnet aspnet-codegenerator controller -m P06Shop.Shared.Models.Product …` kończy się komunikatem *To scaffold controllers and views using models, install Entity Framework core packages*. Można by tymczasowo dodać EF i scaffold bazy (tak robiła edycja 26), ale wtedy P11 dostaje pakiety i pliki, które zaraz trzeba usuwać. Dlatego kontroler piszemy **ręcznie**: szkielet akcji jest ten sam, co w wygenerowanym `ProductsController` z P10, zmienia się tylko źródło danych. Widoki kopiujemy z P10.

### Kroki

1. **Projekt i referencja (9.1).** **Add → New Project… → ASP.NET Core Web App (Model‑View‑Controller)** → `P11ShopWebAppMVCApi.Client`, .NET 10.0, Authentication **None** → **Create**. Porty w `launchSettings.json` na **7203/5203**. Prawy przycisk na P11 → **Add → Project Reference…** → **P06Shop.Shared** → **OK**. Z terminala (w folderze `32Dzien16`):

```powershell
dotnet new mvc -n P11ShopWebAppMVCApi.Client -o P11ShopWebAppMVCApi.Client -f net10.0
dotnet sln 32Dzien16.slnx add P11ShopWebAppMVCApi.Client/P11ShopWebAppMVCApi.Client.csproj
cd P11ShopWebAppMVCApi.Client
dotnet add reference ../P06Shop.Shared/P06Shop.Shared.csproj
```

   **Zero pakietów EF.** P11 ma znać tylko `Product`, `ServiceResponse<T>` i `IProductService` z biblioteki wspólnej. Nazwa projektu ma `Api` na końcu, żeby od razu było widać, skąd bierze dane. `appsettings.Development.json` zostaje z szablonu (tylko `Logging`): P11 nie ma connection stringa.

2. **`AppSettings` (9.2).** Folder `Configuration` → **Add → Class…** → `AppSettings.cs` (wzór: `P03WeatherForecastWPF.Client/Configuration/AppSettings.cs`, bez `DefaultLanguage`, bo P11 nie ma pogody).

3. **Adres API** w `appsettings.json`. Tu, nie w Development: adres API to ustawienie aplikacji, które na produkcji **zmieniamy**, a nie usuwamy.

4. **`Program.cs` (9.3):** `Configure<AppSettings>` i `AddHttpClient` (kod niżej).

5. **Kontroler (9.4).** `Controllers` → **Add → Controller… → MVC Controller – Empty** → `ProductsController.cs`. Piszemy go ręcznie; obok warto otworzyć wygenerowany kontroler z P10 i porównywać.

6. **Widoki (9.5).** W Eksploratorze plików kopiujemy folder `P10ShopWebAppMVC.Client/Views/Products` do `P11ShopWebAppMVCApi.Client/Views/Products` (5 plików). W Solution Explorer pojawiają się same (gdyby nie: **Show All Files**). Potem **Edit → Find and Replace → Replace in Files** (Ctrl+Shift+H): Find `P10ShopWebAppMVC.Client.Models.Product`, Replace `P06Shop.Shared.Models.Product`, Look in **Current project** (P11), File types `*.cshtml` → **Replace All** (5 plików).

7. W `Index.cshtml` dopisujemy licznik i alert, w `Delete.cshtml` miejsce na komunikat z API. Pozostałe trzy widoki (`Create`, `Edit`, `Details`) różnią się od P10 **tylko** pierwszą linią `@model P06Shop.Shared.Models.Product`. `Create` i `Edit` mają już `asp-validation-summary="ModelOnly"`, więc komunikaty z API pokażą się bez zmian. Pełne pliki są niżej, gdyby coś poszło nie tak przy kopiowaniu.

8. **Link w menu (9.6)** w `Views/Shared/_Layout.cshtml`.

9. `dotnet build 32Dzien16.slnx` → 8 projektów bez ostrzeżeń. Jeśli coś wskazuje na `P10ShopWebAppMVC.Client.Models`, zamiana w widokach nie objęła wszystkich plików.

10. **Uruchomienie API i P11 razem (9.7).** Prawy przycisk na solucję → **Configure Startup Projects… → Multiple startup projects**: `P05Shop.API` → **Start** (profil **https**, port 7127), `P11ShopWebAppMVCApi.Client` → **Start**, reszta **None** → **OK** → F5. Otwierają się dwie karty: Scalar (API) i P11. Przechodzimy dziesięć kroków z danymi testowymi:

| Krok | Co robimy | Co widać |
|---|---|---|
| 1 | P11 → **Products (API)** | `Produktów: 100`, pierwszy `Fantastic Fresh Gloves`; w konsoli **API** `SELECT … FROM [Products]` (P11 nie ma własnej konsoli SQL) |
| 2 | **Create New** → pusty formularz → **Create** | pod polami po polsku: `Nazwa jest wymagana.`, `Opis jest wymagany.`, `Kod kreskowy jest wymagany.`, `Cena musi być w przedziale 0,01–100 000.`; Network: brak żądania (walidacja w przeglądarce) |
| 3 | wyłączamy JavaScript (F12 → Ctrl+Shift+P → „Disable JavaScript”) albo wpisujemy pusty Title i Barcode `5909876543210` (13 znaków), reszta poprawna → **Create** | żądanie POST wraca z tymi samymi komunikatami, tym razem z serwera (`ModelState`; ten sam atrybut po obu stronach); w konsoli API nic |
| 4 | `Monitor 27 cali`, `Monitor IPS 2560x1440`, `590987654321`, `1299.00`, dowolna data → **Create** | `302 → /Products`, `Produktów: 101`, `Monitor 27 cali` na końcu; konsola API: `INSERT` |
| 5 | nowa karta `https://localhost:7127/api/products/101` | JSON z monitorem: `"price": 1299` |
| 6 | **Edit** przy monitorze → `1199.00` → **Save** | `UPDATE` w konsoli API; w tabeli `1199.00` |
| 7 | **Delete** → **Delete** | **zostajemy na stronie Delete**, nad danymi czerwone: `Nie można usunąć produktu droższego niż 1000 zł (cena: 1199,00 zł).`; w konsoli API `SELECT TOP(1)` bez `DELETE` |
| 8 | **Edit** → `999.00` → **Save** → **Delete** → **Delete** | `DELETE FROM [Products]` w konsoli, `Produktów: 100` |
| 9 | `/Products/Details/101` | `404` z tekstem `Nie znaleziono produktu o Id 101.` |
| 10 | breakpoint w `P05Shop.API/Controllers/ProductsController.cs` w `GetProducts`, odświeżenie listy w P11 | debugger zatrzymuje się **w API**: dowód, że P11 idzie przez HTTP |

   💡 Krok 7 to reguła biznesowa z `P05Shop.API/Services/ProductService.cs` z dnia 14. P11 nic o niej nie wie, a działa tak samo jak w WPF. **Logika biznesowa jest w jednym miejscu, w API.** W P09 i P10 ten sam produkt dałoby się usunąć bez pytania.

11. **API wyłączone (9.8).** Zatrzymujemy tylko API (w oknie konsoli API Ctrl+C albo **Debug → Windows → Processes** → Detach; najprościej: **Configure Startup Projects → Single startup project → P11** → F5) i otwieramy **Products (API)**. Widać stronę z czerwonym alertem `Brak połączenia z API: No connection could be made because the target machine actively refused it. (localhost:7127)` i `Produktów: 0`, pustą tabelę, działające menu. Żadnej strony z wyjątkiem.

   💡 `HttpClient` rzucił `HttpRequestException`, `SendAsync<T>` w Shared zamienił go na `Fail("Brak połączenia z API: …")`, a kontroler przekazał komunikat do `ViewBag`. Ta sama klasa ratuje WPF (czerwony komunikat w oknie Sklep). Jedno miejsce, dwie aplikacje; jutro trzecia (Blazor). Na koniec przywracamy **Multiple startup projects** i odświeżamy: lista wraca.

12. 🧪 **Opcjonalnie:** w **Configure Startup Projects** dodajemy `P03WeatherForecastWPF.Client` → **Start**. Dwóch klientów (WPF i przeglądarka) pracuje na jednym API; produkt dodany w przeglądarce widać w oknie **Sklep** po **Odśwież**.

### Kod

```json
// P11ShopWebAppMVCApi.Client/Properties/launchSettings.json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5203",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7203;http://localhost:5203",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/P11ShopWebAppMVCApi.Client.csproj` (dopisuje to `dotnet add reference`):

```diff
     <ImplicitUsings>enable</ImplicitUsings>
   </PropertyGroup>
 
+  <ItemGroup>
+    <ProjectReference Include="..\P06Shop.Shared\P06Shop.Shared.csproj" />
+  </ItemGroup>
+
 </Project>
```

```xml
<!-- P11ShopWebAppMVCApi.Client/P11ShopWebAppMVCApi.Client.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\P06Shop.Shared\P06Shop.Shared.csproj" />
  </ItemGroup>

</Project>
```

```csharp
// P11ShopWebAppMVCApi.Client/Configuration/AppSettings.cs
namespace P11ShopWebAppMVCApi.Client.Configuration;

/// <summary>
/// Klasa odpowiadająca sekcji "AppSettings" w appsettings.json (wzorzec Options) –
/// ta sama konwencja co w kliencie WPF (P03). Zamiast builder.Configuration["AppSettings:ApiBaseUrl"]
/// dostajemy silnie typowany obiekt przez IOptions&lt;AppSettings&gt;.
/// </summary>
public class AppSettings
{
    /// <summary>Nazwa sekcji w pliku konfiguracyjnym.</summary>
    public const string SectionName = "AppSettings";

    /// <summary>
    /// Adres naszego API (P05Shop.API). Musi kończyć się ukośnikiem, bo serwis woła adresy względne (api/products).
    /// Po wdrożeniu API do chmury zmieniamy tylko wartość w pliku, bez kompilacji.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://localhost:7127/";
}
```

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/appsettings.json`:

```diff
 {
+  "AppSettings": {
+    "ApiBaseUrl": "https://localhost:7127/"
+  },
   "Logging": {
     "LogLevel": {
       "Default": "Information",
       "Microsoft.AspNetCore": "Warning"
     }
   },
   "AllowedHosts": "*"
 }
```

```json
// P11ShopWebAppMVCApi.Client/appsettings.json
{
  "AppSettings": {
    "ApiBaseUrl": "https://localhost:7127/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

💡 **Wyjaśnienie (`AppSettings`):**

- Ta sama konwencja, co w P03 od dnia 13: sekcja `AppSettings` ↔ klasa `AppSettings`, `SectionName` jako `const`, wzorzec Options (`IOptions<AppSettings>`).
- `ApiBaseUrl` **kończy się ukośnikiem**, bo `ProductHttpService` woła adresy względne `api/products`; `new Uri(base, "api/products")` bez ukośnika obcięłoby ostatni segment adresu bazowego.
- Po wdrożeniu API do Azure (dzień 15) zmieniamy jedną wartość w pliku (albo w ustawieniach App Service), bez kompilacji.

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/Program.cs` (względem szablonu):

```diff
+using System.Globalization;
+using Microsoft.AspNetCore.Localization;
+using Microsoft.Extensions.Options;
+using P06Shop.Shared.Services;
+using P11ShopWebAppMVCApi.Client.Configuration;
+
 var builder = WebApplication.CreateBuilder(args);
 
 // Add services to the container.
 builder.Services.AddControllersWithViews();
 
+// 1. Sekcja "AppSettings" z appsettings.json → klasa AppSettings (dostępna przez IOptions<AppSettings>)
+builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
+
+// 2. Klient naszego API: ta sama klasa ProductHttpService z P06Shop.Shared, której używa WPF (P03).
+// AddHttpClient<TInterface, TImpl> = typed client: HttpClient tworzy IHttpClientFactory i wstrzykuje go do serwisu.
+// (sp, client) => ... pozwala sięgnąć do kontenera po IOptions<AppSettings> w momencie tworzenia klienta.
+// P11 nie ma DbContext ani connection stringa – o bazie wie tylko API.
+builder.Services.AddHttpClient<IProductService, ProductHttpService>((sp, client) =>
+{
+    AppSettings settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
+    client.BaseAddress = new Uri(settings.ApiBaseUrl);
+    client.Timeout = TimeSpan.FromSeconds(10);
+});
+
 var app = builder.Build();
⋯
 app.UseHttpsRedirection();
+
+// Jedna kultura dla całej aplikacji (kropka jako separator dziesiętny) – patrz P09/Program.cs
+var appCulture = new CultureInfo("en-US");
+app.UseRequestLocalization(new RequestLocalizationOptions
+{
+    DefaultRequestCulture = new RequestCulture(appCulture),
+    SupportedCultures = new List<CultureInfo> { appCulture },
+    SupportedUICultures = new List<CultureInfo> { appCulture }
+});
+
 app.UseRouting();
⋯
     .WithStaticAssets();
 
-
 app.Run();
```

```csharp
// P11ShopWebAppMVCApi.Client/Program.cs
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
```

💡 **Wyjaśnienie (`Program.cs`, linia po linii):**

- `Configure<AppSettings>(GetSection("AppSettings"))`: wiąże sekcję z klasą i rejestruje `IOptions<AppSettings>`. Dokładnie tak, jak w `App.xaml.cs` P03 (`Host.CreateApplicationBuilder` ma ten sam `builder.Services`).
- `AddHttpClient<IProductService, ProductHttpService>((sp, client) => …)`: przeciążenie z `(sp, client)` daje `IServiceProvider`, więc adres bierzemy z `IOptions<AppSettings>` zamiast czytać konfigurację ręcznie. W `P03…/App.xaml.cs`, metoda `ConfigureServices`, są **te same cztery linie**.
- `Timeout = 10 s`: `SendAsync<T>` zamienia `TaskCanceledException` na `Przekroczono czas oczekiwania na odpowiedź API.`
- `using P06Shop.Shared.Services;`: tam leżą `IProductService` i `ProductHttpService`. `using Microsoft.Extensions.Options;` dla `IOptions<T>`.
- Kultura jak w P09/P10, z tego samego powodu (`Price` z kropką).

```csharp
// P11ShopWebAppMVCApi.Client/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using P06Shop.Shared.Models;
using P06Shop.Shared.Services;

namespace P11ShopWebAppMVCApi.Client.Controllers;

/// <summary>
/// CRUD produktów przez NASZE API (P05) – struktura akcji jak w wygenerowanym kontrolerze z P10,
/// ale zamiast ShopContext (baza) wstrzykujemy IProductService (HTTP). Kontroler nie wie, że pod spodem jest HTTP.
/// Każda metoda serwisu zwraca ServiceResponse&lt;T&gt;: sprawdzamy Success, do widoku przekazujemy Data,
/// a Message pokazujemy użytkownikowi (ModelState.AddModelError → asp-validation-summary, albo ViewBag).
/// </summary>
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var response = await _productService.GetProductsAsync();

        if (!response.Success || response.Data is null)
        {
            // API wyłączone / błąd – pokazujemy komunikat na liście zamiast strony z wyjątkiem
            ViewBag.ErrorMessage = response.Message;
            return View(new List<Product>());
        }

        return View(response.Data);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            // API odpowiada 404 z ServiceResponse ("Nie znaleziono produktu o Id ...") – oddajemy 404
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    // [Bind] – lista dozwolonych pól (ochrona przed overpostingiem), jak w kontrolerze wygenerowanym w P10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProductAsync(product);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));   // PRG: po zapisie przekierowanie na listę
            }

            // Błąd z API pokazujemy nad formularzem (<div asp-validation-summary="ModelOnly">)
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProductAsync(product);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(product);
    }

    // GET: Products/Delete/5 – strona z potwierdzeniem (nigdy nie usuwamy przez GET)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // POST: Products/Delete/5
    // Dwie metody Delete(int) miałyby tę samą sygnaturę, więc ta nazywa się DeleteConfirmed,
    // a [ActionName("Delete")] zachowuje adres /Products/Delete/5.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _productService.DeleteProductAsync(id);

        if (response.Success)
        {
            return RedirectToAction(nameof(Index));
        }

        // API odmówiło (reguła biznesowa: cena > 1000 zł) – zostajemy na stronie Delete i pokazujemy powód
        var productResponse = await _productService.GetProductAsync(id);

        if (!productResponse.Success || productResponse.Data is null)
        {
            return NotFound(productResponse.Message);
        }

        ModelState.AddModelError(string.Empty, response.Message);
        return View(productResponse.Data);
    }
}
```

💡 **Wyjaśnienie (kontroler):**

- Zamiast `ShopContext` wstrzykujemy **`IProductService`** (interfejs). Kontroler nie wie, że pod spodem jest HTTP; w teście można podstawić atrapę.
- Każda metoda serwisu zwraca **`ServiceResponse<T>`** (`Success`, `Data`, `Message`): tę samą kopertę, którą API zwraca w JSON i którą `SendAsync<T>` produkuje przy błędach. Nigdy `null`, więc nie ma `response?.`.
- `Index` przy błędzie: `ViewBag.ErrorMessage = response.Message` i **pusta lista** → widok pokazuje alert i `Produktów: 0` zamiast strony z wyjątkiem.
- `Details`/`Edit`/`Delete` GET: API dla nieistniejącego `id` odpowiada `404` z `Fail("Nie znaleziono produktu o Id …")`; `SendAsync` przepuszcza to jako `Success = false`, a my oddajemy `NotFound(response.Message)` (404 z tekstem).
- `Create`/`Edit` POST: `ModelState.IsValid` sprawdza **atrybuty z `P06Shop.Shared.Models.Product`** (`[Required]`, `[MaxLength]`, `[Range]`): polskie komunikaty pod polami, zanim cokolwiek poleci do API. Gdy API odmówi (albo nie działa): `ModelState.AddModelError(string.Empty, response.Message)` → komunikat w `asp-validation-summary="ModelOnly"`, formularz zostaje z danymi.
- `DeleteConfirmed`: gdy API odmówi (cena > 1000 zł), pobieramy produkt jeszcze raz i **zostajemy na stronie Delete** z powodem odmowy.
- `[Bind]`, `[ValidateAntiForgeryToken]`, `[HttpPost, ActionName("Delete")]`, PRG: dokładnie jak w P10. Struktura jest identyczna, podmieniona jest tylko warstwa danych. To efekt oddzielenia warstw.

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/Views/Products/Index.cshtml` (po kopii i zamianie):

```diff
-@model IEnumerable<P10ShopWebAppMVC.Client.Models.Product>
+@model IEnumerable<P06Shop.Shared.Models.Product>
 
 @{
     ViewData["Title"] = "Index";
 }
 
 <h1>Index</h1>
+<p>Lista pobrana z API (<code>IProductService</code> → <code>ProductHttpService</code> z P06Shop.Shared). Produktów: @Model.Count()</p>
+
+@* Gdy API nie działa, kontroler przekazuje komunikat z ServiceResponse.Message *@
+@if (ViewBag.ErrorMessage != null)
+{
+    <div class="alert alert-danger">@ViewBag.ErrorMessage</div>
+}
 
 <p>
     <a asp-action="Create">Create New</a>
⋯
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Products/Index.cshtml  (wygenerowane w P10 i skopiowane – nie przepisujemy, porównujemy; dopiski z bloku 🟩 powyżej) *@
@model IEnumerable<P06Shop.Shared.Models.Product>

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>
<p>Lista pobrana z API (<code>IProductService</code> → <code>ProductHttpService</code> z P06Shop.Shared). Produktów: @Model.Count()</p>

@* Gdy API nie działa, kontroler przekazuje komunikat z ServiceResponse.Message *@
@if (ViewBag.ErrorMessage != null)
{
    <div class="alert alert-danger">@ViewBag.ErrorMessage</div>
}

<p>
    <a asp-action="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Title)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Description)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Barcode)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Price)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.ReleaseDate)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Title)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Description)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Barcode)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Price)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ReleaseDate)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/Views/Products/Delete.cshtml`:

```diff
-@model P10ShopWebAppMVC.Client.Models.Product
+@model P06Shop.Shared.Models.Product
⋯
 <h3>Are you sure you want to delete this?</h3>
+@* Miejsce na komunikat z API, np. odmowa usunięcia produktu droższego niż 1000 zł *@
+<div asp-validation-summary="ModelOnly" class="text-danger"></div>
 <div>
     <h4>Product</h4>
⋯
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Products/Delete.cshtml  (wygenerowane w P10 i skopiowane – nie przepisujemy, porównujemy; dopiski z bloku 🟩 powyżej) *@
@model P06Shop.Shared.Models.Product

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

<h3>Are you sure you want to delete this?</h3>
@* Miejsce na komunikat z API, np. odmowa usunięcia produktu droższego niż 1000 zł *@
<div asp-validation-summary="ModelOnly" class="text-danger"></div>
<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Title)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Title)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Description)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Description)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Barcode)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Barcode)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Price)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Price)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.ReleaseDate)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.ReleaseDate)
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-action="Index">Back to List</a>
    </form>
</div>
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Products/Create.cshtml  (wygenerowane – nie przepisujemy, porównujemy; kopia z P10, zmieniony tylko @model) *@
@model P06Shop.Shared.Models.Product

@{
    ViewData["Title"] = "Create";
}

<h1>Create</h1>

<h4>Product</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Description" class="control-label"></label>
                <input asp-for="Description" class="form-control" />
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Barcode" class="control-label"></label>
                <input asp-for="Barcode" class="form-control" />
                <span asp-validation-for="Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Price" class="control-label"></label>
                <input asp-for="Price" class="form-control" />
                <span asp-validation-for="Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="ReleaseDate" class="control-label"></label>
                <input asp-for="ReleaseDate" class="form-control" />
                <span asp-validation-for="ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Products/Edit.cshtml  (wygenerowane – nie przepisujemy, porównujemy; kopia z P10, zmieniony tylko @model) *@
@model P06Shop.Shared.Models.Product

@{
    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

<h4>Product</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Description" class="control-label"></label>
                <input asp-for="Description" class="form-control" />
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Barcode" class="control-label"></label>
                <input asp-for="Barcode" class="form-control" />
                <span asp-validation-for="Barcode" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Price" class="control-label"></label>
                <input asp-for="Price" class="form-control" />
                <span asp-validation-for="Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="ReleaseDate" class="control-label"></label>
                <input asp-for="ReleaseDate" class="form-control" />
                <span asp-validation-for="ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Products/Details.cshtml  (wygenerowane – nie przepisujemy, porównujemy; kopia z P10, zmieniony tylko @model) *@
@model P06Shop.Shared.Models.Product

@{
    ViewData["Title"] = "Details";
}

<h1>Details</h1>

<div>
    <h4>Product</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Title)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Title)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Description)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Description)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Barcode)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Barcode)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Price)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Price)
        </dd>
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.ReleaseDate)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.ReleaseDate)
        </dd>
    </dl>
</div>
<div>
    <a asp-action="Edit" asp-route-id="@Model?.Id">Edit</a> |
    <a asp-action="Index">Back to List</a>
</div>
```

💡 **Wyjaśnienie (widoki):** `Views/_ViewImports.cshtml` z szablonu ma `@using P11ShopWebAppMVCApi.Client.Models`; zostaje, bo tam jest `ErrorViewModel`. Widoki podają pełną nazwę `P06Shop.Shared.Models.Product`, więc nic więcej nie trzeba importować. Model z Shared ma atrybuty walidacji, więc `asp-for` wygeneruje `data-val-required="Nazwa jest wymagana."`: polskie komunikaty **w przeglądarce**, choć w P10 były angielskie (`The Title field is required.`).

🟩 **Co dopisujemy** w `P11ShopWebAppMVCApi.Client/Views/Shared/_Layout.cshtml`:

```diff
⋯
                         <li class="nav-item">
                             <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                         </li>
+                        <li class="nav-item">
+                            <a class="nav-link text-dark" asp-area="" asp-controller="Products" asp-action="Index">Products (API)</a>
+                        </li>
                     </ul>
⋯
```

```cshtml
@* P11ShopWebAppMVCApi.Client/Views/Shared/_Layout.cshtml *@
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - P11ShopWebAppMVCApi.Client</title>
    <script type="importmap"></script>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/P11ShopWebAppMVCApi.Client.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">P11ShopWebAppMVCApi.Client</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Products" asp-action="Index">Products (API)</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2026 - P11ShopWebAppMVCApi.Client - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### ✅ Punkt kontrolny

- Dziesięć kroków tabeli z kroku 10 przechodzi; w konsoli API widać `INSERT`/`UPDATE`/`DELETE` dokładnie wtedy, gdy powinny.
- Wyłączone API → alert, nie wyjątek.
- `P11ShopWebAppMVCApi.Client.csproj` nie ma żadnego `PackageReference`; w projekcie nie ma folderu `Data` ani `Models/Product.cs`; w `appsettings*.json` nie ma connection stringa. Pełny CRUD działa w przeglądarce bez żadnego połączenia P11 z bazą.

### Dlaczego tak

- **P11 nie zna bazy**: jedna logika biznesowa (reguła 1000 zł) w API działa dla WPF, przeglądarki i jutro dla Blazora. Każdy klient na bazie to kolejne miejsce z dostępem do bazy i kolejna kopia modelu.
- `ApiBaseUrl` z końcowym `/` – patrz wyjaśnienie `AppSettings`.
- 🔧 `ProductHttpService`: w edycjach 26/31 `ProductService` leżał w projekcie WPF (`internal`), w dniu 16 przenoszono go do Shared (zmiana na `public`, namespace `P03…` zostawał, bo zmiana psuła P05), a `SendAsync<T>` dopisywano na końcu dnia. P11 dostawał tymczasowo pakiety EF, scaffold bazy i generator, po czym usuwano `Data/` i `Models/Product.cs`, a pakiety zostawały. Teraz serwis jest w Shared od dnia 13, obsługa błędów od dnia 14, a P11 nie ma śladu EF (ani pakietu, ani connection stringa); kontroler jest ręczny.
- 🔧 `Configure<AppSettings>` + `IOptions` w `AddHttpClient((sp, client) => …)`: ta sama konwencja, co w P03. W edycji 26 było `GetSection(nameof(AppSettings)).Get<AppSettings>() ?? throw …` i `AddHttpClient(client => …)` ze zmienną lokalną, w 31 `builder.Configuration["ApiBaseUrl"]` (string bez klasy).
- 🔧 `Index` przy błędzie zwraca pustą listę z `ViewBag.ErrorMessage` (w 26 było `Problem(...)`, czyli JSON ProblemDetails w przeglądarce): użytkownik widzi stronę z komunikatem i menu.
- `Details`/`Edit`/`Delete` GET → `NotFound(response.Message)`: API edycji 32 odpowiada 404 z `Fail("Nie znaleziono …")`, a `SendAsync` przepuszcza tę kopertę.
- 🔧 `DeleteConfirmed` po odmowie pobiera produkt ponownie i zostaje na stronie Delete z komunikatem (w 26 w kodzie była pusta strona 404): reguła biznesowa jest widoczna w UI.
- Walidacja z atrybutów `P06Shop.Shared.Models.Product` działa w trzech miejscach bez żadnego kodu w P11: w przeglądarce (`data-val-*` z `asp-for`), w `ModelState` i w API (`[ApiController]` → 400 → `TryReadValidationErrors`).
- Kontroler ręczny zamiast tymczasowego scaffoldu (jak w 26): P11 nie dostaje pakietów EF, których potem trzeba by się pozbywać.

### Częste problemy

| Objaw | Przyczyna i rozwiązanie |
|---|---|
| `Brak połączenia z API` mimo uruchomionego API | profil API inny niż `https` (port 5008 zamiast 7127) albo `ApiBaseUrl` wskazuje Azure z dnia 15; sprawdzamy `appsettings.json` P11 i okno konsoli API (`Now listening on: https://localhost:7127`) |
| `API odpowiedziało kodem 404 (Not Found)` przy **każdej** operacji | `ApiBaseUrl` bez końcowego `/` (`https://localhost:7127` + `api/products` = `https://localhost:7127api/products`) |
| `Przekroczono czas oczekiwania na odpowiedź API.` | API zatrzymane na breakpointcie dłużej niż 10 s |
| błąd certyfikatu (`The SSL connection could not be established`) | `dotnet dev-certs https --trust`, restart VS |
| `The type or namespace name 'Product' could not be found` | zamiana `@model` nie objęła wszystkich widoków albo w kontrolerze brak `using P06Shop.Shared.Models;` |
| `CS0246: IProductService` | brak `using P06Shop.Shared.Services;` albo brak referencji do `P06Shop.Shared` |
| `Cena musi być w przedziale 0,01–100 000.` dla `1299.00` | brak `UseRequestLocalization` w `Program.cs` P11 (serwer parsuje po polsku) |
| po **Delete** produktu > 1000 zł pusta strona 404 | w `DeleteConfirmed` brak ponownego pobrania produktu (`GetProductAsync`) albo w `Delete.cshtml` brak `asp-validation-summary` |
| dane w P11 inne niż w P09 | API pracuje na Azure SQL (User Secrets), P09 na LocalDB; patrz sekcja 0.2, krok 4 |
| generator `controller` w P11 nie działa | to oczekiwane (brak EF); kontroler piszemy ręcznie (krok 5) |

Mamy dwa podejścia obok siebie: P10 na bazie (szybko, ale kopia modelu i brak reguł biznesowych) i P11 przez API (jedna logika dla wszystkich klientów, zmiana adresu w konfiguracji). Jutro powstanie trzeci klient tego API, tym razem działający w przeglądarce.

---

## 10. Podsumowanie dnia, pytania i zapis pracy

### Podsumowanie (5 min)

1. **Aplikacje server‑side**: żądanie → kod na serwerze → HTML; GET w adresie, POST w treści; HTTP jest bezstanowe; token antiforgery.
2. **Razor Pages**: strona = `.cshtml` + `PageModel`, `OnGet`/`OnPost`/`OnPostAdd`, `Request.Form` vs `[BindProperty]`, `TempData`, Tag Helpery `asp-page`, `asp-for`.
3. **Database First**: cztery pakiety, `dotnet ef dbcontext scaffold "Name=…" --no-onconfiguring --force`, `partial`, ten sam `AddDbContext` co w API.
4. **Scaffolding ekranów**: `razorpage` i `controller`, PRG, `[Bind]`, `[ValidateAntiForgeryToken]`, `ActionName`, usuwanie tylko przez POST.
5. **MVC**: kontroler/akcja/widok, `ViewBag` vs silnie typowany model, `asp-for`.
6. **Kultura**: `UseRequestLocalization` i problem z przecinkiem (oraz jego pułapka: `249,99` → `24999.00`).
7. **MVC przez API**: `AppSettings` + `IOptions`, typed `HttpClient`, kontroler na `IProductService`, komunikaty z `ServiceResponse` w `asp-validation-summary`, reguła 1000 zł w jednym miejscu, API wyłączone bez wyjątku, walidacja z atrybutów Shared po obu stronach.
 