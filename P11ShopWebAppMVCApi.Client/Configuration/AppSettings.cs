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
