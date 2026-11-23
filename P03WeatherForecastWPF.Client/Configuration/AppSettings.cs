namespace P03WeatherForecastWPF.Client.Configuration;

/// <summary>
/// Klasa odpowiadająca sekcji "AppSettings" w appsettings.json.
/// Wzorzec Options: zamiast czytać configuration["AppSettings:DefaultLanguage"]
/// dostajemy silnie typowany obiekt przez IOptions&lt;AppSettings&gt;.
/// </summary>
public class AppSettings
{
    /// <summary>Nazwa sekcji w pliku konfiguracyjnym.</summary>
    public const string SectionName = "AppSettings";

    /// <summary>Język nazw miast zwracanych przez Open-Meteo (np. "pl", "en").</summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Adres naszego API (P05Shop.API). Lokalnie https://localhost:7127/,
    /// po wdrożeniu do chmury (dzień 15) wystarczy zmienić wartość w pliku.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://localhost:7127/";
}
