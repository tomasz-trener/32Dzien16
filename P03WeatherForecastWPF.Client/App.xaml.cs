using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using P03WeatherForecastWPF.Client.Configuration;
using P03WeatherForecastWPF.Client.Services;
using P03WeatherForecastWPF.Client.Services.Dialogs;
using P03WeatherForecastWPF.Client.Services.Navigation;
using P03WeatherForecastWPF.Client.Services.Speech;
using P03WeatherForecastWPF.Client.ViewModels;
using P03WeatherForecastWPF.Client.Views;
using P06Shop.Shared.Services;

namespace P03WeatherForecastWPF.Client;

/// <summary>
/// Punkt startowy aplikacji. Zamiast StartupUri budujemy Generic Host:
/// konfiguracja (appsettings.json, User Secrets, zmienne środowiskowe),
/// logowanie i kontener Dependency Injection w jednym miejscu –
/// dokładnie tak samo jak w ASP.NET Core (dzień 13).
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        // --- Sekrety: User Secrets (secrets.json w profilu użytkownika, poza repozytorium).
        // Host dodaje je sam tylko w środowisku Development; aplikacja desktopowa nie ma tego ustawienia,
        // więc dodajemy jawnie. Wymaga <UserSecretsId> w .csproj (prawy przycisk → Manage User Secrets).
        builder.Configuration.AddUserSecrets<App>(optional: true);

        // --- Konfiguracja: sekcje z appsettings.json (+ User Secrets) → klasy ustawień (IOptions<T>)
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
        builder.Services.Configure<SpeechSettings>(builder.Configuration.GetSection(SpeechSettings.SectionName));

        ConfigureServices(builder.Services);

        _host = builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // --- Serwisy (logika, dostęp do danych)
        // AddHttpClient<TInterface, TImpl>: kontener tworzy HttpClient przez IHttpClientFactory
        // i wstrzykuje go do konstruktora OpenMeteoService. Serwis sam nie tworzy klienta.
        services.AddHttpClient<IMeteoService, OpenMeteoService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Wersja bez internetu – odkomentuj, a OpenMeteoService zakomentuj:
        // services.AddSingleton<IMeteoService, FakeMeteoService>();

        // Klient naszego API (implementacja z P06Shop.Shared). Adres bazowy z appsettings.json:
        // (sp, client) => ... pozwala sięgnąć do kontenera po IOptions<AppSettings> w momencie tworzenia klienta
        services.AddHttpClient<IProductService, ProductHttpService>((sp, client) =>
        {
            AppSettings settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            client.BaseAddress = new Uri(settings.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Nawigacja między oknami i okienka komunikatów – jedna instancja na aplikację
        services.AddSingleton<INavigationService, WpfNavigationService>();
        services.AddSingleton<IMessageDialogService, WpfMessageDialogService>();

        // Rozpoznawanie mowy (Azure AI Speech); klucz z User Secrets przez IOptions<SpeechSettings>
        services.AddSingleton<ISpeechService, AzureSpeechService>();

        // --- ViewModele
        // Singleton: jeden MainViewModel na całą aplikację (stan okna przeżywa jego ponowne otwarcie)
        services.AddSingleton<MainViewModel>();
        // Transient: każde otwarcie okna sklepu dostaje świeży ViewModel
        services.AddTransient<ProductsViewModel>();

        // --- Widoki (okna)
        // Transient: nowe okno przy każdym żądaniu
        services.AddTransient<MainWindow>();
        services.AddTransient<ShopProductsView>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        // Bindingi WPF domyślnie formatują liczby i daty po amerykańsku (en-US), niezależnie od ustawień Windows.
        // Ta linia każe im używać kultury systemu: na polskim Windows cena "12,50" i data "01.05.2025".
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        _host.Start();

        // Kontener tworzy MainWindow i wszystko, czego ono potrzebuje
        // (MainViewModel → IMeteoService, INavigationService → HttpClient + IOptions<AppSettings>)
        MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        _host.StopAsync().GetAwaiter().GetResult();
        _host.Dispose();
    }
}
